using Cart_Management.Core.Enums;
using Cart_Management.Core.Exceptions;
using Cart_Management.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cart_Management.DataService
{
    public class InMemoryDataService : ICartDataService
    {
        private List<Cart> dummyCarts = new List<Cart>();
        private List<Product> dummyProducts = new List<Product>();
        private List<Voucher> dummyVouchers = new List<Voucher>();
        private List<CartVoucher> dummyCartVouchers = new List<CartVoucher>();

        public InMemoryDataService()
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
            
            dummyVouchers = new List<Voucher>
            {
                new Voucher { Id = Guid.NewGuid(), Code = "WELCOME100", Type = VoucherType.Platform, DiscountAmount = 100m },
                new Voucher { Id = Guid.NewGuid(), Code = "SELLER50", Type = VoucherType.Seller, DiscountAmount = 50m, SellerId = seller1 }
            };
        }
        
        public Cart CreateCart(Guid userId)
        {
            var cart = new Cart { Id = Guid.NewGuid(), UserId = userId, Items = new List<CartItem>() };
            dummyCarts.Add(cart);
            return cart;
        }
        public Cart GetCart(Guid cartId)
        {
            return dummyCarts.FirstOrDefault(c => c.Id == cartId);
        }
        public Cart GetCartByUserId(Guid userId)
        {
            return dummyCarts.FirstOrDefault(c => c.UserId == userId);
        }
        public void ClearCart(Guid cartId)
        {
            var cart = GetCart(cartId);
            if (cart != null)
            {
                cart.Items.Clear();
            }
        }
        public decimal GetCartTotal(Guid cartId)
        {
            var items = GetCartItems(cartId);
            return items.Sum(i => i.Quantity * i.UnitPrice);
        }

        public List<CartItem> GetCartItems(Guid cartId)
        {
            return dummyCarts.FirstOrDefault(c => c.Id == cartId)?.Items ?? new List<CartItem>();
        }
        public void AddItem(Guid cartId, CartItem item)
        {
            var cart = GetCart(cartId);
            if (cart == null)
            {
                throw new DataException($"Cart with id '{cartId}' not found.");
            }
            cart.Items.Add(item);
        }
        public void UpdateItem(Guid cartId, CartItem item)
        {
            var cart = GetCart(cartId);
            var existing = cart?.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity = item.Quantity;
                existing.UnitPrice = item.UnitPrice;
                existing.Status = item.Status;
            }
        }
        public void RemoveItem(Guid cartId, Guid productId)
        {
            var cart = GetCart(cartId);
            var existing = cart?.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
            {
                cart.Items.Remove(existing);
            }
        }
        public void UpdateCartItemStatus(Guid cartId, Guid productId, CartItemStatus status)
        {
            var cart = GetCart(cartId);
            var item = cart?.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Status = status;
            }
        }

        public Product GetProduct(Guid productId)
        {
            return dummyProducts.FirstOrDefault(p => p.Id == productId);
        }
        public List<Product> GetAllProducts()
        {
            return dummyProducts;
        }
        public void UpdateStock(Guid productId, int quantityChange)
        {
            var product = GetProduct(productId);
            if (product == null)
            {
                throw new DataException($"Product with id '{productId}' not found.");
            }
            product.Stock += quantityChange;
        }

        public List<Voucher> GetAllVouchers()
        {
            return dummyVouchers;
        }
        public Voucher GetVoucherByCode(string code)
        {
            return dummyVouchers.FirstOrDefault(v => v.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
        public void ApplyVoucherToCart(Guid cartId, Guid voucherId)
        {
            if (!dummyCartVouchers.Any(cv => cv.CartId == cartId && cv.VoucherId == voucherId))
            {
                dummyCartVouchers.Add(new CartVoucher { CartId = cartId, VoucherId = voucherId });
            }
        }
        public List<Voucher> GetCartVouchers(Guid cartId)
        {
            var voucherIds = dummyCartVouchers.Where(cv => cv.CartId == cartId).Select(cv => cv.VoucherId).ToList();
            return dummyVouchers.Where(v => voucherIds.Contains(v.Id)).ToList();
        }
    }
}
