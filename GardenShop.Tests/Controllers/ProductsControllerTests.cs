using GardenShop.Api.Controllers;
using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static GardenShop.Application.DTO.ProductDto;
using static GardenShop.Application.Requests.ProductRequests;

namespace GardenShop.Api.Tests.Controllers;

public class ProductsControllerTests
{
	private readonly Mock<IProductService> _mockService;
	private readonly Mock<ILogger<ProductsController>> _mockLogger;
	private readonly ProductsController _controller;

	public ProductsControllerTests()
	{
		_mockService = new Mock<IProductService>();
		_mockLogger = new Mock<ILogger<ProductsController>>();
		_controller = new ProductsController(_mockService.Object, _mockLogger.Object);
	}

	[Fact]
	public async Task GetProductsAsync_ReturnsOkWithProducts()
	{
		// Arrange
		var products = new List<ProductListItemDto>
		{
			new(1, "Product 1", 10.99m, 5, 1),
			new(2, "Product 2", 20.99m, 10, 1)
		};
		_mockService.Setup(s => s.GetListAsync(null, It.IsAny<CancellationToken>()))
			.ReturnsAsync(products);

		// Act
		var result = await _controller.GetProductsAsync(null, CancellationToken.None);

		// Assert
		var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
		var returnValue = okResult.Value.Should().BeAssignableTo<List<ProductListItemDto>>().Subject;
		returnValue.Should().HaveCount(2);
	}

	[Fact]
	public async Task GetProductAsync_WhenProductExists_ReturnsOk()
	{
		// Arrange
		var product = new ProductDetailsDto(1, "Product 1", "Description", 10.99m, 5, 1);
		_mockService.Setup(s => s.GetDetailsAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(product);

		// Act
		var result = await _controller.GetProductAsync(1, CancellationToken.None);

		// Assert
		var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
		var returnValue = okResult.Value.Should().BeOfType<ProductDetailsDto>().Subject;
		returnValue.Id.Should().Be(1);
	}

	[Fact]
	public async Task GetProductAsync_WhenNotFound_ReturnsNotFound()
	{
		// Arrange
		_mockService.Setup(s => s.GetDetailsAsync(99, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new ProductNotFoundException(99));

		// Act
		var result = await _controller.GetProductAsync(99, CancellationToken.None);

		// Assert
		result.Result.Should().BeOfType<NotFoundResult>();
	}

	[Fact]
	public async Task CreateProductAsync_WithValidData_ReturnsCreated()
	{
		// Arrange
		var request = new CreateProductRequest("New Product", "Description", 49.99m, 10, 1);
		var created = new ProductDetailsDto(1, "New Product", "Description", 49.99m, 10, 1);
		_mockService.Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
			.ReturnsAsync(created);

		// Act
		var result = await _controller.CreateProductAsync(request, CancellationToken.None);

		// Assert
		var createdResult = result.Result.Should().BeOfType<CreatedResult>().Subject;
		createdResult.Location.Should().Be("/api/products/1");
	}

	[Fact]
	public async Task CreateProductAsync_WithInvalidData_ReturnsBadRequest()
	{
		// Arrange
		var request = new CreateProductRequest("", "Description", 10.0m, 5, 1);
		_mockService.Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new ValidationException("Name is required."));

		// Act
		var result = await _controller.CreateProductAsync(request, CancellationToken.None);

		// Assert
		var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
		badRequestResult.Value.Should().Be("Name is required.");
	}

	[Fact]
	public async Task UpdateProductAsync_WithValidData_ReturnsOk()
	{
		// Arrange
		var request = new UpdateProductRequest("Updated Product", "New Description", 59.99m, 15, 1);
		var updated = new ProductDetailsDto(1, "Updated Product", "New Description", 59.99m, 15, 1);
		_mockService.Setup(s => s.UpdateAsync(1, request, It.IsAny<CancellationToken>()))
			.ReturnsAsync(updated);

		// Act
		var result = await _controller.UpdateProductAsync(1, request, CancellationToken.None);

		// Assert
		var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
		var returnValue = okResult.Value.Should().BeOfType<ProductDetailsDto>().Subject;
		returnValue.Name.Should().Be("Updated Product");
	}

	[Fact]
	public async Task DeleteProductAsync_WhenSuccessful_ReturnsNoContent()
	{
		// Arrange
		_mockService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _controller.DeleteProductAsync(1, CancellationToken.None);

		// Assert
		result.Should().BeOfType<NoContentResult>();
	}

	[Fact]
	public async Task DeleteProductAsync_WhenInUse_ReturnsConflict()
	{
		// Arrange
		_mockService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new ProductInUseException(1));

		// Act
		var result = await _controller.DeleteProductAsync(1, CancellationToken.None);

		// Assert
		result.Should().BeOfType<ConflictResult>();
	}
}