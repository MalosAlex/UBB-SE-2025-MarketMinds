using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DomainLayer.Domain;
using Microsoft.Data.SqlClient;

namespace MarketMinds.Repositories.ProductCategoryRepository
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private const int DEFAULTID = -1;
        private DataBaseConnection connection;

        public ProductCategoryRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public List<ProductCategory> GetAllProductCategories()
        {
            // Returns all the product categories
            // output: all the product categories
            List<ProductCategory> productCategories = new List<ProductCategory>();
            string query = "SELECT * FROM ProductCategories";
            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productCategories.Add(new ProductCategory(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2)));
                    }
                }
            }
            connection.CloseConnection();
            return productCategories;
        }

        public ProductCategory CreateProductCategory(string displayTitle, string description)
        {
            // Creates a new product category
            // input: displayTitle, description
            // output: the created product tag
            int newId = DEFAULTID;

            string cmd = "INSERT INTO ProductCategories (title, description) VALUES (@displayTitle, @description); SELECT CAST(SCOPE_IDENTITY() as int);";
            connection.OpenConnection();

            using (SqlCommand command = new SqlCommand(cmd, connection.GetConnection()))
            {
                command.Parameters.AddWithValue("@displayTitle", displayTitle);
                command.Parameters.AddWithValue("@description", description);
                newId = (int)command.ExecuteScalar();
            }
            connection.CloseConnection();

            return new ProductCategory(newId, displayTitle, description);
        }

        public void DeleteProductCategory(string displayTitle)
        {
            // Deletes a product category
            // input: displayTitle
            // output: none
            string cmd = "DELETE FROM ProductCategories WHERE title = @displayTitle";
            connection.OpenConnection();
            using (SqlCommand command = new SqlCommand(cmd, connection.GetConnection()))
            {
                command.Parameters.AddWithValue("@displayTitle", displayTitle);
                command.ExecuteNonQuery();
            }
        }
    }
}
