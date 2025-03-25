using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer.Repositories
{
    public class BorrowProductsRepository : ProductsRepository
    {
        private DataBaseConnection connection;

        public BorrowProductsRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public void SaveBorrowProduct(BorrowProduct product)
        {
            BorrowProduct borrow = product;
            if (borrow == null)
                throw new ArgumentException("Product must be of type BorrowProduct.");


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
                cmd.Parameters.AddWithValue("@SellerId", borrow.Seller.id);
                cmd.Parameters.AddWithValue("@ConditionId", borrow.Condition.id);
                cmd.Parameters.AddWithValue("@CategoryId", borrow.Category.id);
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
                    cmd.Parameters.AddWithValue("@TagId", tag.id);
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (var image in borrow.Images)
            {
                string insertImageQuery = @"
            INSERT INTO BorrowProductsImages (url, product_id)
            VALUES (@Url, @ProductId)";

                using (SqlCommand cmd = new SqlCommand(insertImageQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Url", image.url);
                    cmd.Parameters.AddWithValue("@ProductId", newProductId);
                    cmd.ExecuteNonQuery();
                }
            }

            connection.CloseConnection();
        }

        public void DeleteBorrowProduct(BorrowProduct product)
        {
            int Id = product.Id;
            string query = "DELETE FROM BorrowProducts WHERE id = @Id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@Id", product.id);

                cmd.ExecuteNonQuery();
            }
        }

        private List<ProductTag> GetProductTags(int productId)
        {
            var tags = new List<ProductTag>();

            string query = @"
                        SELECT pt.id, pt.title
                        FROM ProductTags pt
                        INNER JOIN BorrowProductProductTags bpt ON pt.id = bpt.tag_id
                        WHERE apt.product_id = @ProductId";

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

            return tags;
        }

        private List<Image> GetProductImages(int productId)
        {
            var images = new List<Image>();

            string query = @"
            SELECT url
            FROM BorrowProductsImages
            WHERE product_id = @ProductId";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int imageId = reader.GetInt32(reader.GetOrdinal("id"));
                        string url = reader.GetString(reader.GetOrdinal("url"));

                        images.Add(new Image(url));
                    }
                }
            }
            connection.CloseConnection();

            return images;
        }

        public BorrowProduct GetBorrowProductByID(int productId)
        {
            BorrowProduct borrow = new BorrowProduct();

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
                using (SqlDataReader reader = cmd.ExecuteReader())
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

                        float dailyRate = reader.GetFloat(reader.GetOrdinal("daily_rate"));

                        DateTime timeLimit = reader.GetDateTime(reader.GetOrdinal("time_limit"));
                        DateTime startDate = reader.GetDateTime(reader.GetOrdinal("start_date"));
                        DateTime endDate = reader.GetDateTime(reader.GetOrdinal("end_date"));
                        bool isBorrowed = reader.GetBoolean(reader.GetOrdinal("is_borrowed"));

                        List<ProductTag> tags = GetProductTags(id);

                        List<Image> images = GetProductImages(id);

                        borrow = BorrowProduct(
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
                            isBorrowed
                        );
                }
            }
            connection.CloseConnection();
            return borrow;
        }

        public List<Product> GetAllProducts()
        {
            var borrows = new List<Product>();

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
                            JOIN ProductCategories cat ON bp.category_id = cat.id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
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

                        float dailyRate = reader.GetFloat(reader.GetOrdinal("daily_rate"));

                        DateTime timeLimit = reader.GetDateTime(reader.GetOrdinal("time_limit"));
                        DateTime startDate = reader.GetDateTime(reader.GetOrdinal("start_date"));
                        DateTime endDate = reader.GetDateTime(reader.GetOrdinal("end_date"));
                        bool isBorrowed = reader.GetBoolean(reader.GetOrdinal("is_borrowed"));

                        List<ProductTag> tags = GetProductTags(id);

                        List<Image> images = GetProductImages(id);

                        BorrowProduct borrow = new BorrowProduct(
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
                            isBorrowed
                        );

                        borrows.Add(borrow);
                    }
                }
            }
            connection.CloseConnection();
            return borrows;
        }
        
        public override void UpdateProduct(Product product)
        {
            
        }

        public override void DeleteProduct(Product product)
        {
            
        }
    }
}