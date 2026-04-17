using Cart_Management.ApplicationService;
using Cart_Management.DataService;
using Cart_Management.Core.Models;
using Cart_Management.Core.Validations;
using System;
using System.Collections.Generic;

namespace Cart_Management
{
    class Program
    {
        private static ICartApplicationService _cartService;
        private static Guid _currentUserId = Guid.Parse("A1B2C3D4-E5F6-4A5B-9C8D-E7F8A9B0C1D2");
        private static Guid _currentCartId;

        private static void Main(string[] args)
        {
            ICartDataService dataService = new InMemoryDataService();
            ICartValidation validation = new CartValidation();
            _cartService = new CartApplicationService(dataService, validation);

            Guid? existingCartId = _cartService.GetCartByUserId(_currentUserId);
            if (existingCartId != null)
            {
                _currentCartId = existingCartId.Value;
            }
            else
            {
                _currentCartId = _cartService.CreateCart(_currentUserId);
            }

            bool exit = false;
            while (!exit)
            {
                try
                {
                    exit = MainScreen();
                }
                catch (Exception ex)
                {
                    DisplayError(ex);
                }
            }
        }
        private static bool MainScreen()
        {
            Console.Clear();
            Console.WriteLine("\n Cart Management System");
            Console.WriteLine(" ==========================================");

            List<Product> products = _cartService.GetAllProducts();
            Console.WriteLine();
            Console.WriteLine(" Products:");
            Console.WriteLine(" {0,-4}{1,-20}{2,10}{3,7}", "#", "Name", "Price (PHP)", "Stock");
            Console.WriteLine(" ------------------------------------------");
            for (int productIndex = 0; productIndex < products.Count; productIndex++)
            {
                Console.WriteLine(" {0,-4}{1,-20}{2,10:F2}{3,7}",
                    productIndex + 1 + ".",
                    Truncate(products[productIndex].Name, 20),
                    products[productIndex].UnitPrice,
                    products[productIndex].Stock);
            }

            List<CartItem> cartItems = _cartService.GetCartItems(_currentCartId);
            Console.WriteLine();
            Console.WriteLine(" Cart ({0} item{1}):", cartItems.Count, cartItems.Count == 1 ? "" : "s");
            if (cartItems.Count == 0)
            {
                Console.WriteLine(" (empty)");
            }
            else
            {
                Console.WriteLine(" {0,-4}{1,-20}{2,5}{3,12}", "#", "Item", "Qty", "Subtotal");
                Console.WriteLine(" ------------------------------------------");
                for (int cartItemIndex = 0; cartItemIndex < cartItems.Count; cartItemIndex++)
                {
                    decimal subtotal = cartItems[cartItemIndex].Quantity * cartItems[cartItemIndex].UnitPrice;
                    Console.WriteLine(" {0,-4}{1,-20}{2,5}{3,12:F2}",
                        cartItemIndex + 1 + ".",
                        Truncate(cartItems[cartItemIndex].ProductName, 20),
                        cartItems[cartItemIndex].Quantity,
                        subtotal);
                }
                decimal cartTotal = _cartService.GetCartTotal(_currentCartId);
                Console.WriteLine(" ------------------------------------------");
                Console.WriteLine(" Total: {0,34:F2}", cartTotal);
            }

            List<Voucher> vouchers = _cartService.GetAllVouchers();
            Console.WriteLine();
            Console.WriteLine(" Vouchers:");
            if (vouchers.Count == 0)
            {
                Console.WriteLine(" (none available)");
            }
            else
            {
                Console.WriteLine(" {0,-20}{1,-11}{2,10}", "Code", "Type", "Discount");
                Console.WriteLine(" ------------------------------------------");
                foreach (Voucher voucher in vouchers)
                {
                    Console.WriteLine(" {0,-20}{1,-11}{2,10:F2}", voucher.Code, voucher.Type, voucher.DiscountAmount);
                }
            }

            Console.WriteLine();
            Console.WriteLine(" [1] Add to Cart        [4] Apply Voucher");
            Console.WriteLine(" [2] Update Quantity    [5] Checkout");
            Console.WriteLine(" [3] Remove Item        [0] Quit");
            Console.Write("\n  > ");

            char menuSelection = char.ToUpper(Console.ReadKey(false).KeyChar);
            Console.WriteLine();

            switch (menuSelection)
            {
                case '1':
                    AddToCart(products);
                    break;
                case '2':
                    UpdateQuantity(cartItems);
                    break;
                case '3':
                    RemoveItem(cartItems);
                    break;
                case '4':
                    ApplyVoucher();
                    break;
                case '5':
                    Checkout(cartItems);
                    break;
                case '0':
                    return true;
            }
            return false;
        }
        private static void AddToCart(List<Product> products)
        {
            if (products.Count == 0) return;
            Console.Write(" Product #: ");
            string productInput = Console.ReadLine();
            if (!int.TryParse(productInput, out int productNumber) || productNumber < 1 || productNumber > products.Count)
            {
                return;
            }

            Console.Write(" Quantity: ");
            string quantityInput = Console.ReadLine();
            if (!int.TryParse(quantityInput, out int quantity))
            {
                return;
            }
            _cartService.AddItem(_currentCartId, products[productNumber - 1].Id, quantity);
            Console.WriteLine(" Added.");
            Pause();
        }
        private static void UpdateQuantity(List<CartItem> cartItems)
        {
            if (cartItems.Count == 0)
            {
                Console.WriteLine(" Cart is empty.");
                Pause();
                return;
            }

            Console.Write(" Cart item #: ");
            string cartItemInput = Console.ReadLine();
            if (!int.TryParse(cartItemInput, out int cartItemNumber) || cartItemNumber < 1 || cartItemNumber > cartItems.Count)
            {
                return;
            }

            Console.Write(" New quantity: ");
            string quantityInput = Console.ReadLine();
            if (!int.TryParse(quantityInput, out int newQuantity))
            {
                return;
            }
            _cartService.UpdateItemQuantity(_currentCartId, cartItems[cartItemNumber - 1].ProductId, newQuantity);
            Console.WriteLine(" Updated.");
            Pause();
        }
        private static void RemoveItem(List<CartItem> cartItems)
        {
            if (cartItems.Count == 0)
            {
                Console.WriteLine(" Cart is empty.");
                Pause();
                return;
            }

            Console.Write(" Cart item # to remove: ");
            string cartItemInput = Console.ReadLine();
            if (!int.TryParse(cartItemInput, out int cartItemNumber) || cartItemNumber < 1 || cartItemNumber > cartItems.Count)
            {
                return;
            }

            Console.Write(" Confirm remove? (Y/N): ");
            if (char.ToUpper(Console.ReadKey().KeyChar) != 'Y')
            {
                Console.WriteLine();
                return;
            }

            Console.WriteLine();
            _cartService.RemoveItem(_currentCartId, cartItems[cartItemNumber - 1].ProductId);
            Console.WriteLine(" Removed.");
            Pause();
        }
        private static void ApplyVoucher()
        {
            Console.Write(" Voucher code: ");
            string voucherCode = Console.ReadLine();
            _cartService.ApplyVoucher(_currentCartId, voucherCode);
            Console.WriteLine(" Voucher applied.");
            Pause();
        }
        private static void Checkout(List<CartItem> cartItems)
        {
            if (cartItems.Count == 0)
            {
                Console.WriteLine(" Cart is empty.");
                Pause();
                return;
            }
            _cartService.UpdateCartItemPrices(_currentCartId);
            decimal orderTotal = _cartService.GetCartTotal(_currentCartId);

            Console.WriteLine();
            Console.WriteLine(" Checkout");
            Console.WriteLine(" [1] Standard  [2] Express");
            Console.Write(" Shipping: ");
            string shippingInput = Console.ReadLine();
            int shippingChoice = (shippingInput == "2") ? 2 : 1;
            string shippingMethod = (shippingChoice == 1) ? "Standard" : "Express";

            Console.WriteLine(" [1] Cash on Delivery  [2] Card");
            Console.Write(" Payment: ");
            string paymentInput = Console.ReadLine();
            int paymentChoice = (paymentInput == "2") ? 2 : 1;
            string paymentMethod = (paymentChoice == 1) ? "Cash on Delivery" : "Card";

            Console.WriteLine();
            Console.WriteLine(" Order Summary");
            List<CartItem> finalCartItems = _cartService.GetCartItems(_currentCartId);
            foreach (CartItem cartItem in finalCartItems)
            {
                Console.WriteLine(" {0,3}x {1,-14} {2,10:F2}", cartItem.Quantity, Truncate(cartItem.ProductName, 14), cartItem.Quantity * cartItem.UnitPrice);
            }
            Console.WriteLine(" -------------------------");
            List<Voucher> appliedVouchers = _cartService.GetCartVouchers(_currentCartId);
            if (appliedVouchers.Count > 0)
            {
                foreach (Voucher appliedVoucher in appliedVouchers)
                {
                    Console.WriteLine(" Voucher  : {0} (-{1:F2})", appliedVoucher.Code, appliedVoucher.DiscountAmount);
                }
            }
            else
            {
                Console.WriteLine(" Voucher  : (none)");
            }
            Console.WriteLine(" Shipping : {0}", shippingMethod);
            Console.WriteLine(" Payment  : {0}", paymentMethod);
            Console.WriteLine(" Total    : PHP {0:F2}", orderTotal);
            Console.WriteLine(" -------------------------");

            Console.Write(" Place order? (Y/N): ");
            if (char.ToUpper(Console.ReadKey().KeyChar) == 'Y')
            {
                Console.WriteLine();
                _cartService.CheckoutCart(_currentCartId);
                Console.WriteLine(" Order placed! New cart created.");
                _currentCartId = _cartService.CreateCart(_currentUserId);
            }
            else
            {
                Console.WriteLine("\n Checkout cancelled.");
            }
            Pause();
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 1) + ".";
        }
        private static void DisplayError(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(" {0}", ex.Message);
            Pause();
        }
        private static void Pause()
        {
            Console.Write("\n Press any key...");
            Console.ReadKey();
        }
    }
}