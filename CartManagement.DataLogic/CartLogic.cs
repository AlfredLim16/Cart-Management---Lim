using CartManagementModels;
namespace CartManagementDataLogic
{
    public class CartLogic
    {
        public List<CartModel> cart = new List<CartModel>();

        public CartLogic()
        {
            /* Initialize cart list to store carts for different users and
               throw an exception if cart list is null to ensure that cart list is properly initialized before using it */
            cart = cart ?? throw new ArgumentNullException(nameof(cart));
        }
        public decimal computationOfTotal(Guid userId)
        {
            /* Find user cart and calculate a total price of items in cart by multiplying the price of each item with its quantity and
               sum up the total for all items in the cart and
               if user cart is not found, simply return 0 as the total price */
            var userCart = cart.FirstOrDefault(c => c.UserId == userId);
            if (userCart == null)
            {
                return 0;
            }
            return userCart.Items.Sum(item => item.Price * item.Quantity);
        }
        public void addItem(Guid userId, CartItemModel item)
        {
            /* Find the user's cart.
               If not found, create a new cart for the user.
               If an item with the same name exists, update its quantity.
               Otherwise, add the new item to the cart.*/
            var userCart = cart.FirstOrDefault(c => c.UserId == userId);
            if (userCart == null)
            {
                userCart = new CartModel
                {
                    CartId = Guid.NewGuid(),
                    UserId = userId
                };
                cart.Add(userCart);
            }
            var existingItem = userCart.Items.FirstOrDefault(i => i.Name == item.Name);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                userCart.Items.Add(item);
            }
        }
        public void removeItem(Guid userId, Guid productId)
        {
            /* Find the user's cart.
               not found, return immediately.
               Otherwise, remove all items matching the given productId. */
            var userCart = cart.FirstOrDefault(c => c.UserId == userId);
            if (userCart == null)
            {
                return;
            }
            userCart.Items.RemoveAll(i => i.ProductId == productId);
        }
    }
}