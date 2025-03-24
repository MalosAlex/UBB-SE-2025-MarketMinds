using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class BuyProductsRepository : ProductsRepository
    {
        private DataBaseConnection connection;

        public BuyProductsRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public void SaveBuyProduct(BuyProduct product)
        {
            throw new NotImplementedException();
        }

        public void DeleteBuyProduct(BuyProduct product)
        {
            string query = "DELETE FROM BuyProducts WHERE id = @Id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@Id", product.Id);

                cmd.ExecuteNonQuery();
            }
        }

        private List<ProductTag> GetProductTags(int productId)
        {
            var tags = new List<ProductTag>();

            string query = @"
                        SELECT pt.id, pt.title
                        FROM ProductTags pt
                        INNER JOIN BuyProductProductTags bpt ON pt.id = bpt.tag_id
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

        public List<Image> GetProductImages(int productId)
        {
            return null;
        }

        public BorrowProduct GetBuyProductByID(int productId)
        {
            return null;
        }

        public List<BorrowProduct> GetAllBuyProducts()
        {
            return null;
        }
    }
}
