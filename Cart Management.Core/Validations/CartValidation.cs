using Cart_Management.Core.Enums;
using Cart_Management.Core.Exceptions;
using Cart_Management.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cart_Management.Core.Validations
{
    public class CartValidation : ICartValidation
    {
        public const int MAX_UNIQUE_ITEMS = 10;

        public void CheckCart(Cart cart, Guid cartId)
        {
            if (cart == null)
            {
                throw new BusinessException($"Cart '{cartId}' not found.");
            }
            if (cartId != cart.Id)
            {
                throw new BusinessException($"Cart ID mismatch. Expected: '{cartId}', Actual: '{cart.Id}'.");
            }
            if (cartId == Guid.Empty)
            {
                throw new UserException("CartId cannot be empty.");
            }
        }
        public void CheckCartNotEmpty(List<CartItem> items)
        {
            if (items == null || items.Count == 0)
            {
                throw new BusinessException("Cart is empty.");
            }
        }
        public void CheckMergeCartIds(Guid guestCartId, Guid userCartId)
        {
            if (guestCartId == Guid.Empty)
            {
                throw new UserException("Guest CartId cannot be empty.");
            }
            else if (userCartId == Guid.Empty)
            {
                throw new UserException("User CartId cannot be empty.");
            }
            if (guestCartId == userCartId)
            {
                throw new UserException("Guest cart and user cart cannot be the same.");
            }
        }

        public void CheckCartItem(CartItem item)
        {
            if (item == null)
            {
                throw new UserException("Cart item cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(item.ProductName))
            {
                throw new UserException("Product name cannot be empty.");
            }
            if (item.Quantity <= 0)
            {
                throw new UserException("Quantity must be greater than zero.");
            }
            if (item.UnitPrice < 0)
            {
                throw new UserException("Unit price cannot be negative.");
            }
        }
        public void CheckItemInCart(CartItem item, Guid productId)
        {
            if (item == null)
            {
                throw new BusinessException($"Product '{productId}' is not in the cart.");
            }
        }
        public void CheckUpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
            {
                throw new UserException("Quantity must be greater than zero.");
            }
        }

        public void CheckProduct(Product product, Guid productId)
        {
            if (product == null)
            {
                throw new BusinessException($"Product '{productId}' not found.");
            }
            if (productId == Guid.Empty)
            {
                throw new UserException("ProductId cannot be empty.");
            }
        }
        public void CheckStockAvailability(Product product, int requestedQuantity)
        {
            if (product.Stock <= 0)
            {
                throw new BusinessException($"Product '{product.Name}' is out of stock.");
            }
            if (requestedQuantity > product.Stock)
            {
                throw new BusinessException($"Only {product.Stock} units of '{product.Name}' are available in stock.");
            }
        }
        public void CheckStockForAdditionalQuantity(Product product, int existingQuantity, int addedQuantity)
        {
            int totalQuantity = existingQuantity + addedQuantity;
            if (totalQuantity > product.Stock)
            {
                throw new BusinessException($"Cannot add {addedQuantity} units of '{product.Name}' to cart. Only {product.Stock - existingQuantity} more units can be added.");
            }
        }
        
        public void CheckStockForCheckout(Product product, int requestedQuantity)
        {
            if (product.Stock < requestedQuantity)
            {
                throw new BusinessException($"Insufficient stock for '{product.Name}'. Requested: {requestedQuantity}, Available: {product.Stock}.");
            }
        }

        public void CheckUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new UserException("UserId cannot be empty.");
            }
        }
        public void CheckUniqueItemThreshold(int currentUniqueItemsCount, int threshold)
        {
            if (currentUniqueItemsCount >= threshold)
            {
                throw new BusinessException($"Cart has reached its limit of {threshold} unique items.");
            }
        }
        public void CheckVoucherApplicable(Voucher voucher, List<CartItem> cartItems, List<Product> products)
        {
            if (voucher == null)
            {
                throw new BusinessException("Voucher not found.");
            }

            if (voucher.Type == VoucherType.Seller)
            {
                if (voucher.SellerId == null || voucher.SellerId == Guid.Empty)
                {
                    throw new BusinessException("Invalid seller voucher.");
                }

                bool hasSellerProduct = false;
                foreach (var item in cartItems)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null && product.SellerId == voucher.SellerId)
                    {
                        hasSellerProduct = true;
                        break;
                    }
                }

                if (!hasSellerProduct)
                {
                    throw new BusinessException("This voucher can only be applied to items from the specific seller.");
                }
            }
        }
    }
}
