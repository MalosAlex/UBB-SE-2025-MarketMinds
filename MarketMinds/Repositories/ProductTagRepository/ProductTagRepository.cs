using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DomainLayer.Domain;
using Microsoft.Data.SqlClient;

namespace MarketMinds.Repositories.ProductTagRepository
{
    public class ProductTagRepository : IProductTagRepository
    {
        private const int DEFAULTID = -1;
        private DataBaseConnection connection;

        public ProductTagRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public List<ProductTag> GetAllProductTags()
        {
            // Returns all the product tags
            // output: all the product tags
            List<ProductTag> productTags = new List<ProductTag>();
            string query = "SELECT * FROM ProductTags";
            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productTags.Add(new ProductTag(
                            reader.GetInt32(0),
                            reader.GetString(1)));
                    }
                }
            }
            connection.CloseConnection();
            return productTags;
        }

        public ProductTag CreateProductTag(string displayTitle)
        {
            // creates a new product tag
            // input: displayTitle
            // output: the created product tag
            int newId = DEFAULTID;

            string cmd = "INSERT INTO ProductTags (title) VALUES (@displayTitle); SELECT CAST(SCOPE_IDENTITY() as int);";
            connection.OpenConnection();

            using (SqlCommand command = new SqlCommand(cmd, connection.GetConnection()))
            {
                command.Parameters.AddWithValue("@displayTitle", displayTitle);
                newId = (int)command.ExecuteScalar();
            }
            connection.CloseConnection();

            /*
            if (newId == -1)
            {
                throw new Exception("Error creating product tag");
            }
            */
            return new ProductTag(newId, displayTitle);
        }

        public void DeleteProductTag(string displayTitle)
        {
            // deletes a product tag
            // input: displayTitle
            // output: none
            string cmd = "DELETE FROM ProductTags WHERE title = @displayTitle";
            connection.OpenConnection();
            using (SqlCommand command = new SqlCommand(cmd, connection.GetConnection()))
            {
                command.Parameters.AddWithValue("@displayTitle", displayTitle);
                command.ExecuteNonQuery();
            }
        }
    }
}
