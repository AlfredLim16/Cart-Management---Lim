using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartManagementModels;

namespace CartManagementDataLogic
{
    public interface ICartDataLogic
    {
        Cart Create(Cart cart);
        Cart? Get(Guid cartId);
        List<Cart> GetAll();
        void Update(Cart cart);
        void Delete(Guid cartId);
        void Clear(Guid cartId);

        void AddItem(Guid cartId, CartItem item);
        void RemoveItem(Guid cartId, Guid cartItemId);
        List<CartItem> GetItems(Guid cartId);
        int GetItemCount(Guid cartId);
        decimal GetTotal(Guid cartId);

        bool ContainsItem(Guid cartId, Guid cartItemId);
        bool IsEmpty(Guid cartId);

        byte GetThreshold(Guid cartId);
        void SetThreshold(Guid cartId, short threshold);
        bool WithinThreshold(Guid cartId, CartItem item);

        List<CartItem> GetSelectedItems(Guid cartId, List<Guid> cartItemIds);
        decimal GetSelectedTotal(Guid cartId, List<Guid> cartItemIds);
    }
}