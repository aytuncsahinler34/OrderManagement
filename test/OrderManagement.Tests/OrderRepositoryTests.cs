using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Enums;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Tests;

public class OrderRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new OrderRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddOrderToDatabase()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            ProductName = "Test Ürün",
            Price = 99.99m,
            Status = OrderStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(order);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.ProductName, result.ProductName);
        Assert.Equal(order.Price, result.Price);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            ProductName = "Test Ürün",
            Price = 150m,
            Status = OrderStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };
        await _repository.CreateAsync(order);

        // Act
        var result = await _repository.GetByIdAsync(order.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.ProductName, result.ProductName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders()
    {
        // Arrange
        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            ProductName = "Ürün 1",
            Price = 100m,
            Status = OrderStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };
        var order2 = new Order
        {
            Id = Guid.NewGuid(),
            ProductName = "Ürün 2",
            Price = 200m,
            Status = OrderStatus.Processing,
            CreatedDate = DateTime.UtcNow.AddMinutes(1)
        };

        await _repository.CreateAsync(order1);
        await _repository.CreateAsync(order2);

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrderStatus()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            ProductName = "Test Ürün",
            Price = 300m,
            Status = OrderStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };
        await _repository.CreateAsync(order);

        // Act
        order.Status = OrderStatus.Completed;
        await _repository.UpdateAsync(order);

        var updatedOrder = await _repository.GetByIdAsync(order.Id);

        // Assert
        Assert.NotNull(updatedOrder);
        Assert.Equal(OrderStatus.Completed, updatedOrder.Status);
        Assert.NotNull(updatedOrder.UpdatedDate);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
