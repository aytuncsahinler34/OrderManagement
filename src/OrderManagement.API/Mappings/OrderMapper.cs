using OrderManagement.API.DTOs;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Enums;

namespace OrderManagement.API.Mappings;

public static class OrderMapper
{
    public static OrderResponseDto ToDto(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            ProductName = order.ProductName,
            Price = order.Price,
            Status = order.Status.ToString(),
            CreatedDate = order.CreatedDate,
            UpdatedDate = order.UpdatedDate
        };
    }

    public static IEnumerable<OrderResponseDto> ToDto(IEnumerable<Order> orders)
    {
        return orders.Select(ToDto);
    }

    // DTO'dan Entity'ye (Update için)
    public static Order ToEntity(CreateOrderDto dto)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            ProductName = dto.ProductName,
            Price = dto.Price,
            Status = OrderStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };
    }
}