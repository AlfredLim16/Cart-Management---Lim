using CartManagementDataLogic;
using CartManagementModels;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CartManagementDataLogic
{
    public class CartDBData : ICartDataLogic
    {
        private SqlConnection sqlConnection;
        private string connectionString = "Data Source =DESKTOP-B8432K3\\SQLEXPRESS; Initial Catalog = CartMgmt; Integrated Security = True; TrustServerCertificate=True;";
        
        public CartDBData()
        {
            sqlConnection = new SqlConnection(connectionString);
            AddSeeds();
        }
        
        private void AddSeeds()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Carts", connection);
            int count = (int)checkCommand.ExecuteScalar();

            if(count == 0)
            {
                var cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    Threshold = 100,
                    Items = new List<CartItem>
                    {
                        new CartItem { CartItemId = Guid.NewGuid(), ProductName = "Laptop", Quantity = 1, Price = 45000m },
                        new CartItem { CartItemId = Guid.NewGuid(), ProductName = "Mouse", Quantity = 2, Price = 500m },
                        new CartItem { CartItemId = Guid.NewGuid(), ProductName = "Apple", Quantity = 50, Price = 50m }
                    }
                };
                Create(cart);
            }
        }
        public void AddItem(Guid cartId, CartItem item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var insertItemsCommand = new SqlCommand("INSERT INTO CartItems (CartItemId, CartId, ProductName, Quantity, Price) VALUES (@CartItemId, @CartId, @ProductName, @Quantity, @Price)", connection);
            insertItemsCommand.Parameters.AddWithValue("@CartItemId", item.CartItemId);
            insertItemsCommand.Parameters.AddWithValue("@CartId", cartId);
            insertItemsCommand.Parameters.AddWithValue("@ProductName", item.ProductName);
            insertItemsCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
            insertItemsCommand.Parameters.AddWithValue("@Price", item.Price);
            insertItemsCommand.ExecuteNonQuery();
        }

        public void Clear(Guid cartId)
        {
            using var connection = new SqlConnection(connectionString);
            connection  .Open();

            var deleteItemsCommand = new SqlCommand("DELETE FROM CartItems WHERE CartId=@CartId", connection);
            deleteItemsCommand.Parameters.AddWithValue("@CartId", cartId);
            deleteItemsCommand.ExecuteNonQuery();
        }

        public bool ContainsItem(Guid cartId, Guid cartItemId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var countItemsCommand = new SqlCommand("SELECT COUNT(*) FROM CartItems WHERE CartId=@CartId AND CartItemId=@CartItemId", connection);
            countItemsCommand.Parameters.AddWithValue("@CartId", cartId);
            countItemsCommand.Parameters.AddWithValue("@CartItemId", cartItemId);

            return (int)countItemsCommand.ExecuteScalar() > 0;
        }

        public Cart Create(Cart cart)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var insertCartsCommand = new SqlCommand("INSERT INTO Carts (CartId, Threshold) VALUES (@CartId, @Threshold)", connection);
            insertCartsCommand.Parameters.AddWithValue("@CartId", cart.CartId);
            insertCartsCommand.Parameters.AddWithValue("@Threshold", cart.Threshold);
            insertCartsCommand.ExecuteNonQuery();

            foreach(var item in cart.Items)
            {
                var insertItemsCommand = new SqlCommand("INSERT INTO CartItems (CartItemId, CartId, ProductName, Quantity, Price) VALUES (@CartItemId, @CartId, @ProductName, @Quantity, @Price)", connection);
                insertItemsCommand.Parameters.AddWithValue("@CartItemId", item.CartItemId);
                insertItemsCommand.Parameters.AddWithValue("@CartId", cart.CartId);
                insertItemsCommand.Parameters.AddWithValue("@ProductName", item.ProductName);
                insertItemsCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                insertItemsCommand.Parameters.AddWithValue("@Price", item.Price);
                insertItemsCommand.ExecuteNonQuery();
            }

            return cart;
        }

        public void Delete(Guid cartId)
        {
            Clear(cartId);
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var deleteCartsCommand = new SqlCommand("DELETE FROM Carts WHERE CartId=@CartId", connection);
            deleteCartsCommand.Parameters.AddWithValue("@CartId", cartId);
            deleteCartsCommand.ExecuteNonQuery();
        }

        public Cart? Get(Guid cartId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var selectCartsCommand = new SqlCommand("SELECT CartId, Threshold FROM Carts WHERE CartId = @CartId", connection);
            selectCartsCommand.Parameters.AddWithValue("@CartId", cartId);

            using var reader = selectCartsCommand.ExecuteReader();
            if(!reader.Read()) return null;

            var cart = new Cart
            {
                CartId = reader.GetGuid(0),
                Threshold = reader.GetInt16(1),
                Items = new List<CartItem>()
            };

            reader.Close();

            var selectItemsCommand = new SqlCommand("SELECT CartItemId, ProductName, Quantity, Price FROM CartItems WHERE CartId = @CartId", connection);
            selectItemsCommand.Parameters.AddWithValue("@CartId", cartId);

            using var itemReader = selectItemsCommand.ExecuteReader();
            while(itemReader.Read())
            {
                cart.Items.Add(new CartItem
                {
                    CartItemId = itemReader.GetGuid(0),
                    ProductName = itemReader.GetString(1),
                    Quantity = itemReader.GetByte(2),
                    Price = itemReader.GetDecimal(3)
                });
            }
            return cart;
        }

        public List<Cart> GetAll()
        {
            var carts = new List<Cart>();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var selectCartsCommand = new SqlCommand("SELECT CartId, Threshold FROM Carts", connection);
            using var reader = selectCartsCommand.ExecuteReader();
            while(reader.Read())
            {
                var cartId = reader.GetGuid(0);
                var cart = new Cart
                {
                    CartId = cartId,
                    Threshold = reader.GetInt16(1),
                    Items = GetItems(cartId)
                };
                carts.Add(cart);
            }
            return carts;
        }

        public int GetItemCount(Guid cartId)
        {
            return GetItems(cartId).Count;
        }

        public List<CartItem> GetItems(Guid cartId)
        {
            var items = new List<CartItem>();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var selectItemsCommand = new SqlCommand("SELECT CartItemId, ProductName, Quantity, Price FROM CartItems WHERE CartId=@CartId", connection);
            selectItemsCommand.Parameters.AddWithValue("@CartId", cartId);

            using var reader = selectItemsCommand.ExecuteReader();
            while(reader.Read())
            {
                items.Add(new CartItem
                {
                    CartItemId = reader.GetGuid(0),
                    ProductName = reader.GetString(1),
                    Quantity = reader.GetByte(2),
                    Price = reader.GetDecimal(3)
                });
            }
            return items;
        }

        public List<CartItem> GetSelectedItems(Guid cartId, List<Guid> cartItemIds)
        {
            var items = new List<CartItem>();
            foreach(var item in GetItems(cartId))
            {
                if (cartItemIds.Contains(item.CartItemId))
                    items.Add(item);
            }
            return items;
        }

        public decimal GetSelectedTotal(Guid cartId, List<Guid> cartItemIds)
        {
            decimal total = 0;
            foreach(var item in GetSelectedItems(cartId, cartItemIds))
            {
                total += item.Price * item.Quantity;
            }
            return total;
        }

        public byte GetThreshold(Guid cartId)
        {
            var cart = Get(cartId);
            if (cart == null) return 0;
            return (byte)Math.Max(0, cart.Threshold - cart.Items.Count);
        }

        public decimal GetTotal(Guid cartId)
        {
            decimal total = 0;
            foreach(var item in GetItems(cartId))
            {
                total += item.Price * item.Quantity;
            }
            return total;
        }

        public bool IsEmpty(Guid cartId)
        {
            return GetItemCount(cartId) == 0;
        }

        public void RemoveItem(Guid cartId, Guid cartItemId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var deleteItemCommand = new SqlCommand("DELETE FROM CartItems WHERE CartId=@CartId AND CartItemId=@CartItemId", connection);
            deleteItemCommand.Parameters.AddWithValue("@CartId", cartId);
            deleteItemCommand.Parameters.AddWithValue("@CartItemId", cartItemId);
            deleteItemCommand.ExecuteNonQuery();
        }

        public void SetThreshold(Guid cartId, short threshold)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var updateCartsCommand = new SqlCommand("UPDATE Carts SET Threshold=@Threshold WHERE CartId=@CartId", connection);
            updateCartsCommand.Parameters.AddWithValue("@Threshold", threshold);
            updateCartsCommand.Parameters.AddWithValue("@CartId", cartId);
            updateCartsCommand.ExecuteNonQuery();
        }

        public void Update(Cart cart)
        {
            SetThreshold(cart.CartId, cart.Threshold);
            Clear(cart.CartId);
            foreach(var item in cart.Items)
            {
                AddItem(cart.CartId, item);
            }
        }

        public bool WithinThreshold(Guid cartId, CartItem item)
        {
            var cart = Get(cartId);
            if(cart == null)
            {
                return false;
            }
            return cart.Items.Count + item.Quantity <= cart.Threshold;
        }
    }
}