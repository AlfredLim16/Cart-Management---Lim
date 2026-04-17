using System;

namespace Cart_Management.Core.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid SellerId { get; set; }
    }
}
