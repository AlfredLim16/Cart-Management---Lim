namespace CartManagementModels
{  
    public class Cart
    {
        public Guid CartId { get; set; }
        public Guid AccountId { get; set; }
        public required List<CartItem> Items { get; set; }
        public byte Threshold { get; set; } = 100;
    }
    public class CartItem
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public Guid SellerId { get; set; }
        public required string ProductName { get; set; }
        public byte Quantity { get; set; } = 1;
        public decimal Price { get; set; }
    }
}