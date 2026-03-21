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
            var cartQuery = from cart in dummyCarts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                return cart;
            }
            return null;
        }
        public List<Cart> GetAll()
        {
            return dummyCarts;
        }
        public void Update(Cart cart)
        {
            var existingQuery = from existing in dummyCarts where existing.CartId == cart.CartId select existing;
            foreach (var existing in existingQuery)
            {
                existing.Items = cart.Items;
                existing.Threshold = cart.Threshold;
                break;
            }
        }
        public void Delete(Guid cartId)
        {
            var cartQuery = from cart in dummyCarts where cart.CartId == cartId select cart;
            Cart? toRemove = null;
            foreach (var cart in cartQuery)
            {
                toRemove = cart;
                break;
            }
            if (toRemove != null)
            {
                dummyCarts.Remove(toRemove);
            }
        }
        public void Clear(Guid cartId)
        {
            var cartQuery = from cart in dummyCarts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                cart.Items.Clear();
                break;
            }
        }
        public void AddItem(Guid cartId, CartItem item)
        {
            var cartQuery = from cart in dummyCarts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                cart.Items.Add(item);
                break;
            }
        }
        public void RemoveItem(Guid cartId, Guid cartItemId)
        {
            var cartQuery = from cart in dummyCarts where cart.CartId == cartId select cart;
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
                }
                break;
            }
        }
        public List<CartItem> GetItems(Guid cartId)
        {
            var cartItemsQuery = from cart in dummyCarts where cart.CartId == cartId select cart.Items;
            foreach (var cartItems in cartItemsQuery)
            {
                return cartItems;
            }
            return new List<CartItem>();
        }
        public int GetItemCount(Guid cartId)
        {
            var cartItemCountQuery = from cart in dummyCarts where cart.CartId == cartId select cart.Items.Count;
            foreach (var count in cartItemCountQuery)
            {
                return count;
            }
            return 0;
        }
        public decimal GetTotal(Guid cartId)
        {
            var cartItemsQuery = from cart in dummyCarts where cart.CartId == cartId select cart.Items;
            foreach (var cartItems in cartItemsQuery)
            {
                decimal total = 0;
                foreach (var item in cartItems)
                {
                    total += item.Price * item.Quantity;
                }
                return total;
            }
            return 0;
        }
        public bool ContainsItem(Guid cartId, Guid cartItemId)
        {
            var cartItemsQuery = from cart in dummyCarts where cart.CartId == cartId select cart.Items;
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
            var cartItemsQuery = from cart in dummyCarts where cart.CartId == cartId select cart.Items;
            foreach (var cartItems in cartItemsQuery)
            {
                return cartItems.Count == 0;
            }
            return true;
        }
        public byte GetThreshold(Guid cartId)
        {
            var cartQuery = from cart in dummyCarts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                int currentCount = cart.Items.Count;
                return (byte)Math.Max(0, cart.Threshold - currentCount);
            }
            return 0;
        }
        public void SetThreshold(Guid cartId, byte threshold)
        {
            var cartQuery = from cart in dummyCarts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                cart.Threshold = threshold;
                break;
            }
        }
        public bool WithinThreshold(Guid cartId, CartItem item)
        {
            var cartQuery = from cart in dummyCarts where cart.CartId == cartId select cart;
            foreach (var cart in cartQuery)
            {
                int currentCount = cart.Items.Count;
                return currentCount + 1 <= cart.Threshold;
            }
            return false;
        }
        public List<CartItem> GetSelectedItems(Guid cartId, List<Guid> cartItemIds)
        {
            var cartItemsQuery = from cart in dummyCarts where cart.CartId == cartId select cart.Items;
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