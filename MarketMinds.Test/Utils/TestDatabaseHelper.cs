using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using DataAccessLayer;

namespace MarketMinds.Test.Utils
{
    public class TestDatabaseHelper
    {
        private readonly DataBaseConnection _connection;
        private readonly IConfiguration _configuration;

        public TestDatabaseHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new DataBaseConnection(configuration);
        }


        public static IConfiguration CreateTestConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }


        private void ResetTestDatabase()
        {
            _connection.OpenConnection();

            try
            {
                ExecuteNonQuery("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");

                ClearTable("Bids");
                ClearTable("AuctionProductProductTags");
                ClearTable("AuctionProductsImages");
                ClearTable("AuctionProducts");

                ClearTable("BorrowProductProductTags");
                ClearTable("BorrowProductImages");
                ClearTable("BorrowProducts");

                ClearTable("BuyProductProductTags");
                ClearTable("BuyProductImages");
                ClearTable("BuyProducts");

                ClearTable("BasketItemsBuyProducts");
                ClearTable("Baskets");

                ClearTable("ReviewsPictures");
                ClearTable("Reviews");

                ClearTable("ProductTags");
                ClearTable("ProductCategories");
                ClearTable("ProductConditions");
                ClearTable("Users");

                ExecuteNonQuery("EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");
            }
            finally
            {
                _connection.CloseConnection();
            }
        }


        private void ClearTable(string tableName)
        {
            try
            {
                ExecuteNonQuery($"DELETE FROM {tableName}");

                // Reset the identity counter to 0 (SQL Server will start with 1)
                ExecuteNonQuery($"IF EXISTS (SELECT * FROM sys.identity_columns WHERE OBJECT_NAME(OBJECT_ID) = '{tableName}') " +
                             $"DBCC CHECKIDENT ('{tableName}', RESEED, 0)");
            }
            catch (Exception)
            {
                // If table doesn't exist, just continue
            }
        }


        private void ExecuteNonQuery(string commandText)
        {
            using (SqlCommand command = new SqlCommand(commandText, _connection.GetConnection()))
            {
                command.ExecuteNonQuery();
            }
        }

        private void SeedTestData()
        {
            _connection.OpenConnection();

            try
            {
                // Add a test user
                ExecuteNonQuery(@"
                    INSERT INTO Users (email, username, userType, balance, rating, passwordHash)
                    VALUES ('alice@example.com', 'alice123', 1, 500.00, 4.5, 'hashedpassword1'),
                           ('bob@example.com', 'bob321', 2, 1000.00, 4.8, 'hashedpassword2');");

                // Add product conditions
                ExecuteNonQuery(@"
                    INSERT INTO ProductConditions (title, description)
                    VALUES ('New', 'Brand new item, unopened'),
                        ('Used', 'Item has been previously used');");

                // Add product categories
                ExecuteNonQuery(@"
                    INSERT INTO ProductCategories (title, description)
                    VALUES ('Electronics', 'Devices like phones, laptops, etc.'),
                           ('Furniture', 'Chairs, tables, beds, etc.');");

                // Add product tags
                ExecuteNonQuery(@"
                    INSERT INTO ProductTags (title)
                    VALUES ('Tech'),
                           ('Home'),
                           ('Vintage');");
            }
            finally
            {
                _connection.CloseConnection();
            }
        }


        public void PrepareTestDatabase()
        {
            ResetTestDatabase();
            SeedTestData();
        }
    }
}