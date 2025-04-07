using System.Collections.Generic;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using DomainLayer.Domain;
using DataAccessLayer;

namespace MarketMinds.Repositories.BuyProductsRepository
{
    public class BuyProductsRepository : IBuyProductsRepository
    {
        private DataBaseConnection connection;

        public BuyProductsRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public void AddProduct(Product product)
        {
            BuyProduct buy = (BuyProduct)product;
            if (buy == null)
            {
                throw new ArgumentException("Product must be of type BuyProduct.");
            }

            string insertProductQuery = @"
            INSERT INTO BuyProducts 
            (title, description, seller_id, condition_id, category_id, price)
            VALUES 
            (@Title, @Description, @SellerId, @ConditionId, @CategoryId, @Price);
            SELECT SCOPE_IDENTITY();";

            connection.OpenConnection();

            int newProductId;
            using (SqlCommand cmd = new SqlCommand(insertProductQuery, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@Title", buy.Title);
                cmd.Parameters.AddWithValue("@Description", buy.Description);
                cmd.Parameters.AddWithValue("@SellerId", buy.Seller.Id);
                cmd.Parameters.AddWithValue("@ConditionId", buy.Condition.Id);
                cmd.Parameters.AddWithValue("@CategoryId", buy.Category.Id);
                cmd.Parameters.AddWithValue("@Price", buy.Price);

                object result = cmd.ExecuteScalar();
                newProductId = Convert.ToInt32(result);
            }

            foreach (var tag in buy.Tags)
            {
                string insertTagQuery = @"
            INSERT INTO BuyProductProductTags (product_id, tag_id)
            VALUES (@ProductId, @TagId)";

                using (SqlCommand cmd = new SqlCommand(insertTagQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@ProductId", newProductId);
                    cmd.Parameters.AddWithValue("@TagId", tag.Id);
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (var image in buy.Images)
            {
                string insertImageQuery = @"
            INSERT INTO BuyProductImages (url, product_id)
            VALUES (@Url, @ProductId)";

                using (SqlCommand cmd = new SqlCommand(insertImageQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Url", image.Url);
                    cmd.Parameters.AddWithValue("@ProductId", newProductId);
                    cmd.ExecuteNonQuery();
                }
            }

            connection.CloseConnection();
        }

        public void DeleteProduct(Product product)
        {
            string query = "DELETE FROM BuyProducts WHERE id = @Id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@Id", product.Id);

                cmd.ExecuteNonQuery();
            }
            connection.CloseConnection();
        }

        public List<Product> GetProducts()
        {
            List<Product> buys = new List<Product>();
            DataTable productsTable = new DataTable();

            string mainQuery = @"
        SELECT 
            bp.id,
            bp.title,
            bp.description,
            bp.seller_id,
            u.username,
            u.email,
            bp.condition_id,
            pc.title AS conditionTitle,
            pc.description AS conditionDescription,
            bp.category_id,
            cat.title AS categoryTitle,
            cat.description AS categoryDescription,
            bp.price
        FROM BuyProducts bp
        JOIN Users u ON bp.seller_id = u.id
        JOIN ProductConditions pc ON bp.condition_id = pc.id
        JOIN ProductCategories cat ON bp.category_id = cat.id";

            connection.OpenConnection();

            // Step 1: Load all products first
            using (SqlCommand cmd = new SqlCommand(mainQuery, connection.GetConnection()))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                productsTable.Load(reader); // Load into DataTable to close the reader early
            }

            // Step 2: Process each product to fetch tags and images
            foreach (DataRow row in productsTable.Rows)
            {
                int id = (int)row["id"];
                string title = (string)row["title"];
                string description = (string)row["description"];

                int sellerId = (int)row["seller_id"];
                string username = (string)row["username"];
                string email = (string)row["email"];
                User seller = new User(sellerId, username, email);

                int conditionId = (int)row["condition_id"];
                string conditionTitle = (string)row["conditionTitle"];
                string conditionDescription = (string)row["conditionDescription"];
                ProductCondition condition = new ProductCondition(conditionId, conditionTitle, conditionDescription);

                int categoryId = (int)row["category_id"];
                string categoryTitle = (string)row["categoryTitle"];
                string categoryDescription = (string)row["categoryDescription"];
                ProductCategory category = new ProductCategory(categoryId, categoryTitle, categoryDescription);

                double priceDouble = (double)row["price"];
                float price = (float)priceDouble;

                // Fetch tags and images in separate queries
                List<ProductTag> tags = GetProductTags(id);
                List<Image> images = GetProductImages(id);

                BuyProduct buy = new BuyProduct(
                    id, title, description, seller, condition,
                    category, tags, images, price);

                buys.Add(buy);
            }

            connection.CloseConnection();
            return buys;
        }

        private List<ProductTag> GetProductTags(int productId)
        {
            var tags = new List<ProductTag>();

            string query = @"
        SELECT pt.id, pt.title
        FROM ProductTags pt
        INNER JOIN BuyProductProductTags bpt ON pt.id = bpt.tag_id
        WHERE bpt.product_id = @ProductId"; // Fixed typo: was 'apt' instead of 'bpt'

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int tagId = reader.GetInt32(reader.GetOrdinal("id"));
                        string tagTitle = reader.GetString(reader.GetOrdinal("title"));
                        tags.Add(new ProductTag(tagId, tagTitle));
                    }
                }
            }
            connection.CloseConnection();
            return tags;
        }

        private List<Image> GetProductImages(int productId)
        {
            var images = new List<Image>();

            string query = @"
        SELECT url
        FROM BuyProductImages
        WHERE product_id = @ProductId";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string url = reader.GetString(reader.GetOrdinal("url"));
                        images.Add(new Image(url));
                    }
                }
            }
            connection.CloseConnection();
            return images;
        }

        public Product GetProductByID(int productId)
        {
            BuyProduct buy = null;
            string query = @"
                            SELECT 
                                bp.id,
                                bp.title,
                                bp.description,
                                bp.seller_id,
                                u.username,
                                u.email,
                                bp.condition_id,
                                pc.title AS conditionTitle,
                                pc.description AS conditionDescription,
                                bp.category_id,
                                cat.title AS categoryTitle,
                                cat.description AS categoryDescription,
                                bp.price
                            FROM BuyProducts bp
                            JOIN Users u ON bp.seller_id = u.id
                            JOIN ProductConditions pc ON bp.condition_id = pc.id
                            JOIN ProductCategories cat ON bp.category_id = cat.id
                            WHERE bp.id = @productID";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@productID", productId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        string title = reader.GetString(reader.GetOrdinal("title"));
                        string description = reader.GetString(reader.GetOrdinal("description"));

                        int sellerId = reader.GetInt32(reader.GetOrdinal("seller_id"));
                        string username = reader.GetString(reader.GetOrdinal("username"));
                        string email = reader.GetString(reader.GetOrdinal("email"));
                        User seller = new User(sellerId, username, email);

                        int conditionId = reader.GetInt32(reader.GetOrdinal("condition_id"));
                        string conditionTitle = reader.GetString(reader.GetOrdinal("conditionTitle"));
                        string conditionDescription = reader.GetString(reader.GetOrdinal("conditionDescription"));
                        ProductCondition condition = new ProductCondition(conditionId, conditionTitle, conditionDescription);

                        int categoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
                        string categoryTitle = reader.GetString(reader.GetOrdinal("categoryTitle"));
                        string categoryDescription = reader.GetString(reader.GetOrdinal("categoryDescription"));
                        ProductCategory category = new ProductCategory(categoryId, categoryTitle, categoryDescription);

                        float price = (float)reader.GetDouble(reader.GetOrdinal("price"));

                        reader.Close();

                        List<ProductTag> tags = GetProductTags(id);
                        List<Image> images = GetProductImages(id);

                        buy = new BuyProduct(
                            id,
                            title,
                            description,
                            seller,
                            condition,
                            category,
                            tags,
                            images,
                            price);
                    }
                }
            }
            connection.CloseConnection();
            return buy;
        }
        public void UpdateProduct(Product product)
        {
            // Implementation goes here
        }
    }
}
