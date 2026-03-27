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
                Items = new List<CartItem>
                {
                    new CartItem { CartItemId = Guid.NewGuid(), ProductName = "Laptop", Quantity = 1, Price = 45000 },
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
            if(existing != null)
            {
                existing.Items = cart.Items;
                existing.Threshold = cart.Threshold;
            }
        }

        public void Delete(Guid cartId)
        {
            var toRemove = Get(cartId);
            if(toRemove != null)
            {
                dummyCarts.Remove(toRemove);
            }
        }
 
        public void Clear(Guid cartId)
        {
            var cart = Get(cartId);
            cart?.Items.Clear();                      
        }

        public void AddItem(Guid cartId, CartItem item)
        {
            var cart = Get(cartId);
            cart?.Items.Add(item);
        }

        public void RemoveItem(Guid cartId, Guid cartItemId)
        {
            var cart = Get(cartId);
            if(cart != null)
            {
                var cartItem = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
                if(cartItem != null)
                {
                    cart.Items.Remove(cartItem);
                }
            }                
        }

        public List<CartItem> GetItems(Guid cartId)
        {
            var cart = Get(cartId);
            if(cart != null)
            {
                if(cart.Items != null)
                {
                    return cart.Items;
                }
                else
                {
                    return new List<CartItem>();
                }
            }
            else
            {
                return new List<CartItem>();
            }
        }

        public int GetItemCount(Guid cartId)
        {
            var cart = Get(cartId);
            if(cart != null)
            {
                if(cart.Items != null)
                {
                    return cart.Items.Count;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public decimal GetTotal(Guid cartId)
        {
            Cart? cart = Get(cartId);
            if(cart != null)
            {
                if(cart.Items != null && cart.Items.Any())
                {
                    return cart.Items.Sum(i => i.Price * i.Quantity);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public bool ContainsItem(Guid cartId, Guid cartItemId)
        {
            var cart = Get(cartId);
            return cart?.Items.Any(i => i.CartItemId == cartItemId) ?? false;
        }

        public bool IsEmpty(Guid cartId)
        {
            return Get(cartId)?.Items.Count == 0;
        }

        public byte GetThreshold(Guid cartId)
        {
            var cart = Get(cartId);
            return (byte)(cart != null ? (short)Math.Max(0, cart.Threshold - cart.Items.Count) : (short)0);
        }

        public void SetThreshold(Guid cartId, short threshold)
        {
            var cart = Get(cartId);
            if(cart != null)
            {
                cart.Threshold = threshold;
            }
        }

        public bool WithinThreshold(Guid cartId, CartItem item)
        {
            var cart = Get(cartId);
            if(cart != null)
            {
                if(cart.Items != null)
                {
                    return cart.Items.Count + 1 <= cart.Threshold;
                }
                else
                {
                    return 1 <= cart.Threshold;
                }
            }
            else
            {
                return false;
            }
        }

        public List<CartItem> GetSelectedItems(Guid cartId, List<Guid> cartItemIds)
        {
            var cart = Get(cartId);
            if(cart != null)
            {
                if (cart.Items != null)
                {
                    return cart.Items.Where(i => cartItemIds.Contains(i.CartItemId)).ToList();
                }
                else
                {
                    return new List<CartItem>();
                }
            }
            else
            {
                return new List<CartItem>();
            }
        }
        
        public decimal GetSelectedTotal(Guid cartId, List<Guid> cartItemIds)
        {
            var selectedItems = GetSelectedItems(cartId, cartItemIds);
            if(selectedItems != null && selectedItems.Any())
            {
                return selectedItems.Sum(i => i.Price * i.Quantity);
            }
            else
            {
                return 0;
            }
        }

    }
}