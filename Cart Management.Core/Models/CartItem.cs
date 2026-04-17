using Cart_Management.Core.Enums;

using System;

namespace Cart_Management.Core.Models
{
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public CartItemStatus Status { get; set; }
    }
}
