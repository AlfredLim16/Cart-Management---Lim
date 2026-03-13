using CartManagementBusinessLogic;
using CartManagementModels;
using System.Text;
namespace Cart_Management
{
    internal class Program
    {
        private static CartService cartService = new CartService();
        private static Guid userId = Guid.NewGuid();

        private static void checkout()
        {
            cartService.checkout(userId);
        }
        private static void removeCartItem()
        {
            var userCartLogic = cartService.getUserCartLogic(userId);
            var userCart = userCartLogic.cart.Find(c => c.UserId == userId);
            if (userCart == null || userCart.Items.Count == 0)
            {
                Console.WriteLine("Cart is empty.\n");
                return;
            }

            Console.Write("Enter item number to remove: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= userCart.Items.Count)
            {
                var item = userCart.Items[index - 1];
                cartService.removeItemFromCart(userId, item.ProductId);
                Console.WriteLine($"Item '{item.Name}' removed!\n");
            }
            else
            {
                Console.WriteLine("Invalid item number.\n");
            }
        }
        private static void viewCart()
        {
            cartService.displayCart(userId);
        }
        private static void addProductToCart()
        {
            try
            {
                Console.Write(" Product Name: ");
                string name = Console.ReadLine()!;
                Console.Write(" Quantity: ");
                int qty = int.Parse(Console.ReadLine()!);
                Console.Write(" Price: ");
                decimal price = decimal.Parse(Console.ReadLine()!);

                var item = new CartItemModel
                {
                    ProductId = Guid.NewGuid(),
                    Name = name,
                    Quantity = qty,
                    Price = price
                };

                cartService.addItemToCart(userId, item);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" Product {name} added to cart!\n");
                Console.ResetColor();
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" [!] Error: {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" [!] Unexpected error: {ex.Message}");
                Console.ResetColor();
            }
        }
        private static void handleChoice(byte choice)
        {
            switch (choice)
            {
                case 1:
                    addProductToCart();
                    break;
                case 2:
                    viewCart();
                    break;
                case 3:
                    removeCartItem();
                    break;
                case 4:
                    checkout();
                    break;
            }
        }
        private static byte getChoice()
        {
            while (true)
            {
                Console.Write("\n Choose: ");
                string? input = Console.ReadLine();
                if (byte.TryParse(input, out byte choice) && choice >= 0 && choice <= 4)
                    return choice;
                Console.WriteLine("Invalid choice. Please enter 0–4.");
            }
        }
        private static void displayMenu()
        {
            Console.WriteLine(" " + new string('=', 47));
            Console.WriteLine(" " + new string(' ', 12) + "CART MANAGEMENT MENU");
            Console.WriteLine(" " +new string('-', 47));
            Console.WriteLine(" [1]. Add Item to Cart");
            Console.WriteLine(" [2]. View Cart");
            Console.WriteLine(" [3]. Remove Cart Item");
            Console.WriteLine(" [4]. Checkout");
            Console.WriteLine(" [0]. Exit");
            Console.WriteLine(" " + new string('=', 47));
        }
        private static void runProgram()
        {
            byte choice;
            do
            {
                displayMenu();
                choice = getChoice();
                handleChoice(choice);
            } while (choice != 0);
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            runProgram();
        }
    }
}
// NEXT TIME: I will Separate the exceptions
// NEXT TIME: I will add update item quantity functionality