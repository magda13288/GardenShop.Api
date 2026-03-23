using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Abstractions.Persistence;
using GardenShop.Application.Exceptions;
using GardenShop.Application.Services;
using GardenShop.Domain.Entities;
using Microsoft.Extensions.Logging;
using static GardenShop.Application.Requests.ProductRequests;

namespace GardenShop.Api.Tests.Services;

public class ProductServiceTests
{
	private readonly Mock<IProductRepository> _mockRepo;
	private readonly Mock<IUnitOfWork> _mockUow;
	private readonly Mock<ILogger<ProductService>> _mockLogger;
	private readonly IProductService _service;

	public ProductServiceTests()
	{
		_mockRepo = new Mock<IProductRepository>();
		_mockUow = new Mock<IUnitOfWork>();
		_mockLogger = new Mock<ILogger<ProductService>>();
		_service = new ProductService(_mockRepo.Object, _mockUow.Object, _mockLogger.Object);
	}

	[Fact]
	public async Task GetListAsync_ReturnsAllProducts()
	{
		// Arrange
		var products = new List<Product>
		{
			new() { Id = 1, Name = "Product 1", Price = 10.99m, Stock = 5, CategoryId = 1 },
			new() { Id = 2, Name = "Product 2", Price = 20.99m, Stock = 10, CategoryId = 1 }
		};
		_mockRepo.Setup(r => r.GetAllAsync(null, It.IsAny<CancellationToken>()))
			.ReturnsAsync(products);

		// Act
		var result = await _service.GetListAsync(null, CancellationToken.None);

		// Assert
		result.Should().HaveCount(2);
		result[0].Name.Should().Be("Product 1");
		result[1].Name.Should().Be("Product 2");
	}

	[Fact]
	public async Task GetDetailsAsync_WhenProductExists_ReturnsProductDetails()
	{
		// Arrange
		var product = new Product
		{
			Id = 1,
			Name = "Test Product",
			Description = "Test Description",
			Price = 99.99m,
			Stock = 10,
			CategoryId = 1
		};
		_mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(product);

		// Act
		var result = await _service.GetDetailsAsync(1, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Id.Should().Be(1);
		result.Name.Should().Be("Test Product");
		result.Price.Should().Be(99.99m);
	}

	[Fact]
	public async Task GetDetailsAsync_WhenProductNotFound_ThrowsProductNotFoundException()
	{
		// Arrange
		_mockRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Product?)null);

		// Act & Assert
		await _service.Invoking(s => s.GetDetailsAsync(99, CancellationToken.None))
			.Should().ThrowAsync<ProductNotFoundException>();
	}

	[Fact]
	public async Task CreateAsync_WithValidData_CreatesProduct()
	{
		// Arrange
		var request = new CreateProductRequest("New Product", "Description", 49.99m, 5, 1);
		_mockRepo.Setup(r => r.CategoryExistsAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);
		_mockRepo.Setup(r => r.Add(It.IsAny<Product>()))
			.Callback<Product>(p => p.Id = 1);

		// Act
		var result = await _service.CreateAsync(request, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Name.Should().Be("New Product");
		result.Price.Should().Be(49.99m);
		_mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Theory]
	[InlineData("", "Description", 10.0, 5, 1, "Name is required.")]
	[InlineData("Product", "Description", -1.0, 5, 1, "Price must be >= 0.")]
	[InlineData("Product", "Description", 10.0, -1, 1, "Stock must be >= 0.")]
	public async Task CreateAsync_WithInvalidData_ThrowsValidationException(
		string name, string description, decimal price, int stock, long categoryId, string expectedMessage)
	{
		// Arrange
		var request = new CreateProductRequest(name, description, price, stock, categoryId);

		// Act & Assert
		await _service.Invoking(s => s.CreateAsync(request, CancellationToken.None))
			.Should().ThrowAsync<ValidationException>()
			.WithMessage(expectedMessage);
	}

	[Fact]
	public async Task CreateAsync_WithNonExistentCategory_ThrowsValidationException()
	{
		// Arrange
		var request = new CreateProductRequest("Product", "Description", 10.0m, 5, 999);
		_mockRepo.Setup(r => r.CategoryExistsAsync(999, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		// Act & Assert
		await _service.Invoking(s => s.CreateAsync(request, CancellationToken.None))
			.Should().ThrowAsync<ValidationException>()
			.WithMessage("CategoryId does not exist.");
	}

	[Fact]
	public async Task UpdateAsync_WithValidData_UpdatesProduct()
	{
		// Arrange
		var product = new Product
		{
			Id = 1,
			Name = "Old Name",
			Price = 10.0m,
			Stock = 5,
			CategoryId = 1
		};
		var request = new UpdateProductRequest("Updated Name", "New Description", 19.99m, 10, 1);
		_mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(product);
		_mockRepo.Setup(r => r.CategoryExistsAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		// Act
		var result = await _service.UpdateAsync(1, request, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Name.Should().Be("Updated Name");
		result.Price.Should().Be(19.99m);
		product.Name.Should().Be("Updated Name");
		_mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_WhenNotInUse_DeletesProduct()
	{
		// Arrange
		var product = new Product { Id = 1, Name = "Test Product", Price = 10.0m, Stock = 5, CategoryId = 1 };
		_mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(product);
		_mockRepo.Setup(r => r.IsUsedByOrdersAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		// Act
		await _service.DeleteAsync(1, CancellationToken.None);

		// Assert
		_mockRepo.Verify(r => r.Remove(product), Times.Once);
		_mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_WhenInUse_ThrowsProductInUseException()
	{
		// Arrange
		var product = new Product { Id = 1, Name = "Test Product", Price = 10.0m, Stock = 5, CategoryId = 1 };
		_mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(product);
		_mockRepo.Setup(r => r.IsUsedByOrdersAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		// Act & Assert
		await _service.Invoking(s => s.DeleteAsync(1, CancellationToken.None))
			.Should().ThrowAsync<ProductInUseException>();
	}
}