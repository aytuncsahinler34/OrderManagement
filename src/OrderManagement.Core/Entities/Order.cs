using System;
using System.Collections.Generic;
using System.Linq;
using OrderManagement.Core.Enums;

namespace OrderManagement.Core.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
