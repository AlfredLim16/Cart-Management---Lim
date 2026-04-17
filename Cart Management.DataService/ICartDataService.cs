using Cart_Management.Core.Enums;
using Cart_Management.Core.Models;
using System;
using System.Collections.Generic;

namespace Cart_Management.DataService
{
    public interface ICartDataService
    {
        Cart CreateCart(Guid userId);
        Cart GetCart(Guid cartId);
        Cart GetCartByUserId(Guid userId);
        void ClearCart(Guid cartId);
        decimal GetCartTotal(Guid cartId);

        List<CartItem> GetCartItems(Guid cartId);
        void AddItem(Guid cartId, CartItem item);
        void UpdateItem(Guid cartId, CartItem item);
        void RemoveItem(Guid cartId, Guid productId);
        void UpdateCartItemStatus(Guid cartId, Guid productId, CartItemStatus status);

        List<Product> GetAllProducts();
        Product GetProduct(Guid productId);
        void UpdateStock(Guid productId, int quantityChange);

        List<Voucher> GetAllVouchers();
        Voucher GetVoucherByCode(string code);
        void ApplyVoucherToCart(Guid cartId, Guid voucherId);
        List<Voucher> GetCartVouchers(Guid cartId);
    }
}
