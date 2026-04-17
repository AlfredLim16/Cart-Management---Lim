using System;

namespace Cart_Management.Core.Models
{
    public class CartVoucher
    {
        public Guid CartId { get; set; }
        public Guid VoucherId { get; set; }
    }
}
