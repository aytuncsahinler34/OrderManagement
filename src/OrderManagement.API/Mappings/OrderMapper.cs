using OrderManagement.API.DTOs;
using OrderManagement.Core.Entities;

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
}