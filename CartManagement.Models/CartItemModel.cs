namespace CartManagementModels
{
    public class CartItemModel
    {      
        public Guid ProductId { get; set; }
        public required string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}