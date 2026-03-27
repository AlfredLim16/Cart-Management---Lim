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
            var existingCart = Get(cart.CartId);
            if(existingCart != null)
            {
                existingCart.Items = cart.Items;
                existingCart.Threshold = cart.Threshold;
                SaveDataToJsonFile();
            }
        }

        public void Delete(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var toRemove = Get(cartId);
            if(toRemove != null)
            {
                carts.Remove(toRemove);
                SaveDataToJsonFile();          
            }
        }

        public void Clear(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cartQuery = Get(cartId);
            if(cartQuery != null)
            {
                cartQuery.Items.Clear();
                SaveDataToJsonFile();
            }
        }

        public void AddItem(Guid cartId, CartItem item)
        {
            RetrieveDataFromJsonFile();
            var cart = Get(cartId);
            if(cart != null)
            {
                cart.Items.Add(item);
                SaveDataToJsonFile();
            }
        }

        public void RemoveItem(Guid cartId, Guid cartItemId)
        {
            RetrieveDataFromJsonFile();
            var cart = Get(cartId);
            if(cart != null)
            {
                var toRemove = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
                if(toRemove != null)
                {
                    cart.Items.Remove(toRemove);
                    SaveDataToJsonFile();
                }
            }
        }

        public List<CartItem> GetItems(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cart = Get(cartId);
            if(cart != null)
            {
                return cart.Items;
            }
            return new List<CartItem>();
        }

        public int GetItemCount(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cart = Get(cartId);
            if(cart != null)
            {
                return cart.Items.Count;
            }
            return 0;
        }

        public decimal GetTotal(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cart = Get(cartId);
            if(cart != null)
            {
                decimal total = 0;
                foreach(var cartItem in cart.Items)
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
            var cart = Get(cartId);
            if(cart != null)
            {
                foreach(var cartItem in cart.Items)
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
            var cart = Get(cartId);
            if(cart != null)
            {
                return cart.Items.Count == 0;
            }
            return true;
        }

        public byte GetThreshold(Guid cartId)
        {
            RetrieveDataFromJsonFile();
            var cart = Get(cartId);
            if(cart != null)
            {
                int currentCount = cart.Items.Count;
                return (byte)Math.Max(0, cart.Threshold - currentCount);
            }
            return 0;
        }

        public void SetThreshold(Guid cartId, short threshold)
        {
            RetrieveDataFromJsonFile();
            var cart = Get(cartId);
            if(cart != null)
            {
                cart.Threshold = threshold;
                SaveDataToJsonFile();
            }
        }

        public bool WithinThreshold(Guid cartId, CartItem item)
        {
            RetrieveDataFromJsonFile();
            var cart = Get(cartId);
            if(cart != null)
            {
                int currentCount = cart.Items.Count;
                return currentCount + 1 <= cart.Threshold;
            }
            return false;
        }

        public List<CartItem> GetSelectedItems(Guid cartId, List<Guid> cartItemIds)
        {
            RetrieveDataFromJsonFile();
            var cart = Get(cartId);
            if(cart != null)
            {
                List<CartItem> selectedCartItems = new List<CartItem>();
                foreach(var cartItem in cart.Items)
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
            foreach(var cartItem in selectedItems)
            {
                total += cartItem.Price * cartItem.Quantity;
            }
            return total;
        }

    }
}