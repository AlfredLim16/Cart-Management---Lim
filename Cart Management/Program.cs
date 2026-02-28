using System;
using System.Collections.Generic;
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
            // Cart Limit = 10
            if (cart.Count >= 10)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cart limit reached (10 items are max).");
                Console.ResetColor();
                return;
            }
            // if adding but no stock
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
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Total: {total}");
            Console.ResetColor();
        }
        public void editCartItem(int index, int newQty)
        {
            if (index < 0 || index >= cart.Count)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid cart item index.");
                Console.ResetColor();
                return;
            }

            var selectedCartItem = cart[index];
            int currentQty = selectedCartItem.quantity;
            int diff = newQty - currentQty;

            if (diff > selectedCartItem.product.stock)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not enough stock.");
                Console.ResetColor();
                return;
            }

            selectedCartItem.product.stock -= diff;
            selectedCartItem.quantity = newQty;

            undoStack.Push(new actionHistory
            {
                actionType = "Edit",
                affectedCartItem = selectedCartItem,
                previousQuantity = currentQty
            });
            redoStack.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Cart updated.");
            Console.ResetColor();
        }
        public void removeCartItem(int index)
        {
            if (index < 0 || index >= cart.Count)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid cart item index.");
                Console.ResetColor();
                return;
            }

            var selectedCartItem = cart[index];
            selectedCartItem.product.stock += selectedCartItem.quantity;
            cart.RemoveAt(index);

            undoStack.Push(new actionHistory { actionType = "Remove", affectedCartItem = selectedCartItem });
            redoStack.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Item removed!");
            Console.ResetColor();
        }
        public void clearCart()
        {
            foreach (var c in cart)
            {
                c.product.stock += c.quantity;
            }
            cart.Clear();
            undoStack.Clear();
            redoStack.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Cart cleared!");
            Console.ResetColor();
        }
        public void undo()
        {
            if (undoStack.Count == 0)
            {
                Console.WriteLine("Nothing to undo.");
                return;
            }

            var lastAction = undoStack.Pop();
            redoStack.Push(lastAction);

            if (lastAction.actionType == "Add")
            {
                cart.Remove(lastAction.affectedCartItem);
                lastAction.affectedCartItem.product.stock += lastAction.affectedCartItem.quantity;
                Console.WriteLine("Undo: Add reverted.");
            }
            else if (lastAction.actionType == "Remove")
            {
                cart.Add(lastAction.affectedCartItem);
                lastAction.affectedCartItem.product.stock -= lastAction.affectedCartItem.quantity;
                Console.WriteLine("Undo: Remove reverted.");
            }
            else if (lastAction.actionType == "Edit")
            {
                int diff = lastAction.previousQuantity - lastAction.affectedCartItem.quantity;
                lastAction.affectedCartItem.product.stock += diff;
                lastAction.affectedCartItem.quantity = lastAction.previousQuantity;
                Console.WriteLine("Undo: Edit reverted.");
            }
        }
        public void redo()
        {
            if (redoStack.Count == 0)
            {
                Console.WriteLine("Nothing to redo.");
                return;
            }

            var lastAction = redoStack.Pop();
            undoStack.Push(lastAction);

            if (lastAction.actionType == "Add")
            {
                cart.Add(lastAction.affectedCartItem);
                lastAction.affectedCartItem.product.stock -= lastAction.affectedCartItem.quantity;
                Console.WriteLine("Redo: Add reapplied Sucessfully.");
            }
            else if (lastAction.actionType == "Remove")
            {
                cart.Remove(lastAction.affectedCartItem);
                lastAction.affectedCartItem.product.stock += lastAction.affectedCartItem.quantity;
                Console.WriteLine("Redo: Remove reapplied Successfuly.");
            }
            else if (lastAction.actionType == "Edit")
            {
                int currentQty = lastAction.affectedCartItem.quantity;
                int diff = currentQty - lastAction.previousQuantity;

                if (lastAction.affectedCartItem.product.stock >= diff)
                {
                    lastAction.affectedCartItem.product.stock -= diff;
                    lastAction.affectedCartItem.quantity = currentQty;
                    Console.WriteLine("Redo: Edit reapplied Successfully.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Redo failed: Not enough stock to reapply edit.");
                    Console.ResetColor();
                }
            }

        }
    }
    class consoleUserInterface
    {
        private readonly inventoryManager inventory;
        private readonly cartManager cart;

        public consoleUserInterface(inventoryManager inv, cartManager c)
        {
            inventory = inv;
            cart = c;
        }
        //a function for getting input in a data type Byte
        private byte getByteInput(string prompt, int min = byte.MinValue, int max = byte.MaxValue)
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out value) && value >= min && value <= max)
                    return (byte)value;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid input!!. Please enter a number between {min} and {max}.");
                Console.ResetColor();
            }
        }
        // a function for getting input in a data type Int
        private int getIntInput(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out value) && value >= min && value <= max)
                    return value;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid input!!. Please enter a number between {min} and {max}.");
                Console.ResetColor();
            }
        }
        // a function for getting input in a data type Decimal
        private decimal getDecimalInput(string prompt, decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
        {
            decimal value;
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (decimal.TryParse(input, out value) && value >= min && value <= max)
                    return value;
                Console.WriteLine($"Invalid input!!. Please enter a decimal number between {min} and {max}.");
            }
        }
        // a function for getting input in a data type String
        private string getStringInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();
                Console.WriteLine("Invalid input!!. Please enter a non-empty string.");
            }
        }
        public void run()
        {
            // changes: int earlier but now it's byte since choice is only for 0-9
            byte choice;
            do
            {
                displayMenu();
                choice = getChoice();
                handleChoice(choice);
            } while (choice != 0);
        }
        private void displayMenu()
        {
            Console.WriteLine("\n====================================");
            Console.WriteLine("Cart Management");
            Console.WriteLine("[1]. Add Item to Inventory");
            Console.WriteLine("[2]. Add Item to Cart");
            Console.WriteLine("[3]. View Cart");
            Console.WriteLine("[4]. Search Inventory");
            Console.WriteLine("[5]. Edit Cart Item");
            Console.WriteLine("[6]. Remove Cart Item");
            Console.WriteLine("[7]. Clear All Item in Cart");
            Console.WriteLine("[8]. Undo");
            Console.WriteLine("[9]. Redo");
            Console.WriteLine("[0]. Exit");
            Console.WriteLine("====================================\n");
        }
        private byte getChoice()
        {
            while (true)
            {
                // choice in 1-9
                Console.Write("Choose: ");
                string? input = Console.ReadLine();
                if (byte.TryParse(input, out byte choice) && choice >= 0 && choice <= 9)
                    return choice;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice. Please enter 1–9.");
                Console.ResetColor();
            }
        }
        private void handleChoice(byte choice)
        {
            switch (choice)
            {
                case 1:
                    addItemToInventory();
                    break;
                case 2:
                    addItemToCart();
                    break;
                case 3:
                    viewItemInCart();
                    break;
                case 4:
                    searchItemInInventory();
                    break;
                case 5:
                    editItemFromCart();
                    break;
                case 6:
                    removeItemFromCart();
                    break;
                case 7:
                    clearAllItemFromCart();
                    break;
                case 8:
                    undoAction();
                    break;
                case 9:
                    redoAction();
                    break;
                case 0:
                    exitAction();
                    break;
            }
        }
    }
    internal class Program
    {
            // after running the program it will create var inventory based on the class inventoryManager,
            // cart based on class cartManager, and console based on consoleUserInterface. it will run using run()
            static void Main(string[] args)
            {
                var inventory = new inventoryManager();
                var cart = new cartManager();
                var console = new consoleUserInterface(inventory, cart);
                console.run();
        }
    }
}