using CartManagementBusinessLogic.Managers;
using CartManagementBusinessLogic.Rules;
using CartManagementBusinessLogic.Services;
using CartManagementDataLogic;
using CartManagementModels;
using System;
using System.Collections.Generic;

namespace CartManagementApp
{
    internal class Program
    {
        private static CartService? cartService;
        static Cart? cart;
        
        private static void Main(string[] args)
        {
            Setup();
            Run();
        }
        private static void Setup()
        {
            ICartDataLogic dataLogic = new CartDBData();
            CartRules cartRules = new CartRules();
            CartManager cartManager = new CartManager(dataLogic, cartRules);
            cartService = new CartService(cartManager);
            cart = cartService.GetAll().FirstOrDefault();
        }
        private static void Run()
        {
            bool exit = false;
            while (!exit)
            {
                Menu();
                string choice = Console.ReadLine() ?? "";
                Choices(choice, ref exit);
            }
        }
        private static void Menu()
        {
            Console.WriteLine("\nCart Menu");
            Console.WriteLine("1. View Cart");
            Console.WriteLine("2. Add Item");
            Console.WriteLine("3. Remove Item");
            Console.WriteLine("4. Clear Cart");
            Console.WriteLine("5. Cart Summary");
            Console.WriteLine("6. Checkout");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option(0 - 6): ");
        }
        private static void Choices(string choice, ref bool exit)
        {
            switch (choice)
            {
                case "0":
                    exit = true;
                    Console.WriteLine("Program Exit...");
                    break;
                case "1":
                    ViewCart();
                    break;
                case "2":
                    AddItem();
                    break;
                case "3":
                    RemoveItem();
                    break;
                case "4":
                    ClearCart();
                    break;
                case "5":
                    CartSummary();
                    break;
                case "6":
                    CheckoutItems();
                    break;
                default:
                    Console.WriteLine("Invalid choice, try again.");
                    break;
            }
        }
        
        private static void ViewCart()
        {
            var currentCart = cartService?.Get(cart?.CartId ?? Guid.Empty);
            ShowCart(currentCart);
        }  
        private static void ClearCart()
        {
            cartService?.Clear(cart?.CartId ?? Guid.Empty);
            Console.WriteLine("Cart cleared!");
        }
        private static void AddItem()
        {
            CartItem newItem = GetItem();
            if (cartService?.WithinThreshold(cart?.CartId ?? Guid.Empty, newItem) == true)
            {
                cartService?.AddItem(cart?.CartId ?? Guid.Empty, newItem);
                Console.WriteLine("Item added!");
            }
            else
            {
                Console.WriteLine("Cannot add item: threshold exceeded!");
            }
        }
        private static void RemoveItem()
        {
            var cartId = cart?.CartId ?? Guid.Empty;
            var items = cartService?.GetItems(cartId) ?? new List<CartItem>();
            if (CheckEmpty(items))
            {
                return;
            }
            ShowRemoval(items);
            Guid? itemId = GetItemId();
            if (itemId.HasValue)
            {
                cartService?.RemoveItem(cartId, itemId.Value);
                Console.WriteLine("Item removed!");
            }
        }
        private static void CartSummary()
        {
            var cartId = cart?.CartId ?? Guid.Empty;
            ShowSummary(cartId);
        }
        private static void CheckoutItems()
        {
            var cartId = cart?.CartId ?? Guid.Empty;
            var items = cartService?.GetItems(cartId) ?? new List<CartItem>();
            if (CheckCheckout(items))
            {
                return;
            }
            ShowItem(items);
            var selectedIds = GetSelected(items);
            ShowSelected(cartId, selectedIds);
        }

        private static void ShowCart(Cart? currentCart)
        {
            Console.WriteLine($"\nCart ID: {currentCart?.CartId}");
            Console.WriteLine($"Threshold: {currentCart?.Threshold}");
            Console.WriteLine("Items:");
            Console.WriteLine($"{"Cart Item ID",-38} {"Product",-15} {"Qty",-5} {"Price",-10}");
            Console.WriteLine(new string('-', 70));
            foreach (var item in currentCart?.Items ?? new List<CartItem>())
            {
                Console.WriteLine($"{item.CartItemId,-38} {item.ProductName,-15} {item.Quantity,-5} {item.Price.ToString("C"),-10}");
            }
        }
        private static CartItem GetItem()
        {
            Console.Write("Enter product name: ");
            string name = Console.ReadLine() ?? "Unknown";
            Console.Write("Enter quantity: ");
            byte qty = byte.TryParse(Console.ReadLine(), out var q) ? q : (byte)1;
            Console.Write("Enter price: ");
            decimal price = decimal.TryParse(Console.ReadLine(), out var p) ? p : 0;

            return new CartItem
            {
                CartItemId = Guid.NewGuid(),
                ProductName = name,
                Quantity = qty,
                Price = price
            };
        }
        private static bool CheckEmpty(List<CartItem> items)
        {
            if (items.Count == 0)
            {
                Console.WriteLine("\nCart is empty. Nothing to remove.");
                return true;
            }
            return false;
        }
        private static void ShowRemoval(List<CartItem> items)
        {
            Console.WriteLine("\nList of Items in Cart:");
            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.CartItemId} | Product: {item.ProductName} | Qty: {item.Quantity} | Price: {item.Price.ToString("C")}");
            }
        }
        private static Guid? GetItemId()
        {
            Console.Write("\nEnter Cart Item ID to remove: ");
            string idInput = Console.ReadLine() ?? "";
            if (Guid.TryParse(idInput, out var itemId))
            {
                return itemId;
            }
            Console.WriteLine("Invalid ID format.");
            return null;
        }
        private static void ShowSummary(Guid cartId)
        {
            Console.WriteLine("\nCart Summary");

            byte totalItems = (byte)(cartService?.GetItemCount(cartId) ?? 0);
            decimal cartTotal = cartService?.GetTotal(cartId) ?? 0;
            bool isEmpty = cartService?.IsEmpty(cartId) ?? true;
            byte remainingThreshold = cartService?.GetThreshold(cartId) ?? 0;

            Console.WriteLine($"Total Items       : {totalItems}");
            Console.WriteLine($"Cart Total        : {cartTotal.ToString("C")}");
            Console.WriteLine($"Cart Status       : {(isEmpty ? "Empty" : "Has Items")}");
            Console.WriteLine($"Remaining Capacity: {remainingThreshold}");
        }
        private static bool CheckCheckout(List<CartItem> items)
        {
            if (items.Count == 0)
            {
                Console.WriteLine("\nCart is empty. Nothing to checkout.");
                return true;
            }
            return false;
        }
        private static void ShowItem(List<CartItem> items)
        {
            Console.WriteLine("\nList of Items in Cart:");
            Console.WriteLine($"{"No.",-5} {"Product",-15} {"Qty",-5} {"Price",-10}");
            Console.WriteLine(new string('-', 40));
            for (byte i = 0; i < items.Count; i++)
            {
                var item = items[i];
                Console.WriteLine($"{i + 1,-5} {item.ProductName,-15} {item.Quantity,-5} {item.Price.ToString("C"),-10}");
            }
        }
        private static List<Guid> GetSelected(List<CartItem> items)
        {
            Console.Write("\nEnter the numbers of the items you want to checkout (e.g. 1 2 3): ");
            string input = Console.ReadLine() ?? "";
            var numberStrings = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            List<Guid> selectedIds = new List<Guid>();
            foreach (var numStr in numberStrings)
            {
                if (byte.TryParse(numStr.Trim(), out byte num) && num > 0 && num <= items.Count)
                {
                    selectedIds.Add(items[num - 1].CartItemId);
                }
            }
            return selectedIds;
        }
        private static void ShowSelected(Guid cartId, List<Guid> selectedIds)
        {
            var selectedItems = cartService?.GetSelectedItems(cartId, selectedIds) ?? new List<CartItem>();
            decimal selectedTotal = cartService?.GetSelectedTotal(cartId, selectedIds) ?? 0;

            Console.WriteLine("\nSelected Items:");
            Console.WriteLine($"{"Product",-15} {"Qty",-5} {"Price",-10}");
            Console.WriteLine(new string('-', 35));

            foreach (var item in selectedItems)
            {
                Console.WriteLine($"{item.ProductName,-15} {item.Quantity,-5} {item.Price.ToString("C"),-10}");
            }

            Console.WriteLine(new string('-', 35));
            Console.WriteLine($"Total: {selectedTotal.ToString("C")}");
        }

    }
}