using CartManagementModels;
using CartManagementDataLogic;
using CartManagementBusinessLogic.Exceptions;

namespace CartManagementBusinessLogic.Rules
{
    public class CartRules
    {
        public void ValidateThreshold(Guid cartId, CartItem item, ICartDataLogic dataLogic)
        {
            var cart = dataLogic.Get(cartId);
            if (cart != null)
            {
                int currentCount = cart.Items.Count;
                if (currentCount + 1 > cart.Threshold)
                {
                    throw new CartExceptions("Cart threshold exceeded!");
                }
            }
        }
        public void ValidateQuantity(CartItem item)
        {
            if (item.Quantity <= 0)
            {
                throw new CartExceptions("Item quantity must be greater than zero.");
            }
        }
        public void ValidatePrice(CartItem item)
        {
            if (item.Price <= 0)
            {
                throw new CartExceptions("Item price must be greater than zero.");
            }
        }
    }
}