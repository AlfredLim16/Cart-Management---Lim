using Cart_Management.Core.Enums;
using Cart_Management.Core.Exceptions;
using Cart_Management.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Cart_Management.DataService
{
    public class JsonFileDataService : ICartDataService
    {
        private List<Cart> dummyCarts = new List<Cart>();
        private List<Product> dummyProducts = new List<Product>();
        private List<Voucher> dummyVouchers = new List<Voucher>();
        private List<CartVoucher> dummyCartVouchers = new List<CartVoucher>();
        
        private readonly string cartsJsonFileName;
        private readonly string productsJsonFileName;
        private readonly string vouchersJsonFileName;
        private readonly string cartVouchersJsonFileName;

        public JsonFileDataService()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            cartsJsonFileName = Path.Combine(baseDir, "Carts.json");
            productsJsonFileName = Path.Combine(baseDir, "Products.json");
            vouchersJsonFileName = Path.Combine(baseDir, "Vouchers.json");
            cartVouchersJsonFileName = Path.Combine(baseDir, "CartVouchers.json");

            PopulateInitialData();
        }

        private void PopulateInitialData()
        {
            RetrieveCartsFromJsonFile();
            RetrieveProductsFromJsonFile();
            RetrieveVouchersFromJsonFile();
            RetrieveCartVouchersFromJsonFile();

            if (dummyProducts.Count == 0)
            {
                PopulateDefaultProducts();
            }

            if (dummyVouchers.Count == 0)
            {
                PopulateDefaultVouchers();
            }

            if (dummyCarts.Count == 0)
            {
                PopulateDefaultCarts();
            }
        }
        private void PopulateDefaultCarts()
        {
            dummyCarts.Add(new Cart
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = Guid.NewGuid(), ProductName = "Sample Item 1", Quantity = 2, UnitPrice = 10.99m },
                    new CartItem { ProductId = Guid.NewGuid(), ProductName = "Sample Item 2", Quantity = 1, UnitPrice = 5.49m  }
                }
            });
            SaveCartsToJsonFile();
        }
        private void PopulateDefaultProducts()
        {
            Guid seller1 = Guid.NewGuid();
            Guid seller2 = Guid.NewGuid();
            dummyProducts = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Laptop",     UnitPrice = 45000m, Stock = 10, SellerId = seller1 },
                new Product { Id = Guid.NewGuid(), Name = "Smartphone", UnitPrice = 12000m, Stock = 20, SellerId = seller2 },
                new Product { Id = Guid.NewGuid(), Name = "Headphones", UnitPrice = 350m,   Stock = 30, SellerId = Guid.NewGuid() },
                new Product { Id = Guid.NewGuid(), Name = "Shoes",      UnitPrice = 200m,   Stock = 15, SellerId = Guid.NewGuid() },
                new Product { Id = Guid.NewGuid(), Name = "Backpack",   UnitPrice = 300m,   Stock = 25, SellerId = Guid.NewGuid() }
            };
            SaveProductsToJsonFile();
        }

        private void PopulateDefaultVouchers()
        {
            Guid seller2 = dummyProducts.FirstOrDefault()?.SellerId ?? Guid.NewGuid();
            dummyVouchers = new List<Voucher>
            {
                new Voucher { Id = Guid.NewGuid(), Code = "WELCOME100", Type = VoucherType.Platform, DiscountAmount = 100m },
                new Voucher { Id = Guid.NewGuid(), Code = "SELLER50",   Type = VoucherType.Seller,   DiscountAmount = 50m, SellerId = seller2 }
            };
            SaveVouchersToJsonFile();
        }

        private void SaveCartsToJsonFile()
        {
            SaveToFile(cartsJsonFileName, dummyCarts);
        }
        private void SaveProductsToJsonFile()
        {
            SaveToFile(productsJsonFileName, dummyProducts);
        }
        private void SaveVouchersToJsonFile()
        {
            SaveToFile(vouchersJsonFileName, dummyVouchers);
        }
        private void SaveCartVouchersToJsonFile()
        {
            SaveToFile(cartVouchersJsonFileName, dummyCartVouchers);
        }

        private void SaveToFile<SaveClass>(string fileName, SaveClass data)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(fileName, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to {fileName}: {ex.Message}");
            }
        }

        private void RetrieveCartsFromJsonFile()
        {
            dummyCarts = RetrieveFromFile<List<Cart>>(cartsJsonFileName) ?? new List<Cart>();
        }
        private void RetrieveProductsFromJsonFile()
        {
            dummyProducts = RetrieveFromFile<List<Product>>(productsJsonFileName) ?? new List<Product>();
        }
        private void RetrieveVouchersFromJsonFile()
        {
            dummyVouchers = RetrieveFromFile<List<Voucher>>(vouchersJsonFileName) ?? new List<Voucher>();
        }
        private void RetrieveCartVouchersFromJsonFile()
        {
            dummyCartVouchers = RetrieveFromFile<List<CartVoucher>>(cartVouchersJsonFileName) ?? new List<CartVoucher>();
        }

        private RetrieveClass RetrieveFromFile<RetrieveClass>(string fileName)
        {
            if (!File.Exists(fileName)) return default;

            try
            {
                var jsonString = File.ReadAllText(fileName);
                if (string.IsNullOrWhiteSpace(jsonString)) return default;
                
                return JsonSerializer.Deserialize<RetrieveClass>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from {fileName}: {ex.Message}");
                return default;
            }
        }

        public Cart CreateCart(Guid userId)
        {
            RetrieveCartsFromJsonFile();
            var cart = new Cart { Id = Guid.NewGuid(), UserId = userId, Items = new List<CartItem>() };
            dummyCarts.Add(cart);
            SaveCartsToJsonFile();
            return cart;
        }
        public Cart GetCart(Guid cartId)
        {
            RetrieveCartsFromJsonFile();
            return dummyCarts.FirstOrDefault(c => c.Id == cartId);
        }
        public Cart GetCartByUserId(Guid userId)
        {
            RetrieveCartsFromJsonFile();
            return dummyCarts.FirstOrDefault(c => c.UserId == userId);
        }
        public void ClearCart(Guid cartId)
        {
            RetrieveCartsFromJsonFile();
            var cart = dummyCarts.FirstOrDefault(c => c.Id == cartId);
            if (cart != null)
            {
                cart.Items.Clear();
                SaveCartsToJsonFile();
            }
        }
        public decimal GetCartTotal(Guid cartId)
        {
            var items = GetCartItems(cartId);
            return items.Sum(i => i.Quantity * i.UnitPrice);
        }

        public List<CartItem> GetCartItems(Guid cartId)
        {
            RetrieveCartsFromJsonFile();
            return dummyCarts.FirstOrDefault(c => c.Id == cartId)?.Items ?? new List<CartItem>();
        }
        public void AddItem(Guid cartId, CartItem item)
        {
            RetrieveCartsFromJsonFile();
            var cart = dummyCarts.FirstOrDefault(c => c.Id == cartId);
            if (cart == null)
            {
                throw new DataException($"Cart with id '{cartId}' not found.");
            }
            cart.Items.Add(item);
            SaveCartsToJsonFile();
        }
        public void UpdateItem(Guid cartId, CartItem item)
        {
            RetrieveCartsFromJsonFile();
            var cart = dummyCarts.FirstOrDefault(c => c.Id == cartId);
            var existing = cart?.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity = item.Quantity;
                existing.UnitPrice = item.UnitPrice;
                existing.Status = item.Status;
                SaveCartsToJsonFile();
            }
        }
        public void RemoveItem(Guid cartId, Guid productId)
        {
            RetrieveCartsFromJsonFile();
            var cart = dummyCarts.FirstOrDefault(c => c.Id == cartId);
            var existing = cart?.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
            {
                cart.Items.Remove(existing);
                SaveCartsToJsonFile();
            }
        }
        public void UpdateCartItemStatus(Guid cartId, Guid productId, CartItemStatus status)
        {
            RetrieveCartsFromJsonFile();
            var cart = dummyCarts.FirstOrDefault(c => c.Id == cartId);
            var item = cart?.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Status = status;
                SaveCartsToJsonFile();
            }
        }

        public Product GetProduct(Guid productId)
        {
            RetrieveProductsFromJsonFile();
            return dummyProducts.FirstOrDefault(p => p.Id == productId);
        }
        public List<Product> GetAllProducts()
        {
            RetrieveProductsFromJsonFile();
            return dummyProducts;
        }
        public void UpdateStock(Guid productId, int quantityChange)
        {
            RetrieveProductsFromJsonFile();
            var product = dummyProducts.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                throw new DataException($"Product with id '{productId}' not found.");
            }
            product.Stock += quantityChange;
            SaveProductsToJsonFile();
        }
        
        public List<Voucher> GetAllVouchers()
        {
            RetrieveVouchersFromJsonFile();
            return dummyVouchers;
        }
        public Voucher GetVoucherByCode(string code)
        {
            RetrieveVouchersFromJsonFile();
            return dummyVouchers.FirstOrDefault(v => v.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
        public void ApplyVoucherToCart(Guid cartId, Guid voucherId)
        {
            RetrieveCartVouchersFromJsonFile();
            if (!dummyCartVouchers.Any(cv => cv.CartId == cartId && cv.VoucherId == voucherId))
            {
                dummyCartVouchers.Add(new CartVoucher { CartId = cartId, VoucherId = voucherId });
                SaveCartVouchersToJsonFile();
            }
        }
        public List<Voucher> GetCartVouchers(Guid cartId)
        {
            RetrieveCartVouchersFromJsonFile();
            RetrieveVouchersFromJsonFile();
            var voucherIds = dummyCartVouchers.Where(cv => cv.CartId == cartId).Select(cv => cv.VoucherId).ToList();
            return dummyVouchers.Where(v => voucherIds.Contains(v.Id)).ToList();
        }
    }
}
