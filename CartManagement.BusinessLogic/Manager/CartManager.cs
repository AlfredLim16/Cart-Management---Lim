using CartManagementBusinessLogic.Rules;
using CartManagementDataLogic;
using CartManagementModels;
using System;
using System.Collections.Generic;

namespace CartManagementBusinessLogic.Managers
{
    public class CartManager
    {
        ICartDataLogic _dataLogic;
        CartRules _cartRules;
        public CartManager(ICartDataLogic dataLogic, CartRules cartRules)
        {
            _dataLogic = dataLogic;
            _cartRules = cartRules;
        }
        public Cart Create(Cart cart)
        {
            return _dataLogic.Create(cart);
        }
        public Cart? Get(Guid cartId)
        {
            return _dataLogic.Get(cartId);
        }
        public List<Cart> GetAll()
        {
            return _dataLogic.GetAll();
        }
        public void Update(Cart cart)
        {
            _dataLogic.Update(cart);
        }
        public void Delete(Guid cartId)
        {
            _dataLogic.Delete(cartId);
        }
        public void Clear(Guid cartId)
        {
            _dataLogic.Clear(cartId);
        }
        public void AddItem(Guid cartId, CartItem item)
        {
            _cartRules.ValidateThreshold(cartId, item, _dataLogic);
            _cartRules.ValidateQuantity(item);
            _cartRules.ValidatePrice(item);
            _dataLogic.AddItem(cartId, item);
        }
        public void RemoveItem(Guid cartId, Guid cartItemId)
        {
            _dataLogic.RemoveItem(cartId, cartItemId);
        }
        public List<CartItem> GetItems(Guid cartId)
        {
            return _dataLogic.GetItems(cartId);
        }
        public int GetItemCount(Guid cartId)
        {
            return _dataLogic.GetItemCount(cartId);
        }
        public decimal GetTotal(Guid cartId)
        {
            return _dataLogic.GetTotal(cartId);
        }
        public bool ContainsItem(Guid cartId, Guid cartItemId)
        {
            return _dataLogic.ContainsItem(cartId, cartItemId);
        }
        public bool IsEmpty(Guid cartId)
        {
            return _dataLogic.IsEmpty(cartId);
        }
        public byte GetThreshold(Guid cartId)
        {
            return _dataLogic.GetThreshold(cartId);
        }
        public void SetThreshold(Guid cartId, byte threshold)
        {
            _dataLogic.SetThreshold(cartId, threshold);
        }
        public bool WithinThreshold(Guid cartId, CartItem item)
        {
            return _dataLogic.WithinThreshold(cartId, item);
        }
        public List<CartItem> GetSelectedItems(Guid cartId, List<Guid> cartItemIds)
        {
            return _dataLogic.GetSelectedItems(cartId, cartItemIds);
        }
        public decimal GetSelectedTotal(Guid cartId, List<Guid> cartItemId)
        {
            return _dataLogic.GetSelectedTotal(cartId, cartItemId);
        }
    }
}