using CartManagementModels;
using CartManagementBusinessLogic.Managers;
using System;
using System.Collections.Generic;

namespace CartManagementBusinessLogic.Services
{
    public class CartService
    {
        CartManager _cartManager;

        public CartService(CartManager cartManager)
        {
            _cartManager = cartManager;
        }
        public Cart Create(Cart cart)
        {
            return _cartManager.Create(cart);
        }
        public Cart? Get(Guid cartId)
        {
            return _cartManager.Get(cartId);
        }
        public List<Cart> GetAll()
        {
            return _cartManager.GetAll();
        }
        public void Update(Cart cart)
        {
            _cartManager.Update(cart);
        }
        public void Delete(Guid cartId)
        {
            _cartManager.Delete(cartId);
        }
        public void Clear(Guid cartId)
        {
            _cartManager.Clear(cartId);
        }
        public void AddItem(Guid cartId, CartItem item)
        {
            _cartManager.AddItem(cartId, item);
        }
        public void RemoveItem(Guid cartId, Guid cartItemId)
        {
            _cartManager.RemoveItem(cartId, cartItemId);
        }
        public List<CartItem> GetItems(Guid cartId)
        {
            return _cartManager.GetItems(cartId);
        }
        public int GetItemCount(Guid cartId)
        {
            return _cartManager.GetItemCount(cartId);
        }
        public decimal GetTotal(Guid cartId)
        {
            return _cartManager.GetTotal(cartId);
        }
        public bool ContainsItem(Guid cartId, Guid cartItemId)
        {
            return _cartManager.ContainsItem(cartId, cartItemId);
        }
        public bool IsEmpty(Guid cartId)
        {
            return _cartManager.IsEmpty(cartId);
        }
        public byte GetThreshold(Guid cartId)
        {
            return _cartManager.GetThreshold(cartId);
        }
        public void SetThreshold(Guid cartId, byte threshold)
        {
            _cartManager.SetThreshold(cartId, threshold);
        }
        public bool WithinThreshold(Guid cartId, CartItem item)
        {
            return _cartManager.WithinThreshold(cartId, item);
        }
    }
}