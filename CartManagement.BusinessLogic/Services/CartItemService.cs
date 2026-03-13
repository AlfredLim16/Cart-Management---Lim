using CartManagementDataLogic;
using CartManagementModels;
using System.Globalization;
namespace CartManagementBusinessLogic
{
    public class CartItemService
    {
        public List<CartItemLogic> cartItemLogic = new List<CartItemLogic>();
        public CartItemRules cartItemRules = new CartItemRules();
        public CultureInfo philippineCurrency = new CultureInfo("fil-PH");

        public CartItemService()
        {
            cartItemLogic.Add(new CartItemLogic());
            cartItemRules = new CartItemRules();
        }
        public decimal getSubtotal(CartItemModel item)
        {
            cartItemRules.validateCartItem(item);
            return cartItemLogic[0].computeSubtotal(item);
        }
        public void updateCartItemQuantity(CartItemModel item, int newQuantity)
        {
            if (!cartItemRules.validateMaxinumCartItemQuantity(newQuantity, 99))
            {
                Console.WriteLine(" Quantity exceeds maximum allowed per item.");
                return;
            }
            cartItemLogic[0].updateQuantity(item, newQuantity);
        }
        public void applyCartItemDiscount(CartItemModel item, decimal discountPercentage)
        {
            var newPrice = cartItemRules.applyCartItemDiscount(item, discountPercentage);
            item.Price = newPrice;
        }
        public CartItemModel cloneCartItem(CartItemModel item)
        {
            cartItemRules.validateCartItem(item);
            return cartItemLogic[0].CloneItem(item);
        }
        public void displayCartItem(CartItemModel item)
        {
            cartItemRules.validateCartItem(item);
            Console.WriteLine($" {item.Name} = Qty: {item.Quantity}, Price: {item.Price.ToString("C", philippineCurrency)}, Subtotal: {getSubtotal(item).ToString("C", philippineCurrency)}");
        }
    }
}