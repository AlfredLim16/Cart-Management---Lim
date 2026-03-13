using CartManagementModels;
namespace CartManagementDataLogic
{
    public class CartItemLogic
    {
        public decimal computeSubtotal(CartItemModel item)
        {
            // ensures that quantity and price of the cart item are not negative
            if (item.Quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.");
            }
            if (item.Price < 0)
            {
                throw new ArgumentException("Price cannot be negative.");
            }
            return item.Quantity * item.Price;
        }
        public void updateQuantity(CartItemModel item, int newQuantity)
        {
            // ensures that quantity of cart items cannot be negative
            if (newQuantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.");
            }
            item.Quantity = newQuantity;
        }
        public void applyDiscount(CartItemModel item, decimal discountPercentage)
        {
            // ensures that discount is reasonable and prevents potential issues with negative pricing or giving away items for free.
            if (discountPercentage < 0) {
                throw new ArgumentException("Discount cannot be negative.");
            }
            if (discountPercentage > 100)
            {
                throw new ArgumentException("Discount cannot be greater than 100.");
            }          
            var discountFactor = (100 - discountPercentage) / 100;
            item.Price = item.Price * discountFactor;
        }
        public CartItemModel CloneItem(CartItemModel item)
        {
            /* new instance of CartItemModel with same properties as original item, ensuring that changes to cloned item do not affect original item.
               it's useful in scenarios that you want to create a copy of an item for modification without altering original data,
               such as when applying discounts or updating quantities in a shopping cart. */
            return new CartItemModel
            {
                ProductId = item.ProductId,
                Name = item.Name,
                Quantity = item.Quantity,
                Price = item.Price
            };
        }
    }
}