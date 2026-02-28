namespace Cart_Management
{
    internal class Program
    {
        class Item
        {
            public required string name
            {
                get;
                set;
            }
            public decimal price
            {
                get;
                set;
            }
            public int stock
            {
                get;
                set;
            }
        }
        class cartItem
        {
            public required Item product
            {
                get;
                set;
            }
            public int quantity
            {
                get;
                set;
            }
        }
        class actionHistory
        {
            public required string actionType
            {
                get;
                set;
            }
            public required cartItem affectedCartItem
            {
                get;
                set;
            }
            public int previousQuantity
            {
                get;
                set;
            }
        }
        static void Main(string[] args)
        {                    
            int choice = 0;
            while(choice != 3)
            {
                switch (choice)
                {
                    case 0:
                        addToCart();
                    break;
                    case 1:
                        viewCart();
                    break;
                    case 2:
                        Console.WriteLine("");
                    break;
                    case 3:
                        Console.WriteLine("Exit");
                    break;
                }
            }
        }

        static void addToCart()
        {
            string[] items = new string[3];
            string[] prices = new string[3];
            items[0] = "1. Apple";
            items[1] = "2. Banana";
            items[2] = "3. Watermelon";
            prices[0] = "1. 50.00";
            prices[1] = "2. 30.00";
            prices[2] = "3. 100.00";

            foreach (string item in items)
            {
                Console.WriteLine(item);
            }

            Console.Write("Select an Item: ");
            string selectItem = Console.ReadLine();

            string userChoice = "yes";

            Console.WriteLine("Do you want to add this item into cart? (yes/no): ");
            userChoice = Console.ReadLine();

            
            if (userChoice == "yes")
            {
                Console.WriteLine("Item added to cart!!");
            }
            else
            {
                Console.WriteLine("Item failed to add to cart!!");
            }
        }

        static void viewCart()
        {

        }
    }
}
