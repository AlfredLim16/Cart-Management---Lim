using CartManagementModels;
namespace CartManagementBusinessLogic
{
    public class CartRules
    {
        public bool validateCart(List<CartItemModel> items)
        {
            // cart must have at least one item and that each item has valid quantity and price
            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("Cart must contain at least one item.");
            }

            foreach (var item in items)
            {
                if (int.TryParse(item.Name, out _))
                {
                    throw new ArgumentException("Cart Item name cannot be a number.");
                }
                if (item.Quantity <= 0)
                {
                    throw new ArgumentException($"Item '{item.Name}' has invalid quantity.");
                }
                if (item.Price < 0)
                {
                    throw new ArgumentException($"Item '{item.Name}' has invalid price.");
                }
            }
            return true;
        }
        public decimal applyCartDiscount(decimal total, decimal discountPercentage)
        {
            if (discountPercentage < 0 || discountPercentage > 100)
            {
                throw new ArgumentException("Cart Discount must be between 0 and 100.");
            }
            return total * ((100 - discountPercentage) / 100);
        }
        public bool validateMinimumOrder(decimal total, decimal minimumAmount)
        {
            return total >= minimumAmount;
        }
    }
}