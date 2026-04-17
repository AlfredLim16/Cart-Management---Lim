using Cart_Management.Core.Models;
using System;
using System.Collections.Generic;

namespace Cart_Management.Core.Validations
{
    public interface ICartValidation
    {
        void CheckCart(Cart cart, Guid cartId);
        void CheckCartNotEmpty(List<CartItem> items);
        void CheckMergeCartIds(Guid guestCartId, Guid userCartId);

        void CheckCartItem(CartItem item);
        void CheckItemInCart(CartItem item, Guid productId);
        void CheckUpdateQuantity(int newQuantity);

        void CheckProduct(Product product, Guid productId);
        void CheckStockAvailability(Product product, int requestedQuantity);
        void CheckStockForAdditionalQuantity(Product product, int existingQuantity, int addedQuantity);
        void CheckStockForCheckout(Product product, int requestedQuantity);
        
        void CheckUserId(Guid userId);
        void CheckUniqueItemThreshold(int currentUniqueItemsCount, int threshold);
        void CheckVoucherApplicable(Voucher voucher, List<CartItem> cartItems, List<Product> products);
    }
}
