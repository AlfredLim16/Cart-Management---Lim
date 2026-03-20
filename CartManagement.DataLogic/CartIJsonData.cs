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
            return carts.FirstOrDefault(c => c.CartId == cartId);
        }
        public List<Cart> GetAll()
        {
            RetrieveDataFromJsonFile();
            return carts;
        }
        public void Update(Cart cart)
        {
            RetrieveDataFromJsonFile();
            var existing = carts.FirstOrDefault(c => c.CartId == cart.CartId);
            if (existing != null)
            {
                existing.Items = cart.Items;
                existing.Threshold = cart.Threshold;
                SaveDataToJsonFile();
            }
        }
        public void Delete(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart != null)
            {
                carts.Remove(cart);
                SaveDataToJsonFile();
            }
        }
        public void Clear(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart != null)
            {
                cart.Items.Clear();
                SaveDataToJsonFile();
            }
        }
        public void AddItem(Guid cartId, CartItem item)
        {
            RetrieveDataFromJsonFile();
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart != null)
            {
                cart.Items.Add(item);
                SaveDataToJsonFile();
            }
        }
        public void RemoveItem(Guid cartId, Guid cartItemId)
        {
            RetrieveDataFromJsonFile();
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
                if (item != null)
                {
                    cart.Items.Remove(item);
                    SaveDataToJsonFile();
                }
            }
        }
        public List<CartItem> GetItems(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            return cart?.Items ?? new List<CartItem>();
        }
        public int GetItemCount(Guid cartId)
        {
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            return cart?.Items.Count ?? 0;
        }
        public decimal GetTotal(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            return cart?.Items.Sum(i => i.Price * i.Quantity) ?? 0;
        }
        public bool ContainsItem(Guid cartId, Guid cartItemId)
        {
            RetrieveDataFromJsonFile();
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            return cart != null && cart.Items.Any(i => i.CartItemId == cartItemId);
        }
        public bool IsEmpty(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            return cart == null || !cart.Items.Any();
        }
        public byte GetThreshold(Guid cartId)
        {
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart != null)
            {
                int currentCount = cart.Items.Count;
                return (byte)Math.Max(0, cart.Threshold - currentCount);
            }
            return 0;
        }
        public void SetThreshold(Guid cartId, byte threshold)
        {
            RetrieveDataFromJsonFile();
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart != null)
            {
                cart.Threshold = threshold;
                SaveDataToJsonFile();
            }
        }
        public bool WithinThreshold(Guid cartId, CartItem item)
        {
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart != null)
            {
                int currentCount = cart.Items.Count;
                return currentCount + 1 <= cart.Threshold;
            }
            return false;
        }
        public List<CartItem> GetSelectedItems(Guid cartId, List<Guid> cartItemId)
        {
            RetrieveDataFromJsonFile();
            var cart = carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart == null) return new List<CartItem>();
            return cart.Items.Where(i => cartItemId.Contains(i.CartItemId)).ToList();
        }
        public decimal GetSelectedTotal(Guid cartId, List<Guid> cartItemId)
        {
            var selectedItems = GetSelectedItems(cartId, cartItemId);
            return selectedItems.Sum(i => i.Price * i.Quantity);
        }
    }
}