using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.DTOs;
using OrderManagement.API.Mappings;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Enums;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<OrdersController> _logger;
    private const string ORDER_QUEUE = "order-queue";

    public OrdersController(
        IOrderRepository orderRepository,
        IMessagePublisher messagePublisher,
        ILogger<OrdersController> logger)
    {
        _orderRepository = orderRepository;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    /// <summary>
    /// Yeni sipariş oluşturur ve RabbitMQ kuyruğuna gönderir
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        try
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductName = dto.ProductName,
                Price = dto.Price,
                Status = OrderStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            await _orderRepository.CreateAsync(order);
            await _messagePublisher.PublishAsync(order, ORDER_QUEUE);

            _logger.LogInformation("Sipariş oluşturuldu: {OrderId}", order.Id);

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, OrderMapper.ToDto(order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sipariş oluşturulurken hata oluştu");
            return StatusCode(500, "Sipariş oluşturulurken bir hata oluştu");
        }
    }

    /// <summary>
    /// Belirli bir siparişin detaylarını getirir
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
        {
            _logger.LogWarning("Sipariş bulunamadı: {OrderId}", id);
            return NotFound($"Sipariş bulunamadı: {id}");
        }
        return Ok(OrderMapper.ToDto(order));
    }

    /// <summary>
    /// Tüm siparişlerin listesini getirir
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAllOrders()
    {
        var orders = await _orderRepository.GetAllAsync();

        _logger.LogInformation("Toplam {Count} sipariş listelendi", orders.Count());
        return Ok(OrderMapper.ToDto(orders));
    }
}