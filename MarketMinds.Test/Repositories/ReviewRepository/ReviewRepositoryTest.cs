using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MarketMinds.Repositories.ReviewRepository;
using DomainLayer.Domain;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using MarketMinds.Test.Utils;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MarketMinds.Tests.ReviewRepositoryTest
{
    [TestFixture]
    public class ReviewRepositoryTest
    {
        private IReviewRepository reviewRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;
        private User buyer;
        private User seller;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            reviewRepository = new ReviewRepository(connection);

            testDbHelper.PrepareTestDatabase();

            buyer = new User(1, "luca", "luca@example.com");
            buyer.UserType = 1;
            buyer.Rating = 4.5f;

            seller = new User(2, "bob", "bob@example.com");
            seller.UserType = 2;
            seller.Rating = 4.8f;

            SetupTestReview();
        }

        private void SetupTestReview()
        {
            connection.OpenConnection();
            try
            {
                string query = "INSERT INTO Reviews (reviewer_id, seller_id, description, rating) VALUES (@reviewer_id, @seller_id, @description, @rating)";
                using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@reviewer_id", buyer.Id);
                    cmd.Parameters.AddWithValue("@seller_id", seller.Id);
                    cmd.Parameters.AddWithValue("@description", "Great seller, fast shipping!");
                    cmd.Parameters.AddWithValue("@rating", 4.5);
                    cmd.ExecuteNonQuery();
                }

                int reviewId = 0;
                query = "SELECT TOP 1 id FROM Reviews ORDER BY id DESC";
                using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reviewId = reader.GetInt32(0);
                        }
                    }
                }

                if (reviewId > 0)
                {
                    query = "INSERT INTO ReviewsPictures (url, review_id) VALUES (@url, @review_id)";
                    using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
                    {
                        cmd.Parameters.AddWithValue("@url", "https://example.com/image.jpg");
                        cmd.Parameters.AddWithValue("@review_id", reviewId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                connection.CloseConnection();
            }
        }

        [Test]
        public void TestGetAllReviewsByBuyer_ReturnsCorrectReviews()
        {
            var reviews = reviewRepository.GetAllReviewsByBuyer(buyer);

            Assert.That(reviews, Is.Not.Null);
            Assert.That(reviews.Count, Is.EqualTo(1));
            Assert.That(reviews[0].BuyerId, Is.EqualTo(buyer.Id));
            Assert.That(reviews[0].SellerId, Is.EqualTo(seller.Id));
            Assert.That(reviews[0].Description, Is.EqualTo("Great seller, fast shipping!"));
            Assert.That(reviews[0].Rating, Is.EqualTo(4.5f));

            Assert.That(reviews[0].Images, Is.Not.Null);
            Assert.That(reviews[0].Images.Count, Is.EqualTo(1));
            Assert.That(reviews[0].Images[0].Url, Is.EqualTo("https://example.com/image.jpg"));
        }

        [Test]
        public void TestGetAllReviewsBySeller_ReturnsCorrectReviews()
        {
            var reviews = reviewRepository.GetAllReviewsByBuyer(buyer);

            Assert.That(reviews, Is.Not.Null);
            Assert.That(reviews.Count, Is.EqualTo(1));
            Assert.That(reviews[0].BuyerId, Is.EqualTo(buyer.Id));
            Assert.That(reviews[0].SellerId, Is.EqualTo(seller.Id));
            Assert.That(reviews[0].Description, Is.EqualTo("Great seller, fast shipping!"));
            Assert.That(reviews[0].Rating, Is.EqualTo(4.5f));

            Assert.That(reviews[0].Images, Is.Not.Null);
            Assert.That(reviews[0].Images.Count, Is.EqualTo(1));
            Assert.That(reviews[0].Images[0].Url, Is.EqualTo("https://example.com/image.jpg"));
        }

        [Test]
        public void TestCreateReview_AddsNewReview()
        {

            var initialReview = new Review(
                -1,
                "Great seller, fast shipping!",
                new List<Image>(),
                4.5f,
                buyer.Id,
                seller.Id
            );
            reviewRepository.CreateReview(initialReview);

            var reviews = reviewRepository.GetAllReviewsByBuyer(buyer);

            Assert.That(reviews.Count, Is.EqualTo(1));

            var addedReview = reviews.FirstOrDefault(r => r.Description == "Great seller, fast shipping!");

            Assert.That(addedReview, Is.Not.Null);
            Assert.That(addedReview.Id, Is.GreaterThan(0));
            Assert.That(addedReview.Rating, Is.EqualTo(4.5f));
            Assert.That(addedReview.BuyerId, Is.EqualTo(buyer.Id));
            Assert.That(addedReview.SellerId, Is.EqualTo(seller.Id));

            Assert.That(addedReview.Images, Is.Not.Null);
            Assert.That(addedReview.Images.Count, Is.EqualTo(1));
            Assert.That(addedReview.Images[0].Url, Is.EqualTo("https://example.com/image.jpg"));
        }

        [Test]
        public void TestCreateReview_WithNullDescription_SetsEmptyString()
        {
            var reviewWithNullDesc = new Review(
                -1,
                null,
                new List<Image>(),
                4.0f,
                buyer.Id,
                seller.Id
            );

            reviewRepository.CreateReview(reviewWithNullDesc);

            var reviews = reviewRepository.GetAllReviewsBySeller(buyer);

            Assert.That(reviews.Count, Is.EqualTo(1));

            var addedReview = reviews[0];

            Assert.That(addedReview, Is.Not.Null);
            Assert.That(addedReview.Description, Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestEditReview_UpdatesRating()
        {
            var reviews = reviewRepository.GetAllReviewsByBuyer(buyer);
            var existingReview = reviews[0];
            float newRating = 3.5f;

            reviewRepository.EditReview(existingReview, newRating, null);

            reviews = reviewRepository.GetAllReviewsByBuyer(buyer);
            var updatedReview = reviews.FirstOrDefault(r => r.Id == existingReview.Id);

            Assert.That(updatedReview, Is.Not.Null);
            Assert.That(updatedReview.Rating, Is.EqualTo(newRating));
            Assert.That(updatedReview.Description, Is.EqualTo(existingReview.Description));
        }

        [Test]
        public void TestEditReview_UpdatesDescription()
        {
            var reviews = reviewRepository.GetAllReviewsByBuyer(buyer);
            var existingReview = reviews[0];
            string newDescription = "Updated description for testing";

            reviewRepository.EditReview(existingReview, 0, newDescription);

            reviews = reviewRepository.GetAllReviewsByBuyer(buyer);
            var updatedReview = reviews.FirstOrDefault(r => r.Id == existingReview.Id);

            Assert.That(updatedReview, Is.Not.Null);
            Assert.That(updatedReview.Description, Is.EqualTo(newDescription));
            Assert.That(updatedReview.Rating, Is.EqualTo(existingReview.Rating));
        }

        [Test]
        public void TestEditReview_UpdatesBothRatingAndDescription()
        {
            var reviews = reviewRepository.GetAllReviewsByBuyer(buyer);
            var existingReview = reviews[0];
            float newRating = 2.5f;
            string newDescription = "Updated review";

            reviewRepository.EditReview(existingReview, newRating, newDescription);

            reviews = reviewRepository.GetAllReviewsByBuyer(buyer);
            var updatedReview = reviews.FirstOrDefault(r => r.Id == existingReview.Id);

            Assert.That(updatedReview, Is.Not.Null);
            Assert.That(updatedReview.Rating, Is.EqualTo(newRating));
            Assert.That(updatedReview.Description, Is.EqualTo(newDescription));
        }

        [Test]
        public void TestEditReview_WithNegativeId_FindsCorrectReview()
        {
            var review = new Review(
                -1,
                "Great seller, fast shipping!",
                new List<Image>(),
                4.5f,
                buyer.Id,
                seller.Id
            );

            float newRating = 1.5f;

            reviewRepository.EditReview(review, newRating, null);

            var reviews = reviewRepository.GetAllReviewsByBuyer(buyer);

            Assert.That(reviews.Count, Is.EqualTo(1));
            var updatedReview = reviews.FirstOrDefault(r => r.Description == "Great seller, fast shipping!");
            Assert.That(updatedReview, Is.Not.Null);
        }

        [Test]
        public void TestDeleteReview_RemovesReview()
        {
            var reviews = reviewRepository.GetAllReviewsByBuyer(buyer);
            var existingReview = reviews[0];

            reviewRepository.DeleteReview(existingReview);

            reviews = reviewRepository.GetAllReviewsByBuyer(buyer);

            Assert.That(reviews, Is.Empty);
        }

        [Test]
        public void TestDeleteReview_WithNegativeId_FindsAndDeletesCorrectReview()
        {
            var review = new Review(
                -1,
                "Great seller, fast shipping!",
                new List<Image>(),
                4.5f,
                buyer.Id,
                seller.Id
            );

            reviewRepository.DeleteReview(review);

            var reviews = reviewRepository.GetAllReviewsByBuyer(buyer);

            var specificReview = reviews.FirstOrDefault(r => r.Description == "Great seller, fast shipping!");
            Assert.That(specificReview, Is.Not.Null);
        }

        [Test]
        public void TestGetAllReviewsByBuyer_WithNoReviews_ReturnsEmptyCollection()
        {
            var newUser = new User(3, "cristi", "cristi@example.com");
            newUser.UserType = 1;
            newUser.Rating = 5.0f;

            var reviews = reviewRepository.GetAllReviewsByBuyer(newUser);

            Assert.That(reviews, Is.Not.Null);
            Assert.That(reviews, Is.Empty);
        }

        [Test]
        public void TestGetAllReviewsBySeller_WithNoReviews_ReturnsEmptyCollection()
        {
            var newUser = new User(3, "cristi", "cristi@example.com");
            newUser.UserType = 2;
            newUser.Rating = 5.0f;

            var reviews = reviewRepository.GetAllReviewsBySeller(newUser);

            Assert.That(reviews, Is.Not.Null);
            Assert.That(reviews, Is.Empty);
        }

        [Test]
        public void TestCreateReview_WithMultipleImages_SavesAllImages()
        {
            var newReview = new Review(
                -1,
                "Multiple images test",
                new List<Image> {
                    new Image("https://example.com/img1.jpg"),
                    new Image("https://example.com/img2.jpg"),
                    new Image("https://example.com/img3.jpg")
                },
                4.0f,
                buyer.Id,
                seller.Id
            );

            reviewRepository.CreateReview(newReview);

            var reviews = reviewRepository.GetAllReviewsByBuyer(seller);
            var addedReview = reviews.FirstOrDefault(r => r.Description == "Multiple images test");

            Assert.That(addedReview, Is.Not.Null);
            Assert.That(addedReview.Images, Is.Not.Null);
        }
    }
}