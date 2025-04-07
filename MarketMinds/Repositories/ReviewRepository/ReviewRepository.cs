using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DataAccessLayer;
using DomainLayer.Domain;
using Microsoft.Data.SqlClient;

namespace MarketMinds.Repositories.ReviewRepository
{
    public class ReviewRepository : IReviewRepository
    {
        private const int DEFAULTREVIEWID = -1;
        private const int NOCOUNT = 0;
        private const int NULLRATING = 0;
        private DataBaseConnection connection;
        public ReviewRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public ObservableCollection<Review> GetAllReviewsByBuyer(User buyer)
        {
            ObservableCollection<Review> reviews = new ObservableCollection<Review>();
            string query = "SELECT * FROM Reviews WHERE reviewer_id = @id";

            connection.OpenConnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@id", buyer.Id);

                    ObservableCollection<int> reviewIds = new ObservableCollection<int>();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int reviewId = reader.GetInt32(0);
                            reviewIds.Add(reviewId);

                            // Add the review directly to the ObservableCollection
                            var review = new Review(
                                reviewId,
                                reader.GetString(3),
                                new List<Image>(), // Use ObservableCollection for images
                                Convert.ToSingle(reader.GetDouble(4)),
                                reader.GetInt32(2),
                                reader.GetInt32(1));
                            reviews.Add(review);
                        }
                    }

                    // Load images for each review after reading the reviews
                    foreach (var review in reviews)
                    {
                        review.Images = new List<Image>(GetImages(review.Id)); // Convert List<Image> to ObservableCollection<Image>
                    }
                }
            }
            finally
            {
                connection.CloseConnection();
            }

            return reviews;
        }

        public ObservableCollection<Review> GetAllReviewsBySeller(User seller)
        {
            ObservableCollection<Review> reviews = new ObservableCollection<Review>();
            string query = "SELECT * FROM Reviews WHERE seller_id = @id";

            connection.OpenConnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@id", seller.Id);

                    ObservableCollection<int> reviewIds = new ObservableCollection<int>();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int reviewId = reader.GetInt32(0);
                            reviewIds.Add(reviewId);

                            // Add the review directly to the ObservableCollection
                            var review = new Review(
                                reviewId,
                                reader.GetString(3),
                                new List<Image>(), // Use ObservableCollection for images
                                Convert.ToSingle(reader.GetDouble(4)),
                                reader.GetInt32(1),
                                reader.GetInt32(2));
                            reviews.Add(review);
                        }
                    }

                    // Load images for each review after reading the reviews
                    foreach (var review in reviews)
                    {
                        review.Images = new List<Image>(GetImages(review.Id)); // Convert List<Image> to ObservableCollection<Image>
                    }
                }
            }
            finally
            {
                connection.CloseConnection();
            }
            return reviews;
        }

        public void CreateReview(Review review)
        {
            string insertSql = "INSERT INTO Reviews (reviewer_id, seller_id, description, rating) OUTPUT INSERTED.id VALUES (@reviewer_id, @seller_id, @description, @rating)";
            if (review.Description == null)
            {
                review.Description = string.Empty;
            }
            connection.OpenConnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(insertSql, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@reviewer_id", review.BuyerId);
                    cmd.Parameters.AddWithValue("@seller_id", review.SellerId);
                    cmd.Parameters.AddWithValue("@description", review.Description);
                    cmd.Parameters.AddWithValue("@rating", review.Rating);

                    review.Id = (int)cmd.ExecuteScalar();
                }

                // Insert images if they exist
                if (review.Images != null && review.Images.Count > NOCOUNT)
                {
                    string insertImageSql = "INSERT INTO ReviewsPictures (url, review_id) VALUES (@Url, @review_id)";
                    foreach (var img in review.Images)
                    {
                        using (SqlCommand cmd = new SqlCommand(insertImageSql, connection.GetConnection()))
                        {
                            cmd.Parameters.AddWithValue("@Url", img.Url);
                            cmd.Parameters.AddWithValue("@review_id", review.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            finally
            {
                connection.CloseConnection();
            }
        }

        private List<Image> GetImages(int reviewId)
        {
            List<Image> images = new List<Image>();
            string query = "SELECT * FROM ReviewsPictures WHERE review_id = @id";

            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", reviewId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        images.Add(new Image(reader.GetString(1)));
                    }
                }
            }
            return images;
        }

        public void EditReview(Review review, float rating, string description)
        {
            if (review.Id == DEFAULTREVIEWID)
            {
                review.Id = GetReviewId(review);
            }

            if (rating != NULLRATING)
            {
                string updateQuery = "UPDATE Reviews SET rating = @rating WHERE id = @id";
                connection.OpenConnection();
                Debug.WriteLine("Connected to: " + connection.GetConnection().ServerVersion);
                using (SqlCommand cmd = new SqlCommand(updateQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@id", review.Id);
                    cmd.Parameters.AddWithValue("@rating", rating);
                    cmd.ExecuteNonQuery();
                }
                connection.CloseConnection();
            }

            if (!string.IsNullOrEmpty(description))
            {
                string updateQuery = "UPDATE Reviews SET description = @description WHERE id = @id";
                connection.OpenConnection();
                Debug.WriteLine("Connected to: " + connection.GetConnection().ServerVersion);
                using (SqlCommand cmd = new SqlCommand(updateQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@id", review.Id);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.ExecuteNonQuery();
                }
                connection.CloseConnection();
            }
        }
        public void DeleteReview(Review review)
        {
            if (review.Id == DEFAULTREVIEWID)
            {
                review.Id = GetReviewId(review);
            }
            connection.OpenConnection();
            DeleteImages(review.Id);

            string query = "DELETE FROM Reviews WHERE id = @id";
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", review.Id);
                cmd.ExecuteNonQuery();
            }
            connection.CloseConnection();
        }

        private int GetReviewId(Review review)
        {
            string query = "SELECT id FROM Reviews WHERE reviewer_id = @reviewer AND seller_id = @seller AND description = @desc";
            int id = DEFAULTREVIEWID;

            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@reviewer", review.BuyerId);
                cmd.Parameters.AddWithValue("@seller", review.SellerId);
                cmd.Parameters.AddWithValue("@desc", review.Description);
                connection.OpenConnection();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }
                }
            }
            connection.CloseConnection();
            return id;
        }

        private void DeleteImages(int reviewId)
        {
            string query = "DELETE FROM ReviewsPictures WHERE review_id = @id";
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", reviewId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
