using CartManagementModels;
namespace CartManagementBusinessLogic
{
    public class CartItemRules
    {
        public bool validateCartItem(CartItemModel item)
        {
            // validation rules for a cart item such as non-empty name, not number, positive quantity, and non-negative price
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                throw new ArgumentException("Cart Item name cannot be empty.");
            }
            if (int.TryParse(item.Name, out _))
            {
                throw new ArgumentException("Cart Item name cannot be a number.");
            }
            if (item.Quantity <= 0)
            {
                throw new ArgumentException("Cart Item quantity must be greater than zero.");
            }
            if (item.Price < 0)
            {
                throw new ArgumentException("Cart Item price cannot be negative.");
            }
            return true;
        }     
        public bool validateMaxinumCartItemQuantity(int newQuantity, int maxQuantity)
        {
            // max quantity for a single cart item is 99 to prevent unrealistic orders
            return newQuantity > 0 && newQuantity <= maxQuantity;
        }
        public decimal applyCartItemDiscount(CartItemModel item, decimal discountPercentage)
        {
            if (discountPercentage < 0 || discountPercentage > 100)
            {
                throw new ArgumentException("Cart Item discount must be between 0 and 100.");
            }
            return item.Price * ((100 - discountPercentage) / 100);
        }
    }
}