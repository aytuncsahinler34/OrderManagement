using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.API.Controllers;
using OrderManagement.API.DTOs;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Enums;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Tests;

public class OrdersControllerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IMessagePublisher> _mockPublisher;
    private readonly Mock<ILogger<OrdersController>> _mockLogger;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockPublisher = new Mock<IMessagePublisher>();
        _mockLogger = new Mock<ILogger<OrdersController>>();
        _controller = new OrdersController(_mockRepository.Object, _mockPublisher.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnCreatedResult_WithValidData()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            ProductName = "Test Ürün",
            Price = 100m
        };

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<Order>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateOrder(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<OrderResponseDto>(createdResult.Value);

        Assert.Equal(dto.ProductName, response.ProductName);
        Assert.Equal(dto.Price, response.Price);
        Assert.Equal("Pending", response.Status);

        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
        _mockPublisher.Verify(p => p.PublishAsync(It.IsAny<Order>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnOk_WhenOrderExists()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            ProductName = "Test Ürün",
            Price = 150m,
            Status = OrderStatus.Completed,
            CreatedDate = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _controller.GetOrderById(orderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<OrderResponseDto>(okResult.Value);

        Assert.Equal(orderId, response.Id);
        Assert.Equal("Test Ürün", response.ProductName);
        Assert.Equal(150m, response.Price);
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _controller.GetOrderById(orderId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllOrders_ShouldReturnOk_WithOrderList()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order
            {
                Id = Guid.NewGuid(),
                ProductName = "Ürün 1",
                Price = 100m,
                Status = OrderStatus.Pending,
                CreatedDate = DateTime.UtcNow
            },
            new Order
            {
                Id = Guid.NewGuid(),
                ProductName = "Ürün 2",
                Price = 200m,
                Status = OrderStatus.Completed,
                CreatedDate = DateTime.UtcNow
            }
        };

        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(orders);

        // Act
        var result = await _controller.GetAllOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<OrderResponseDto>>(okResult.Value);

        Assert.Equal(2, response.Count());
    }

    [Fact]
    public async Task GetAllOrders_ShouldReturnEmptyList_WhenNoOrders()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Order>());

        // Act
        var result = await _controller.GetAllOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<OrderResponseDto>>(okResult.Value);

        Assert.Empty(response);
    }
}
