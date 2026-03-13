using CartManagementDataLogic;
using CartManagementModels;
using System.Globalization;
namespace CartManagementBusinessLogic
{
    public class CartService
    {
        public List<CartLogic> cartLogic = new List<CartLogic>();
        public CartRules cartRules = new CartRules();
        public CartItemRules cartItemRules = new CartItemRules();
        public CultureInfo philippineCurrency = new CultureInfo("fil-PH");
        
        public CartService()
        {
            cartLogic = new List<CartLogic>();
            cartRules = new CartRules();
        }
        public CartLogic getUserCartLogic(Guid userId)
        {
            /* Check if user already has a cart and return it, 
               otherwise create a new one to ensure that each user has their own cart to manage their cart operations */
            var userCartLogic = cartLogic.FirstOrDefault(c => c.cart.Any(cart => cart.UserId == userId));
            if (userCartLogic == null)
            {
                userCartLogic = new CartLogic();
                userCartLogic.cart.Add(new CartModel
                {
                    CartId = Guid.NewGuid(),
                    UserId = userId
                });
                cartLogic.Add(userCartLogic);
            }
            return userCartLogic;
        }
        public void addItemToCart(Guid userId, CartItemModel item)
        {
            cartRules.validateCart(new List<CartItemModel> { item });
            if (!cartItemRules.validateMaxinumCartItemQuantity(item.Quantity, 99))
            {
                throw new ArgumentException(" Quantity exceeds maximum allowed per item (max: 99).");
            }
            var userCartLogic = getUserCartLogic(userId);
            userCartLogic.addItem(userId, item);
        }
        public void removeItemFromCart(Guid userId, Guid productId)
        {
            var userCartLogic = getUserCartLogic(userId);
            userCartLogic.removeItem(userId, productId);
        }
        public decimal getCartTotal(Guid userId)
        {
            var userCartLogic =getUserCartLogic(userId);
            return userCartLogic.computationOfTotal(userId);
        }
        public void displayCart(Guid userId)
        {
            var userCartLogic = getUserCartLogic(userId);
            var userCart = userCartLogic.cart.Find(c => c.UserId == userId);
            if (userCart == null || userCart.Items.Count == 0)
            {
                Console.WriteLine("Cart is empty.\n");
                return;
            }
            
            Console.WriteLine(" Your Cart:");
            Console.WriteLine($"{" No.",-6} {"Name",-13} {"Qty",-5} {"Price",-10} {"Subtotal",-13}");
            for (int i = 0; i < userCart.Items.Count; i++)
            {
                var item = userCart.Items[i];
                Console.WriteLine($" {i + 1,-5} {item.Name,-13} {item.Quantity,-5} {item.Price.ToString("C", philippineCurrency),-10} {(item.Quantity * item.Price).ToString("C", philippineCurrency),-13}");
            }
            var total = getCartTotal(userId);
            Console.WriteLine(" " + new string('-', 47));
            Console.WriteLine($"{" ", -6} {"Total:",-17} {total.ToString("C", philippineCurrency)}\n");
        }
        public void checkout(Guid userId, decimal minimumOrderAmount = 500)
        {
            var total = getCartTotal(userId);
            if (!cartRules.validateMinimumOrder(total, minimumOrderAmount))
            {
                Console.WriteLine($" Order must be at least {minimumOrderAmount.ToString("C", philippineCurrency)}.\n");
                return;
            }
            Console.WriteLine($" Checkout successful! Total: {total.ToString("C", philippineCurrency)}\n");
            var userCartLogic = getUserCartLogic(userId);
            var userCart = userCartLogic.cart.Find(c => c.UserId == userId);
            userCart?.Items.Clear();
        }
    }
}