using System.ComponentModel.DataAnnotations.Schema;

namespace CartManagementModels
{  
    public class Cart
    {
        private Guid _cartId = Guid.NewGuid();
        private Guid _accountId;
        private List<CartItem> _items = new List<CartItem>();
        private short _threshold = 100;

        public Guid CartId
        {
            get { return _cartId; }
            set { _cartId = value;}
        }

        public Guid AccountId
        {
            get { return _accountId; }
            set { _accountId = value;}
        }

        public List<CartItem> Items
        {
            get { return _items; }
            set { _items = value;}
        }

        public short Threshold
        {
            get { return _threshold; }
            set { _threshold = value;}
        }
    }
    public class CartItem
    {
        public Guid CartItemId { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public Guid SellerId { get; set; }
        public required string ProductName { get; set; }
        public byte Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }
    }
}