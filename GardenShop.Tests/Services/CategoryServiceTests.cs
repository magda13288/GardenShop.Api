using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Abstractions.Persistence;
using GardenShop.Application.Exceptions;
using GardenShop.Application.Services;
using GardenShop.Domain.Entities;
using Microsoft.Extensions.Logging;
using static GardenShop.Application.DTO.CategoryDto;
using static GardenShop.Application.Requests.CategoryRequests;

namespace GardenShop.Tests.Services;

public class CategoryServiceTests
{
	private readonly Mock<ICategoryRepository> _mockRepo;
	private readonly Mock<IUnitOfWork> _mockUow;
	private readonly Mock<ILogger<CategoryService>> _mockLogger;
	private readonly ICategoryService _service;

	public CategoryServiceTests()
	{
		_mockRepo = new Mock<ICategoryRepository>();
		_mockUow = new Mock<IUnitOfWork>();
		_mockLogger = new Mock<ILogger<CategoryService>>();
		_service = new CategoryService(_mockRepo.Object, _mockUow.Object, _mockLogger.Object);
	}

	[Fact]
	public async Task GetListAsync_ReturnsAllCategories()
	{
		// Arrange
		var categories = new List<Category>
		{
			new() { Id = 1, Name = "Category 1" },
			new() { Id = 2, Name = "Category 2" }
		};
		_mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(categories);

		// Act
		var result = await _service.GetListAsync(CancellationToken.None);

		// Assert
		result.Should().HaveCount(2);
		result[0].Name.Should().Be("Category 1");
		result[1].Name.Should().Be("Category 2");
	}

	[Fact]
	public async Task GetDetailsAsync_WhenCategoryExists_ReturnsCategoryDetails()
	{
		// Arrange
		var category = new Category
		{
			Id = 1,
			Name = "Test Category",
			Products = new List<Product>
			{
				new() { Id = 1, Name = "Product 1", Price = 10.99m, Stock = 5, CategoryId = 1 }
			}
		};
		_mockRepo.Setup(r => r.GetByIdWithProductsAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(category);

		// Act
		var result = await _service.GetDetailsAsync(1, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Id.Should().Be(1);
		result.Name.Should().Be("Test Category");
		result.Products.Should().HaveCount(1);
	}

	[Fact]
	public async Task GetDetailsAsync_WhenCategoryNotFound_ThrowsCategoryNotFoundException()
	{
		// Arrange
		_mockRepo.Setup(r => r.GetByIdWithProductsAsync(99, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Category?)null);

		// Act & Assert
		await _service.Invoking(s => s.GetDetailsAsync(99, CancellationToken.None))
			.Should().ThrowAsync<CategoryNotFoundException>();
	}

	[Fact]
	public async Task CreateAsync_WithValidData_CreatesCategory()
	{
		// Arrange
		var request = new CreateCategoryRequest("New Category");
		_mockRepo.Setup(r => r.NameExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);
		_mockRepo.Setup(r => r.Add(It.IsAny<Category>()))
			.Callback<Category>(c => c.Id = 1);

		// Act
		var result = await _service.CreateAsync(request, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Name.Should().Be("New Category");
		_mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task CreateAsync_WithEmptyName_ThrowsValidationException()
	{
		// Arrange
		var request = new CreateCategoryRequest("");

		// Act & Assert
		await _service.Invoking(s => s.CreateAsync(request, CancellationToken.None))
			.Should().ThrowAsync<ValidationException>()
			.WithMessage("Name is required.");
	}

	[Fact]
	public async Task CreateAsync_WithDuplicateName_ThrowsCategoryNameConflictException()
	{
		// Arrange
		var request = new CreateCategoryRequest("Existing Category");
		_mockRepo.Setup(r => r.NameExistsAsync("Existing Category", null, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		// Act & Assert
		await _service.Invoking(s => s.CreateAsync(request, CancellationToken.None))
			.Should().ThrowAsync<CategoryNameConflictException>();
	}

	[Fact]
	public async Task UpdateAsync_WithValidData_UpdatesCategory()
	{
		// Arrange
		var category = new Category { Id = 1, Name = "Old Name" };
		var request = new UpdateCategoryRequest("Updated Name");
		_mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(category);
		_mockRepo.Setup(r => r.NameExistsAsync("Updated Name", 1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		// Act
		var result = await _service.UpdateAsync(1, request, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Name.Should().Be("Updated Name");
		category.Name.Should().Be("Updated Name");
		_mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task UpdateAsync_WhenCategoryNotFound_ThrowsCategoryNotFoundException()
	{
		// Arrange
		var request = new UpdateCategoryRequest("Updated Name");
		_mockRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Category?)null);

		// Act & Assert
		await _service.Invoking(s => s.UpdateAsync(99, request, CancellationToken.None))
			.Should().ThrowAsync<CategoryNotFoundException>();
	}

	[Fact]
	public async Task DeleteAsync_WhenNotInUse_DeletesCategory()
	{
		// Arrange
		var category = new Category { Id = 1, Name = "Test Category" };
		_mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(category);
		_mockRepo.Setup(r => r.IsUsedByProductsAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		// Act
		await _service.DeleteAsync(1, CancellationToken.None);

		// Assert
		_mockRepo.Verify(r => r.Remove(category), Times.Once);
		_mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_WhenInUse_ThrowsCategoryInUseException()
	{
		// Arrange
		var category = new Category { Id = 1, Name = "Test Category" };
		_mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(category);
		_mockRepo.Setup(r => r.IsUsedByProductsAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		// Act & Assert
		await _service.Invoking(s => s.DeleteAsync(1, CancellationToken.None))
			.Should().ThrowAsync<CategoryInUseException>();
	}

	[Fact]
	public async Task DeleteAsync_WhenCategoryNotFound_ThrowsCategoryNotFoundException()
	{
		// Arrange
		_mockRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Category?)null);

		// Act & Assert
		await _service.Invoking(s => s.DeleteAsync(99, CancellationToken.None))
			.Should().ThrowAsync<CategoryNotFoundException>();
	}
}