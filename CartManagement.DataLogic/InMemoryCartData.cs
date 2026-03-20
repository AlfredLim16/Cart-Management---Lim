using System;
using System.Collections.Generic;
using System.Linq;
using CartManagementModels;

namespace CartManagementDataLogic
{
    public class InMemoryCartData : ICartDataLogic
    {
        public List<Cart> dummyCarts = new List<Cart>();

        public InMemoryCartData()
        {
            Cart sampleCart = new Cart()
            {
                CartId = Guid.NewGuid(),
                Items = new List<CartItem>
                {
                    new CartItem { CartItemId = Guid.NewGuid(), ProductName = "Laptop", Quantity = 1, Price = 450 },
                    new CartItem { CartItemId = Guid.NewGuid(), ProductName = "Mouse", Quantity = 2, Price = 500 },
                    new CartItem { CartItemId = Guid.NewGuid(), ProductName = "Apple", Quantity = 50, Price = 50 }
                },
                Threshold = 100
            };
            dummyCarts.Add(sampleCart);
        }
        public Cart Create(Cart cart)
        {
            dummyCarts.Add(cart);
            return cart;
        }
        public Cart? Get(Guid cartId)
        {
            return dummyCarts.FirstOrDefault(c => c.CartId == cartId);
        }
        public List<Cart> GetAll()
        {
            return dummyCarts;
        }
        public void Update(Cart cart)
        {
            var existing = Get(cart.CartId);
            if (existing != null)
            {
                existing.Items = cart.Items;
                existing.Threshold = cart.Threshold;
            }
        }
        public void Delete(Guid cartId)
        {
            var cart = Get(cartId);
            if (cart != null)
            {
                dummyCarts.Remove(cart);
            }
        }
        public void Clear(Guid cartId)
        {
            var cart = Get(cartId);
            if (cart != null)
            {
                cart.Items.Clear();
            }
        }
        public void AddItem(Guid cartId, CartItem item)
        {
            var cart = Get(cartId);
            if (cart != null)
            {
                cart.Items.Add(item);
            }
        }
        public void RemoveItem(Guid cartId, Guid cartItemId)
        {
            var cart = Get(cartId);
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
                if (item != null)
                {
                    cart.Items.Remove(item);
                }
            }
        }
        public List<CartItem> GetItems(Guid cartId)
        {
            var cart = Get(cartId);
            return cart?.Items ?? new List<CartItem>();
        }
        public int GetItemCount(Guid cartId)
        {
            var cart = Get(cartId);
            return cart?.Items.Count ?? 0;
        }
        public decimal GetTotal(Guid cartId)
        {
            var cart = Get(cartId);
            return cart?.Items.Sum(i => i.Price * i.Quantity) ?? 0;
        }
        public bool ContainsItem(Guid cartId, Guid cartItemId)
        {
            var cart = Get(cartId);
            return cart != null && cart.Items.Any(i => i.CartItemId == cartItemId);
        }
        public bool IsEmpty(Guid cartId)
        {
            var cart = Get(cartId);
            return cart == null || !cart.Items.Any();
        }
        public byte GetThreshold(Guid cartId)
        {
            var cart = Get(cartId);
            if (cart != null)
            {
                int currentCount = cart.Items.Count;
                return (byte)Math.Max(0, cart.Threshold - currentCount);
            }
            return 0;
        }
        public void SetThreshold(Guid cartId, byte threshold)
        {
            var cart = Get(cartId);
            if (cart != null)
            {
                cart.Threshold = threshold;
            }
        }
        public bool WithinThreshold(Guid cartId, CartItem item)
        {
            var cart = Get(cartId);
            if (cart != null)
            {
                int currentCount = cart.Items.Count;
                return currentCount + 1 <= cart.Threshold;
            }
            return false;
        }
        public List<CartItem> GetSelectedItems(Guid cartId, List<Guid> cartItemId)
        {
            var cart = Get(cartId);
            if (cart != null)
            {
                return cart.Items.Where(i => cartItemId.Contains(i.CartItemId)).ToList();
            }
            return new List<CartItem>();
        }
        public decimal GetSelectedTotal(Guid cartId, List<Guid> cartItemId)
        {
            var selectedItems = GetSelectedItems(cartId, cartItemId);
            return selectedItems.Sum(i => i.Price * i.Quantity);
        }
    }
}