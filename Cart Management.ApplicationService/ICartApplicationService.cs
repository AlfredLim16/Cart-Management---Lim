using Cart_Management.Core.Models;
using System;
using System.Collections.Generic;

namespace Cart_Management.ApplicationService
{
    public interface ICartApplicationService
    {
        Guid CreateCart(Guid userId);
        Guid? GetCartByUserId(Guid userId);
        Cart GetCart(Guid cartId);
        decimal GetCartTotal(Guid cartId);
        void ClearCart(Guid cartId);
        void CheckoutCart(Guid cartId);

        List<CartItem> GetCartItems(Guid cartId);
        void AddItem(Guid cartId, Guid productId, int quantity);
        void UpdateItemQuantity(Guid cartId, Guid productId, int newQuantity);
        void RemoveItem(Guid cartId, Guid productId);
        void UpdateCartItemPrices(Guid cartId);

        List<Product> GetAllProducts();
        Product GetProduct(Guid productId);
        int GetAvailableStock(Guid productId);
        
        List<Voucher> GetAllVouchers();
        List<Voucher> GetCartVouchers(Guid cartId);
        void ApplyVoucher(Guid cartId, string code);
    }
}
