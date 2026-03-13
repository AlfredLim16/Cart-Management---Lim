namespace CartManagementModels
{  
    public class CartModel
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemModel> Items { get; set; } = new List<CartItemModel>();
    }  
}