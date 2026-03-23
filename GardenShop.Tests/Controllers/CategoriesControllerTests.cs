using GardenShop.Api.Controllers;
using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static GardenShop.Application.DTO.CategoryDto;
using static GardenShop.Application.Requests.CategoryRequests;

namespace GardenShop.Api.Tests.Controllers;

public class CategoriesControllerTests
{
	private readonly Mock<ICategoryService> _mockService;
	private readonly Mock<ILogger<CategoriesController>> _mockLogger;
	private readonly CategoriesController _controller;

	public CategoriesControllerTests()
	{
		_mockService = new Mock<ICategoryService>();
		_mockLogger = new Mock<ILogger<CategoriesController>>();
		_controller = new CategoriesController(_mockService.Object, _mockLogger.Object);
	}

	[Fact]
	public async Task GetCategoriesAsync_ReturnsOkWithCategories()
	{
		// Arrange
		var categories = new List<CategoryListItemDto>
		{
			new(1, "Category 1"),
			new(2, "Category 2")
		};
		_mockService.Setup(s => s.GetListAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(categories);

		// Act
		var result = await _controller.GetCategoriesAsync(CancellationToken.None);

		// Assert
		var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
		var returnValue = okResult.Value.Should().BeAssignableTo<List<CategoryListItemDto>>().Subject;
		returnValue.Should().HaveCount(2);
	}

	[Fact]
	public async Task GetCategoriesAsync_WhenExceptionThrown_Returns500()
	{
		// Arrange
		_mockService.Setup(s => s.GetListAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		// Act
		var result = await _controller.GetCategoriesAsync(CancellationToken.None);

		// Assert
		var statusResult = result.Result.Should().BeOfType<StatusCodeResult>().Subject;
		statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
	}

	[Fact]
	public async Task GetCategoryAsync_WhenCategoryExists_ReturnsOk()
	{
		// Arrange
		var category = new CategoryDetailsDto(1, "Category 1", new List<CategoryProductListItemDto>());
		_mockService.Setup(s => s.GetDetailsAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(category);

		// Act
		var result = await _controller.GetCategoryAsync(1, CancellationToken.None);

		// Assert
		var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
		var returnValue = okResult.Value.Should().BeOfType<CategoryDetailsDto>().Subject;
		returnValue.Id.Should().Be(1);
	}

	[Fact]
	public async Task GetCategoryAsync_WhenNotFound_ReturnsNotFound()
	{
		// Arrange
		_mockService.Setup(s => s.GetDetailsAsync(99, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new CategoryNotFoundException(99));

		// Act
		var result = await _controller.GetCategoryAsync(99, CancellationToken.None);

		// Assert
		result.Result.Should().BeOfType<NotFoundResult>();
	}

	[Fact]
	public async Task CreateCategoryAsync_WithValidData_ReturnsCreated()
	{
		// Arrange
		var request = new CreateCategoryRequest("New Category");
		var created = new CategoryListItemDto(1, "New Category");
		_mockService.Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
			.ReturnsAsync(created);

		// Act
		var result = await _controller.CreateCategoryAsync(request, CancellationToken.None);

		// Assert
		var createdResult = result.Result.Should().BeOfType<CreatedResult>().Subject;
		createdResult.Location.Should().Be("/api/categories/1");
		var returnValue = createdResult.Value.Should().BeOfType<CategoryListItemDto>().Subject;
		returnValue.Id.Should().Be(1);
	}

	[Fact]
	public async Task CreateCategoryAsync_WithInvalidData_ReturnsBadRequest()
	{
		// Arrange
		var request = new CreateCategoryRequest("");
		_mockService.Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new ValidationException("Name is required."));

		// Act
		var result = await _controller.CreateCategoryAsync(request, CancellationToken.None);

		// Assert
		var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
		badRequestResult.Value.Should().Be("Name is required.");
	}

	[Fact]
	public async Task CreateCategoryAsync_WithDuplicateName_ReturnsConflict()
	{
		// Arrange
		var request = new CreateCategoryRequest("Existing");
		_mockService.Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new CategoryNameConflictException("Existing"));

		// Act
		var result = await _controller.CreateCategoryAsync(request, CancellationToken.None);

		// Assert
		result.Result.Should().BeOfType<ConflictResult>();
	}

	[Fact]
	public async Task UpdateCategoryAsync_WithValidData_ReturnsOk()
	{
		// Arrange
		var request = new UpdateCategoryRequest("Updated Name");
		var updated = new CategoryListItemDto(1, "Updated Name");
		_mockService.Setup(s => s.UpdateAsync(1, request, It.IsAny<CancellationToken>()))
			.ReturnsAsync(updated);

		// Act
		var result = await _controller.UpdateCategoryAsync(1, request, CancellationToken.None);

		// Assert
		var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
		var returnValue = okResult.Value.Should().BeOfType<CategoryListItemDto>().Subject;
		returnValue.Name.Should().Be("Updated Name");
	}

	[Fact]
	public async Task DeleteCategoryAsync_WhenSuccessful_ReturnsNoContent()
	{
		// Arrange
		_mockService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _controller.DeleteCategoryAsync(1, CancellationToken.None);

		// Assert
		result.Should().BeOfType<NoContentResult>();
	}

	[Fact]
	public async Task DeleteCategoryAsync_WhenInUse_ReturnsConflict()
	{
		// Arrange
		_mockService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new CategoryInUseException(1));

		// Act
		var result = await _controller.DeleteCategoryAsync(1, CancellationToken.None);

		// Assert
		result.Should().BeOfType<ConflictResult>();
	}
}