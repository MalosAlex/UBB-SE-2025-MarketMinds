using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DomainLayer.Domain;
using Microsoft.Data.SqlClient;

namespace MarketMinds.Repositories.BorrowProductsRepository
{
    public class BorrowProductsRepository : IBorrowProductsRepository
    {
        private DataBaseConnection connection;

        public BorrowProductsRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public void AddProduct(Product product)
        {
            BorrowProduct borrow = (BorrowProduct)product;
            if (borrow == null)
            {
                throw new ArgumentException("Product must be of type BorrowProduct.");
            }

            string insertProductQuery = @"
            INSERT INTO BorrowProducts 
            (title, description, seller_id, condition_id, category_id, time_limit, start_date, end_date, daily_rate, is_borrowed)
            VALUES 
            (@Title, @Description, @SellerId, @ConditionId, @CategoryId, @TimeLimit, @StartDate, @EndDate, @DailyRate, @IsBorrowed);
            SELECT SCOPE_IDENTITY();";

            connection.OpenConnection();

            int newProductId;
            using (SqlCommand cmd = new SqlCommand(insertProductQuery, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@Title", borrow.Title);
                cmd.Parameters.AddWithValue("@Description", borrow.Description);
                cmd.Parameters.AddWithValue("@SellerId", borrow.Seller.Id);
                cmd.Parameters.AddWithValue("@ConditionId", borrow.Condition.Id);
                cmd.Parameters.AddWithValue("@CategoryId", borrow.Category.Id);
                cmd.Parameters.AddWithValue("@TimeLimit", borrow.TimeLimit);
                cmd.Parameters.AddWithValue("@StartDate", borrow.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", borrow.EndDate);
                cmd.Parameters.AddWithValue("@DailyRate", borrow.DailyRate);
                cmd.Parameters.AddWithValue("@IsBorrowed", borrow.IsBorrowed);

                object result = cmd.ExecuteScalar();
                newProductId = Convert.ToInt32(result);
            }

            foreach (var tag in borrow.Tags)
            {
                string insertTagQuery = @"
            INSERT INTO BorrowProductProductTags (product_id, tag_id)
            VALUES (@ProductId, @TagId)";

                using (SqlCommand cmd = new SqlCommand(insertTagQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@ProductId", newProductId);
                    cmd.Parameters.AddWithValue("@TagId", tag.Id);
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (var image in borrow.Images)
            {
                string insertImageQuery = @"
            INSERT INTO BorrowProductImages (url, product_id)
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
            int id = product.Id;
            string query = "DELETE FROM BorrowProducts WHERE id = @id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", product.Id);

                cmd.ExecuteNonQuery();
            }
            connection.CloseConnection();
        }
        public List<Product> GetProducts()
        {
            var borrows = new List<Product>();
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
            bp.daily_rate,
            bp.time_limit,
            bp.start_date,
            bp.end_date,
            bp.is_borrowed
        FROM BorrowProducts bp
        JOIN Users u ON bp.seller_id = u.id
        JOIN ProductConditions pc ON bp.condition_id = pc.id
        JOIN ProductCategories cat ON bp.category_id = cat.id";

            connection.OpenConnection();

            using (SqlCommand cmd = new SqlCommand(mainQuery, connection.GetConnection()))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                productsTable.Load(reader);
            }

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

                double dailyRateDouble = (double)row["daily_rate"];
                float dailyRate = (float)dailyRateDouble;

                DateTime timeLimit = (DateTime)row["time_limit"];
                DateTime startDate = (DateTime)row["start_date"];
                DateTime endDate = (DateTime)row["end_date"];
                bool isBorrowed = (bool)row["is_borrowed"];

                List<ProductTag> tags = GetProductTags(id);
                List<Image> images = GetProductImages(id);
                BorrowProduct borrow = new BorrowProduct(
                    id, title, description, seller, condition,
                    category, tags, images, timeLimit,
                    startDate, endDate, dailyRate, isBorrowed);
                borrows.Add(borrow);
            }
            connection.CloseConnection();
            return borrows;
        }
        private List<ProductTag> GetProductTags(int productId)
        {
            var tags = new List<ProductTag>();

            string query = @"
        SELECT pt.id, pt.title
        FROM ProductTags pt
        INNER JOIN BorrowProductProductTags bpt ON pt.id = bpt.tag_id
        WHERE bpt.product_id = @ProductId";

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
        FROM BorrowProductImages
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
            BorrowProduct? borrow = null;

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
                bp.daily_rate,
                bp.time_limit,
                bp.start_date,
                bp.end_date,
                bp.is_borrowed
            FROM BorrowProducts bp
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

                        float dailyRate = (float)reader.GetDouble(reader.GetOrdinal("daily_rate"));

                        DateTime timeLimit = reader.GetDateTime(reader.GetOrdinal("time_limit"));
                        DateTime startDate = reader.GetDateTime(reader.GetOrdinal("start_date"));
                        DateTime endDate = reader.GetDateTime(reader.GetOrdinal("end_date"));
                        bool isBorrowed = reader.GetBoolean(reader.GetOrdinal("is_borrowed"));

                        reader.Close();

                        List<ProductTag> tags = GetProductTags(id);
                        List<Image> images = GetProductImages(id);

                        borrow = new BorrowProduct(
                            id,
                            title,
                            description,
                            seller,
                            condition,
                            category,
                            tags,
                            images,
                            timeLimit,
                            startDate,
                            endDate,
                            dailyRate,
                            isBorrowed);
                    }
                }
            }
            connection.CloseConnection();
            return borrow!;
        }

        public void UpdateProduct(Product product)
        {
            // Update the product in the database
        }
    }
}