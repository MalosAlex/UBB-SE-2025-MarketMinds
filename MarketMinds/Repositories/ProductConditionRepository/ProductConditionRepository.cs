using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DomainLayer.Domain;
using Microsoft.Data.SqlClient;

namespace MarketMinds.Repositories.ProductConditionRepository
{
    public class ProductConditionRepository : IProductConditionRepository
    {
        private DataBaseConnection connection;
        private const int DEFAULTID = -1;

        public ProductConditionRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public List<ProductCondition> GetAllProductConditions()
        {
            // Returns all the product conditions
            // output: all the product conditions
            List<ProductCondition> productConditions = new List<ProductCondition>();
            string query = "SELECT * FROM ProductConditions";
            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productConditions.Add(new ProductCondition(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2)));
                    }
                }
            }
            connection.CloseConnection();
            return productConditions;
        }

        public ProductCondition CreateProductCondition(string displayTitle, string description)
        {
            // creates a new product condition
            // input: displayTitle, description
            // output: the created product condition
            int newId = DEFAULTID;

            string cmd = "INSERT INTO ProductConditions (title, description) VALUES (@displayTitle, @description); SELECT CAST(SCOPE_IDENTITY() as int);";
            connection.OpenConnection();

            using (SqlCommand command = new SqlCommand(cmd, connection.GetConnection()))
            {
                command.Parameters.AddWithValue("@displayTitle", displayTitle);
                command.Parameters.AddWithValue("@description", description);
                newId = (int)command.ExecuteScalar();
            }
            connection.CloseConnection();

            /*
            if (newId == -1)
            {
                throw new Exception("Product condition was not created");
            }
            */
            return new ProductCondition(newId, displayTitle, description);
        }

        public void DeleteProductCondition(string displayTitle)
        {
            // deletes a product condition
            // input: displayTitle
            // output: none
            string cmd = "DELETE FROM ProductConditions WHERE title = @displayTitle";
            connection.OpenConnection();
            using (SqlCommand command = new SqlCommand(cmd, connection.GetConnection()))
            {
                command.Parameters.AddWithValue("@displayTitle", displayTitle);
                command.ExecuteNonQuery();
            }
        }
    }
}
