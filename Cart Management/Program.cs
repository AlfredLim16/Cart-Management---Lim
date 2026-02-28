namespace Cart_Management
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
    class inventoryManager
    {
        private List<Item> inventory = new List<Item>();
        public IReadOnlyList<Item> GetInventory() => inventory.AsReadOnly();

        public void addItem(string name, decimal price, int stock)
        {
            inventory.Add(new Item { name = name, price = price, stock = stock });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Item added!!.");
            Console.ResetColor();
        }
        public void Search(string keyword)
        {
            var results = inventory.FindAll(i => i.name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            if (results.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No items found.");
                Console.ResetColor();
                return;
            }
            Console.WriteLine("Search results:");
            foreach (var item in results)
                Console.WriteLine($"{item.name} | Price: {item.price} | Stock: {item.stock}");
        }
    }
    class cartManager
    {
        private List<cartItem> cart = new List<cartItem>();
        public IReadOnlyList<cartItem> GetCart() => cart.AsReadOnly();
        private Stack<actionHistory> undoStack = new Stack<actionHistory>();
        private Stack<actionHistory> redoStack = new Stack<actionHistory>();

        public void addToCart(Item item, int quantity)
        {
            if (cart.Count >= 10)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cart limit reached (10 items are max).");
                Console.ResetColor();
                return;
            }
            if (item.stock < quantity)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not enough stock.");
                Console.ResetColor();
                return;
            }

            item.stock -= quantity;
            var cartItem = new cartItem { product = item, quantity = quantity };
            cart.Add(cartItem);

            undoStack.Push(new actionHistory { actionType = "Add", affectedCartItem = cartItem });
            redoStack.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Added!");
            Console.ResetColor();

            if (item.stock <= 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Reorder Alert: {item.name} stock low ({item.stock} left).");
                Console.ResetColor();
            }
        }
        public void ViewCart(string sortBy = "name")
        {
            if (cart.Count == 0)
            {
                Console.WriteLine("Cart empty.");
                return;
            }

            IEnumerable<cartItem> sortedCart = cart;

            switch (sortBy.ToLower())
            {
                case "name":
                    sortedCart = cart.OrderBy(c => c.product.name);
                    break;
                case "price":
                    sortedCart = cart.OrderBy(c => c.product.price);
                    break;
                case "quantity":
                    sortedCart = cart.OrderByDescending(c => c.quantity);
                    break;
            }

            decimal total = 0;
            foreach (var c in sortedCart)
            {
                decimal sub = c.product.price * c.quantity;
                total += sub;
                Console.WriteLine($"{c.product.name} x{c.quantity} = {sub}");
                if (c.product.stock <= 2)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Reorder Alert: {c.product.name} only {c.product.stock} left!");
                    Console.ResetColor();
                }
            }
    internal class Program
    {
            static void Main(string[] args)
            {                    
            byte choice = 0;
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
    }
}
