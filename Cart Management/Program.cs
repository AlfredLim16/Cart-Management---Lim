using CartManagementBusinessLogic.Managers;
using CartManagementBusinessLogic.Rules;
using CartManagementBusinessLogic.Services;
using CartManagementDataLogic;
using CartManagementModels;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CartManagementApp
{
    internal class Program
    {
        private static CartService? cartService;
        static Cart? cart;

        private static void Main(string[] args)
        {
            Setup();
            runMenu();
        }
        private static void Setup()
        {
            ICartDataLogic dataLogic = new InMemoryCartData();
            CartRules cartRules = new CartRules();
            CartManager cartManager = new CartManager(dataLogic, cartRules);
            cartService = new CartService(cartManager);

            cart = new Cart
            {
                CartId = Guid.NewGuid(),
                Items = new List<CartItem>(),
                Threshold = 100
            };
            cartService.Create(cart);
        }
        private static void runMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nCart Menu");
                Console.WriteLine("1. View Cart");
                Console.WriteLine("2. Add Item");
                Console.WriteLine("3. Remove Item");
                Console.WriteLine("4. Clear Cart");
                Console.WriteLine("5. Cart Summary");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        viewCart();
                        break;
                    case "2":
                        addItem();
                        break;
                    case "3":
                        removeItem();
                        break;
                    case "4":
                        clearCart();
                        break;
                    case "5":
                        showCartSummary();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }
        private static void viewCart()
        {
            var currentCart = cartService?.Get(cart?.CartId ?? Guid.Empty);
            Console.WriteLine($"\nCart ID: {currentCart?.CartId}");
            Console.WriteLine($"Threshold: {currentCart?.Threshold}");
            Console.WriteLine("Items:");
            foreach (var item in currentCart?.Items ?? new List<CartItem>())
            {
                Console.WriteLine($"ID: {item.CartItemId} | Product: {item.ProductName} | Qty: {item.Quantity} | Price: {item.Price.ToString("C")}");
            }
        }
        private static void clearCart()
        {
            cartService?.Clear(cart?.CartId ?? Guid.Empty);
            Console.WriteLine("Cart cleared!");
        }
        private static void addItem()
        {
            Console.Write("Enter product name: ");
            string name = Console.ReadLine() ?? "Unknown";
            Console.Write("Enter quantity: ");
            byte qty = byte.TryParse(Console.ReadLine(), out var q) ? q : (byte)1;
            Console.Write("Enter price: ");
            decimal price = decimal.TryParse(Console.ReadLine(), out var p) ? p : 0;

            CartItem newItem = new CartItem
            {
                CartItemId = Guid.NewGuid(),
                ProductName = name,
                Quantity = qty,
                Price = price
            };

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
        private static void removeItem()
        {
            var cartId = cart?.CartId ?? Guid.Empty;
            var items = cartService?.GetItems(cartId) ?? new List<CartItem>();

            if (items.Count == 0)
            {
                Console.WriteLine("\nCart is empty. Nothing to remove.");
                return;
            }
            Console.WriteLine("\nList of Items in Cart:");
            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.CartItemId} | Product: {item.ProductName} | Qty: {item.Quantity} | Price: {item.Price.ToString("C")}");
            }

            Console.Write("\nEnter CartItemId to remove: ");
            string idInput = Console.ReadLine() ?? "";
            if (Guid.TryParse(idInput, out var itemId))
            {
                cartService?.RemoveItem(cartId, itemId);
                Console.WriteLine("Item removed!");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
        private static void showCartSummary()
        {
            Console.WriteLine("\nCart Summary");
            var cartId = cart?.CartId ?? Guid.Empty;

            byte totalItems = (byte)(cartService?.GetItemCount(cartId) ?? 0);
            decimal cartTotal = cartService?.GetTotal(cartId) ?? 0;
            bool isEmpty = cartService?.IsEmpty(cartId) ?? true;
            byte remainingThreshold = cartService?.GetThreshold(cartId) ?? 0;

            Console.WriteLine($"Total Items       : {totalItems}");
            Console.WriteLine($"Cart Total        : {cartTotal.ToString("C")}");
            Console.WriteLine($"Cart Status       : {(isEmpty ? "Empty" : "Has Items")}");
            Console.WriteLine($"Remaining Capacity: {remainingThreshold}");
        }
    }
}