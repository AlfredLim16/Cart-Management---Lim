using Cart_Management.Core.Enums;
using Cart_Management.Core.Exceptions;
using Cart_Management.Core.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Cart_Management.DataService
{
    public class DatabaseDataService : ICartDataService
    {
        private string connectionString = "Data Source=localhost\\SQLEXPRESS; Initial Catalog=CartMgmt; Integrated Security=True; TrustServerCertificate=True;";
        private SqlConnection sqlConnection;
        private List<Product> dummyProducts = new List<Product>();
        private List<Voucher> dummyVouchers = new List<Voucher>();
        public DatabaseDataService()
        {
            sqlConnection = new SqlConnection(connectionString);
            AddSeeds();
        }
        private void AddSeeds()
        {
            List<Product> existingProducts = GetAllProducts();
            if (existingProducts.Count == 0)
            {
                sqlConnection.Open();
                Guid seller1 = Guid.NewGuid();
                Guid seller2 = Guid.NewGuid();
                dummyProducts = new List<Product>
                {
                    new Product { Id = Guid.NewGuid(), Name = "Laptop",     UnitPrice = 45000m, Stock = 10, SellerId = seller1 },
                    new Product { Id = Guid.NewGuid(), Name = "Smartphone", UnitPrice = 12000m, Stock = 20, SellerId = seller2 },
                    new Product { Id = Guid.NewGuid(), Name = "Headphones", UnitPrice = 350m,   Stock = 30, SellerId = Guid.NewGuid() },
                    new Product { Id = Guid.NewGuid(), Name = "Shoes",      UnitPrice = 200m,   Stock = 15, SellerId = Guid.NewGuid() },
                    new Product { Id = Guid.NewGuid(), Name = "Backpack",   UnitPrice = 300m,   Stock = 25, SellerId = Guid.NewGuid() }
                };

                foreach (Product product in dummyProducts)
                {
                    string insertProduct = "INSERT INTO Products (Id, Name, UnitPrice, Stock, SellerId) VALUES (@Id, @Name, @UnitPrice, @Stock, @SellerId)";
                    SqlCommand cmd = new SqlCommand(insertProduct, sqlConnection);
                    cmd.Parameters.AddWithValue("@Id", product.Id);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);
                    cmd.Parameters.AddWithValue("@SellerId", product.SellerId);
                    cmd.ExecuteNonQuery();
                }

                dummyVouchers = new List<Voucher>
                {
                    new Voucher { Id = Guid.NewGuid(), Code = "WELCOME100", Type = VoucherType.Platform, DiscountAmount = 100m },
                    new Voucher { Id = Guid.NewGuid(), Code = "SELLER50", Type = VoucherType.Seller, DiscountAmount = 50m, SellerId = seller1 }
                };

                foreach (Voucher voucher in dummyVouchers)
                {
                    string insertVoucher = "INSERT INTO Vouchers (Id, Code, Type, DiscountAmount, SellerId) VALUES (@Id, @Code, @Type, @DiscountAmount, @SellerId)";
                    SqlCommand cmd = new SqlCommand(insertVoucher, sqlConnection);
                    cmd.Parameters.AddWithValue("@Id", voucher.Id);
                    cmd.Parameters.AddWithValue("@Code", voucher.Code);
                    cmd.Parameters.AddWithValue("@Type", voucher.Type.ToString());
                    cmd.Parameters.AddWithValue("@DiscountAmount", voucher.DiscountAmount);
                    cmd.Parameters.AddWithValue("@SellerId", (object)voucher.SellerId ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
                sqlConnection.Close();
            }
        }
        public Cart CreateCart(Guid userId)
        {
            sqlConnection.Open();
            Guid cartId = Guid.NewGuid();
            string insertQuery = "INSERT INTO Carts (Id, UserId) VALUES (@Id, @UserId)";
            SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection);
            insertCommand.Parameters.AddWithValue("@Id", cartId);
            insertCommand.Parameters.AddWithValue("@UserId", userId);
            insertCommand.ExecuteNonQuery();
            Cart cart = new Cart
            {
                Id = cartId,
                UserId = userId,
                Items = new List<CartItem>()
            };
            sqlConnection.Close();
            return cart;
        }
        public Cart GetCart(Guid cartId)
        {
            sqlConnection.Open();
            string query = "SELECT Id, UserId FROM Carts WHERE Id = @CartId";
            SqlCommand selectCommand = new SqlCommand(query, sqlConnection);
            selectCommand.Parameters.AddWithValue("@CartId", cartId);
            SqlDataReader reader = selectCommand.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                sqlConnection.Close();
                return null;
            }
            Cart cart = new Cart
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                UserId = Guid.Parse(reader["UserId"].ToString()),
                Items = new List<CartItem>()
            };
            reader.Close();
            sqlConnection.Close();
            return cart;
        }
        public Cart GetCartByUserId(Guid userId)
        {
            sqlConnection.Open();
            string query = "SELECT Id, UserId FROM Carts WHERE UserId = @UserId";
            SqlCommand selectCommand = new SqlCommand(query, sqlConnection);
            selectCommand.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader reader = selectCommand.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                sqlConnection.Close();
                return null;
            }
            Cart cart = new Cart
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                UserId = Guid.Parse(reader["UserId"].ToString()),
                Items = new List<CartItem>()
            };
            reader.Close();
            sqlConnection.Close();
            return cart;
        }
        public void ClearCart(Guid cartId)
        {
            sqlConnection.Open();
            string deleteQuery = "DELETE FROM CartItems WHERE CartId = @CartId";
            SqlCommand clearCommand = new SqlCommand(deleteQuery, sqlConnection);
            clearCommand.Parameters.AddWithValue("@CartId", cartId);
            clearCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        public decimal GetCartTotal(Guid cartId)
        {
            sqlConnection.Open();
            string selectQuery = "SELECT SUM(Quantity * UnitPrice) FROM CartItems WHERE CartId = @CartId";
            SqlCommand cmd = new SqlCommand(selectQuery, sqlConnection);
            cmd.Parameters.AddWithValue("@CartId", cartId);
            object result = cmd.ExecuteScalar();
            sqlConnection.Close();
            if (result == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Convert.ToDecimal(result);
            }
        }

        public List<CartItem> GetCartItems(Guid cartId)
        {
            sqlConnection.Open();
            List<CartItem> cartItems = new List<CartItem>();
            string selectQuery = "SELECT ProductId, ProductName, Quantity, UnitPrice FROM CartItems WHERE CartId = @CartId";
            SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
            selectCommand.Parameters.AddWithValue("@CartId", cartId);
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                CartItem item = new CartItem
                {
                    ProductId = Guid.Parse(reader["ProductId"].ToString()),
                    ProductName = reader["ProductName"].ToString(),
                    Quantity = int.Parse(reader["Quantity"].ToString()),
                    UnitPrice = decimal.Parse(reader["UnitPrice"].ToString())
                };
                cartItems.Add(item);
            }
            reader.Close();
            sqlConnection.Close();
            return cartItems;
        }
        public void AddItem(Guid cartId, CartItem item)
        {
            sqlConnection.Open();
            string insertQuery = "INSERT INTO CartItems(CartId, ProductId, ProductName, Quantity, UnitPrice) VALUES (@CartId, @ProductId, @ProductName, @Quantity, @UnitPrice)";
            SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection);
            insertCommand.Parameters.AddWithValue("@CartId", cartId);
            insertCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
            insertCommand.Parameters.AddWithValue("@ProductName", item.ProductName);
            insertCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
            insertCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
            insertCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        public void UpdateItem(Guid cartId, CartItem item)
        {
            sqlConnection.Open();
            string updateQuery = "UPDATE CartItems SET Quantity = @Quantity, UnitPrice = @UnitPrice WHERE CartId = @CartId AND ProductId = @ProductId";
            SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
            updateCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
            updateCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
            updateCommand.Parameters.AddWithValue("@CartId", cartId);
            updateCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
            updateCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        public void RemoveItem(Guid cartId, Guid productId)
        {
            sqlConnection.Open();
            string deleteQuery = "DELETE FROM CartItems WHERE CartId = @CartId AND ProductId = @ProductId";
            SqlCommand removeCommand = new SqlCommand(deleteQuery, sqlConnection);
            removeCommand.Parameters.AddWithValue("@CartId", cartId);
            removeCommand.Parameters.AddWithValue("@ProductId", productId);
            removeCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        public void UpdateCartItemStatus(Guid cartId, Guid productId, CartItemStatus status)
        {
            sqlConnection.Open();
            string updateQuery = "UPDATE CartItems SET Status = @Status WHERE CartId = @CartId AND ProductId = @ProductId";
            SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
            updateCommand.Parameters.AddWithValue("@Status", status.ToString());
            updateCommand.Parameters.AddWithValue("@CartId", cartId);
            updateCommand.Parameters.AddWithValue("@ProductId", productId);
            updateCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        public Product GetProduct(Guid productId)
        {
            sqlConnection.Open();
            string getProductQuery = "SELECT Id, Name, UnitPrice, Stock, SellerId FROM Products WHERE Id = @ProductId";
            SqlCommand selectCommand = new SqlCommand(getProductQuery, sqlConnection);
            selectCommand.Parameters.AddWithValue("@ProductId", productId);
            SqlDataReader reader = selectCommand.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                sqlConnection.Close();
                return null;
            }
            Product product = new Product
            {
                Id = Guid.Parse(reader[0].ToString()),
                Name = reader[1].ToString(),
                UnitPrice = decimal.Parse(reader[2].ToString()),
                Stock = int.Parse(reader[3].ToString()),
                SellerId = Guid.Parse(reader[4].ToString())
            };
            reader.Close();
            sqlConnection.Close();
            return product;
        }
        public List<Product> GetAllProducts()
        {
            sqlConnection.Open();
            List<Product> products = new List<Product>();
            string selectQuery = "SELECT Id, Name, UnitPrice, Stock, SellerId FROM Products";
            SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Product product = new Product
                {
                    Id = Guid.Parse(reader[0].ToString()),
                    Name = reader[1].ToString(),
                    UnitPrice = decimal.Parse(reader[2].ToString()),
                    Stock = int.Parse(reader[3].ToString()),
                    SellerId = Guid.Parse(reader[4].ToString())
                };
                products.Add(product);
            }
            reader.Close();
            sqlConnection.Close();
            return products;
        }
        public void UpdateStock(Guid productId, int quantityChange)
        {
            sqlConnection.Open();
            string updateQuery = "UPDATE Products SET Stock = Stock + @QuantityChange WHERE Id = @ProductId";
            SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
            updateCommand.Parameters.AddWithValue("@QuantityChange", quantityChange);
            updateCommand.Parameters.AddWithValue("@ProductId", productId);
            if (updateCommand.ExecuteNonQuery() == 0)
            {
                throw new DataException($"Product with id '{productId}' not found.");
            }
            sqlConnection.Close();
        }

        public List<Voucher> GetAllVouchers()
        {
            sqlConnection.Open();
            List<Voucher> vouchers = new List<Voucher>();
            string selectQuery = "SELECT Id, Code, Type, DiscountAmount, SellerId FROM Vouchers";
            SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                vouchers.Add(new Voucher
                {
                    Id = Guid.Parse(reader["Id"].ToString()),
                    Code = reader["Code"].ToString(),
                    Type = (VoucherType)Enum.Parse(typeof(VoucherType), reader["Type"].ToString()),
                    DiscountAmount = decimal.Parse(reader["DiscountAmount"].ToString()),
                    SellerId = reader["SellerId"] != DBNull.Value ? Guid.Parse(reader["SellerId"].ToString()) : (Guid?)null
                });
            }
            reader.Close();
            sqlConnection.Close();
            return vouchers;
        }
        public Voucher GetVoucherByCode(string code)
        {
            sqlConnection.Open();
            string selectQuery = "SELECT Id, Code, Type, DiscountAmount, SellerId FROM Vouchers WHERE Code = @Code";
            SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
            selectCommand.Parameters.AddWithValue("@Code", code);
            SqlDataReader reader = selectCommand.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                sqlConnection.Close();
                return null;
            }
            Voucher voucher = new Voucher
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                Code = reader["Code"].ToString(),
                Type = (VoucherType)Enum.Parse(typeof(VoucherType), reader["Type"].ToString()),
                DiscountAmount = decimal.Parse(reader["DiscountAmount"].ToString()),
                SellerId = reader["SellerId"] != DBNull.Value ? Guid.Parse(reader["SellerId"].ToString()) : (Guid?)null
            };
            reader.Close();
            sqlConnection.Close();
            return voucher;
        }
        public void ApplyVoucherToCart(Guid cartId, Guid voucherId)
        {
            sqlConnection.Open();
            string insertQuery = "INSERT INTO CartVouchers (CartId, VoucherId) VALUES (@CartId, @VoucherId)";
            SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection);
            insertCommand.Parameters.AddWithValue("@CartId", cartId);
            insertCommand.Parameters.AddWithValue("@VoucherId", voucherId);
            insertCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        public List<Voucher> GetCartVouchers(Guid cartId)
        {
            sqlConnection.Open();
            List<Voucher> vouchers = new List<Voucher>();
            string selectQuery = "SELECT v.Id, v.Code, v.Type, v.DiscountAmount, v.SellerId FROM Vouchers v JOIN CartVouchers cv ON v.Id = cv.VoucherId WHERE cv.CartId = @CartId";
            SqlCommand cmd = new SqlCommand(selectQuery, sqlConnection);
            cmd.Parameters.AddWithValue("@CartId", cartId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                vouchers.Add(new Voucher
                {
                    Id = Guid.Parse(reader["Id"].ToString()),
                    Code = reader["Code"].ToString(),
                    Type = (VoucherType)Enum.Parse(typeof(VoucherType), reader["Type"].ToString()),
                    DiscountAmount = decimal.Parse(reader["DiscountAmount"].ToString()),
                    SellerId = reader["SellerId"] != DBNull.Value ? Guid.Parse(reader["SellerId"].ToString()) : (Guid?)null
                });
            }
            reader.Close();
            sqlConnection.Close();
            return vouchers;
        }
    }
}
