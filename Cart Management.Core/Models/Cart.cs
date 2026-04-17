using System;
using System.Collections.Generic;

namespace Cart_Management.Core.Models
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<CartItem> Items { get; set; }
        public List<Voucher> AppliedVouchers { get; set; } = new List<Voucher>();
    }
}
        