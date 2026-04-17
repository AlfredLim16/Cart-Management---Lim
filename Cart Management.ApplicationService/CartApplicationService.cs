using Cart_Management.DataService;
using Cart_Management.Core.Enums;
using Cart_Management.Core.Exceptions;
using Cart_Management.Core.Validations;
using Cart_Management.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cart_Management.ApplicationService
{
    public class CartApplicationService : ICartApplicationService
    {
        private ICartDataService _dataService;
        private ICartValidation _validation;
        
        public CartApplicationService(ICartDataService dataService, ICartValidation validation)
        {
            _dataService = dataService;
            _validation = validation;
        }

        public Guid CreateCart(Guid userId)
        {
            _validation.CheckUserId(userId);
            Cart cart = _dataService.CreateCart(userId);
            return cart.Id;
        }

        public Guid? GetCartByUserId(Guid userId)
        {
            _validation.CheckUserId(userId);
            Cart cart = _dataService.GetCartByUserId(userId);
            return cart?.Id;
        }

        public Cart GetCart(Guid cartId)
        {
            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);
            return existingCart;
        }
        public void ClearCart(Guid cartId)
        {
            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);
            _dataService.ClearCart(cartId);
        }
        public void CheckoutCart(Guid cartId)
        {
            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);

            List<CartItem> items = _dataService.GetCartItems(cartId);
            _validation.CheckCartNotEmpty(items);
            foreach (var item in items)
            {
                Product product = _dataService.GetProduct(item.ProductId);
                _validation.CheckProduct(product, item.ProductId);
                _validation.CheckStockForCheckout(product, item.Quantity);
            }
            foreach (CartItem item in items)
            {
                Product product = _dataService.GetProduct(item.ProductId);
                _dataService.UpdateStock(item.ProductId, -item.Quantity);
            }
            _dataService.ClearCart(cartId);
        }
        public decimal GetCartTotal(Guid cartId)
        {
            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);

            decimal total = _dataService.GetCartTotal(cartId);

            List<Voucher> vouchers = _dataService.GetCartVouchers(cartId);
            if (vouchers.Count == 0) return total;

            List<CartItem> items = _dataService.GetCartItems(cartId);
            List<Product> products = _dataService.GetAllProducts();

            foreach (var voucher in vouchers)
            {
                if (voucher.Type == VoucherType.Platform)
                {
                    total -= voucher.DiscountAmount;
                }
                else if (voucher.Type == VoucherType.Seller)
                {
                    decimal sellerSubtotal = 0;
                    foreach (var item in items)
                    {
                        var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                        if (product != null && product.SellerId == voucher.SellerId)
                        {
                            sellerSubtotal += item.Quantity * item.UnitPrice;
                        }
                    }
                    decimal discountToApply = Math.Min(sellerSubtotal, voucher.DiscountAmount);
                    total -= discountToApply;
                }
            }

            return Math.Max(0, total);
        }
        
        public List<CartItem> GetCartItems(Guid cartId)
        {
            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);
            return _dataService.GetCartItems(cartId);
        }
        public void AddItem(Guid cartId, Guid productId, int quantity)
        {
            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);

            Product existingProduct = _dataService.GetProduct(productId);
            _validation.CheckProduct(existingProduct, productId);

            _validation.CheckStockAvailability(existingProduct, quantity);

            List<CartItem> cartItems = _dataService.GetCartItems(cartId);
            CartItem existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem == null)
            {
                _validation.CheckUniqueItemThreshold(cartItems.Count, CartValidation.MAX_UNIQUE_ITEMS);

                CartItem newItem = new CartItem
                {
                    ProductId = productId,
                    ProductName = existingProduct.Name,
                    Quantity = quantity,
                    UnitPrice = existingProduct.UnitPrice,
                    Status = CartItemStatus.NotAvailable
                };
                _dataService.AddItem(cartId, newItem);
            }
            else
            {
                _validation.CheckStockForAdditionalQuantity(existingProduct, existingItem.Quantity, quantity);
                int totalQuantity = existingItem.Quantity + quantity;
                existingItem.Quantity = totalQuantity;
                existingItem.UnitPrice = existingProduct.UnitPrice;
                _dataService.UpdateItem(cartId, existingItem);
            }

            UpdateItemStatusBasedOnStock(cartId, productId);
        }
        public void UpdateItemQuantity(Guid cartId, Guid productId, int newQuantity)
        {
            _validation.CheckUpdateQuantity(newQuantity);

            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);

            List<CartItem> cartItems = _dataService.GetCartItems(cartId);
            CartItem existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId);

            _validation.CheckItemInCart(existingItem, productId);

            Product product = _dataService.GetProduct(productId);
            if (product != null)
            {
                _validation.CheckStockAvailability(product, newQuantity);
            }

            existingItem.Quantity = newQuantity;
            _dataService.UpdateItem(cartId, existingItem);
            UpdateItemStatusBasedOnStock(cartId, productId);
        }
        public void RemoveItem(Guid cartId, Guid productId)
        {
            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);
            List<CartItem> cartItems = _dataService.GetCartItems(cartId);
            CartItem existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId);
            _validation.CheckItemInCart(existingItem, productId);
            _dataService.RemoveItem(cartId, productId);
        }
        public void UpdateCartItemPrices(Guid cartId)
        {
            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);

            List<CartItem> items = _dataService.GetCartItems(cartId);

            foreach (var item in items)
            {
                Product product = _dataService.GetProduct(item.ProductId);
                if (product != null && product.UnitPrice != item.UnitPrice)
                {
                    item.UnitPrice = product.UnitPrice;
                    _dataService.UpdateItem(cartId, item);
                }
            }
        }

        public Product GetProduct(Guid productId)
        {
            return _dataService.GetProduct(productId);
        }
        public List<Product> GetAllProducts()
        {
            return _dataService.GetAllProducts();
        }
        public int GetAvailableStock(Guid productId)
        {
            Product product = _dataService.GetProduct(productId);
            _validation.CheckProduct(product, productId);
            return product.Stock;
        }
        
        public void ApplyVoucher(Guid cartId, string code)
        {
            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);

            Voucher voucher = _dataService.GetVoucherByCode(code);
            if (voucher == null)
            {
                throw new BusinessException($"Voucher with code '{code}' does not exist.");
            }

            List<CartItem> cartItems = _dataService.GetCartItems(cartId);
            List<Product> allProducts = _dataService.GetAllProducts();

            _validation.CheckVoucherApplicable(voucher, cartItems, allProducts);

            _dataService.ApplyVoucherToCart(cartId, voucher.Id);
        }
        public List<Voucher> GetAllVouchers()
        {
            return _dataService.GetAllVouchers();
        }
        public List<Voucher> GetCartVouchers(Guid cartId)
        {
            Cart existingCart = _dataService.GetCart(cartId);
            _validation.CheckCart(existingCart, cartId);
            return _dataService.GetCartVouchers(cartId);
        }
        private void UpdateItemStatusBasedOnStock(Guid cartId, Guid productId)
        {
            CartItem item = _dataService.GetCartItems(cartId).FirstOrDefault(i => i.ProductId == productId);
            Product product = _dataService.GetProduct(productId);

            if (item != null && product != null)
            {
                CartItemStatus newStatus;

                if (product.Stock <= 0)
                {
                    newStatus = CartItemStatus.OutOfStock;
                }
                else if (item.Quantity <= product.Stock)
                {
                    newStatus = CartItemStatus.NotAvailable;
                }
                else
                {
                    newStatus = CartItemStatus.OutOfStock;
                }

                _dataService.UpdateCartItemStatus(cartId, productId, newStatus);
            }
        }
    }
}
