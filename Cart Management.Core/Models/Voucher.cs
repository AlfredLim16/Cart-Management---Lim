using Cart_Management.Core.Enums;
using System;

namespace Cart_Management.Core.Models
{
    public class Voucher
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public VoucherType Type { get; set; }
        public decimal DiscountAmount { get; set; }
        public Guid? SellerId { get; set; }
    }
}
