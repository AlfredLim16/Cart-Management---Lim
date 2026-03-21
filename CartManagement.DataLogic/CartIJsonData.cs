using CartManagementModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CartManagementDataLogic
{
    public class CartJsonData : ICartDataLogic
    {
        private List<Cart> carts = new List<Cart>();
        private string _jsonFileName;
        public CartJsonData()
        {
            _jsonFileName = $"{AppDomain.CurrentDomain.BaseDirectory}/Carts.json";
            PopulateJsonFile();
        }
        private void PopulateJsonFile()
        {
            RetrieveDataFromJsonFile();
            if (carts.Count <= 0)
            {
                carts.Add(new Cart
                {
                    CartId = Guid.NewGuid(),
                    Items = new List<CartItem>
                    {
                        new CartItem { CartItemId = Guid.NewGuid(), ProductName = "Laptop", Quantity = 1, Price = 45000m },
                        new CartItem { CartItemId = Guid.NewGuid(), ProductName = "Mouse", Quantity = 2, Price = 500m },
                        new CartItem { CartItemId = Guid.NewGuid(), ProductName = "Apple", Quantity = 50, Price = 50m }
                    },
                    Threshold = 100
                });
                SaveDataToJsonFile();
            }
        }
        private void SaveDataToJsonFile()
        {
            using (var outputStream = File.Create(_jsonFileName))
            {
                JsonSerializer.Serialize<List<Cart>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions { SkipValidation = true, Indented = true }), carts);
            }
        }
        private void RetrieveDataFromJsonFile()
        {
            if (!File.Exists(_jsonFileName))
            {
                carts = new List<Cart>();
                return;
            }
            using (var jsonFileReader = File.OpenText(this._jsonFileName))
            {
                this.carts = JsonSerializer.Deserialize<List<Cart>>(jsonFileReader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.ToList() ?? new List<Cart>();
            }
        }
        public Cart Create(Cart cart)
        {
            carts.Add(cart);
            SaveDataToJsonFile();
            return cart;
        }
        public Cart? Get(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cartQuery = from cart in carts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                return cart;
            }
            return null;
        }
        public List<Cart> GetAll()
        {
            RetrieveDataFromJsonFile();
            return carts;
        }
        public void Update(Cart cart)
        {
            RetrieveDataFromJsonFile();
            var existingQuery = from existing in carts where existing.CartId == cart.CartId select existing;
            foreach (var existing in existingQuery)
            {
                existing.Items = cart.Items;
                existing.Threshold = cart.Threshold;
                SaveDataToJsonFile();
                break;
            }
        }
        public void Delete(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cartQuery = from cart in carts where cart.CartId == cartId select cart;
            Cart? toRemove = null;
            foreach (var cart in cartQuery)
            {
                toRemove = cart;
                break;
            }
            if (toRemove != null)
            {
                carts.Remove(toRemove);
                SaveDataToJsonFile();
            }
        }
        public void Clear(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cartQuery = from cart in carts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                cart.Items.Clear();
                SaveDataToJsonFile();
                break;
            }
        }
        public void AddItem(Guid cartId, CartItem item)
        {
            RetrieveDataFromJsonFile();
            var cartQuery = from cart in carts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                cart.Items.Add(item);
                SaveDataToJsonFile();
                break;
            }
        }
        public void RemoveItem(Guid cartId, Guid cartItemId)
        {
            RetrieveDataFromJsonFile();
            var cartQuery = from cart in carts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                var cartItemQuery = from cartItem in cart.Items where cartItem.CartItemId == cartItemId select cartItem;
                CartItem? toRemove = null;
                foreach (var cartItem in cartItemQuery)
                {
                    toRemove = cartItem;
                    break;
                }
                if (toRemove != null)
                {
                    cart.Items.Remove(toRemove);
                    SaveDataToJsonFile();
                }
                break;
            }
        }
        public List<CartItem> GetItems(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cartItemsQuery = from cart in carts where cart.CartId == cartId select cart.Items;
            foreach (var cartItems in cartItemsQuery)
            {
                return cartItems;
            }
            return new List<CartItem>();
        }
        public int GetItemCount(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cartItemCountQuery = from cart in carts where cart.CartId == cartId select cart.Items.Count;
            foreach (var itemCount in cartItemCountQuery)
            {
                return itemCount;
            }
            return 0;
        }
        public decimal GetTotal(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cartItemsQuery = from cart in carts where cart.CartId == cartId select cart.Items;
            foreach (var cartItems in cartItemsQuery)
            {
                decimal total = 0;
                foreach (var cartItem in cartItems)
                {
                    total += cartItem.Price * cartItem.Quantity;
                }
                return total;
            }
            return 0;
        }
        public bool ContainsItem(Guid cartId, Guid cartItemId)
        {
            RetrieveDataFromJsonFile();
            var cartItemsQuery = from cart in carts where cart.CartId == cartId select cart.Items;
            foreach (var cartItems in cartItemsQuery)
            {
                foreach (var cartItem in cartItems)
                {
                    if (cartItem.CartItemId == cartItemId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsEmpty(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cartItemsQuery = from cart in carts where cart.CartId == cartId select cart.Items;
            foreach (var cartItems in cartItemsQuery)
            {
                return cartItems.Count == 0;
            }
            return true;
        }
        public byte GetThreshold(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cartQuery = from cart in carts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                int currentCount = cart.Items.Count;
                return (byte)Math.Max(0, cart.Threshold - currentCount);
            }
            return 0;
        }
        public void SetThreshold(Guid cartId, byte threshold)
        {
            RetrieveDataFromJsonFile();
            var cartQuery = from cart in carts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                cart.Threshold = threshold;
                SaveDataToJsonFile();
                break;
            }
        }
        public bool WithinThreshold(Guid cartId, CartItem item)
        {
            RetrieveDataFromJsonFile();
            var cartQuery = from cart in carts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                int currentCount = cart.Items.Count;
                return currentCount + 1 <= cart.Threshold;
            }
            return false;
        }
        public List<CartItem> GetSelectedItems(Guid cartId, List<Guid> cartItemIds)
        {
            RetrieveDataFromJsonFile();
            var cartItemsQuery = from cart in carts where cart.CartId == cartId select cart.Items;
            foreach (var cartItems in cartItemsQuery)
            {
                List<CartItem> selectedCartItems = new List<CartItem>();
                foreach (var cartItem in cartItems)
                {
                    if (cartItemIds.Contains(cartItem.CartItemId))
                    {
                        selectedCartItems.Add(cartItem);
                    }
                }
                return selectedCartItems;
            }
            return new List<CartItem>();
        }
        public decimal GetSelectedTotal(Guid cartId, List<Guid> cartItemIds)
        {
            var selectedItems = GetSelectedItems(cartId, cartItemIds);
            decimal total = 0;
            foreach (var cartItem in selectedItems)
            {
                total += cartItem.Price * cartItem.Quantity;
            }
            return total;
        }
    }
}