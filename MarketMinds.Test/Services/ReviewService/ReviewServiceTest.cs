using DomainLayer.Domain;
using MarketMinds.Services.ReviewService;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketMinds.Test.Services.ReviewService
{
    [TestFixture]
    internal class ReviewServiceTest
    {
        private ReviewRepositoryMock mockRepository;
        private IReviewsService reviewsService;

        [Test]
        public void GetAllReviewsBySeller_ShouldReturnOnlySellerReviews()
        {
            mockRepository = new ReviewRepositoryMock();
            reviewsService = new ReviewsService(mockRepository);

            var seller1 = new User(1, "Marcel", "marcel@mail.com");
            var seller2 = new User(2, "Dorel", "dorel@mail.com");
            var buyer = new User(3, "Sorin", "sorin@mail.com");

            mockRepository.CreateReview(new Review(1, "Review 1", new List<Image>(), 4.5f, seller1.Id, buyer.Id));
            mockRepository.CreateReview(new Review(2, "Review 2", new List<Image>(), 5.0f, seller1.Id, buyer.Id));
            mockRepository.CreateReview(new Review(3, "Review 3", new List<Image>(), 3.5f, seller2.Id, buyer.Id));

            var seller1Reviews = reviewsService.GetReviewsBySeller(seller1);

            Assert.That(seller1Reviews.Count, Is.EqualTo(2));
            Assert.That(seller1Reviews.All(r => r.SellerId == seller1.Id), Is.True);
        }

        [Test]
        public void GetReviewsByBuyer_ShouldReturnOnlyBuyerReviews()
        {
            var mockRepository = new ReviewRepositoryMock();
            var reviewsService = new ReviewsService(mockRepository);

            var seller = new User(1, "Marcel", "marcel@mail.com");
            var buyer1 = new User(2, "Dorel", "dorel@mail.com");
            var buyer2 = new User(3, "Sorin", "sorin@mail.com");

            mockRepository.CreateReview(new Review(1, "Review 1", new List<Image>(), 4.5f, seller.Id, buyer1.Id));
            mockRepository.CreateReview(new Review(2, "Review 2", new List<Image>(), 5.0f, seller.Id, buyer1.Id));
            mockRepository.CreateReview(new Review(3, "Review 3", new List<Image>(), 3.5f, seller.Id, buyer2.Id));

            var buyerReviews = reviewsService.GetReviewsByBuyer(buyer1);

            Assert.That(buyerReviews.Count, Is.EqualTo(2));
            Assert.That(buyerReviews.All(r => r.BuyerId == buyer1.Id), Is.True);
        }

        [Test]
        public void AddReview_ShouldAddReviewToRepository()
        {
            var mockRepository = new ReviewRepositoryMock();
            var reviewsService = new ReviewsService(mockRepository);

            var seller = new User(1, "Luca", "luca@mail.com");
            var buyer = new User(2, "Cristi", "cristi@mail.com");
            string description = "I love testing.";
            List<Image> images = new List<Image>();
            float rating = 4.8f;

            reviewsService.AddReview(description, images, rating, seller, buyer);

            Assert.That(mockRepository.Reviews.Count, Is.EqualTo(1));
            Assert.That(mockRepository.Reviews[0].Description, Is.EqualTo(description));
            Assert.That(mockRepository.Reviews[0].Rating, Is.EqualTo(rating));
            Assert.That(mockRepository.Reviews[0].SellerId, Is.EqualTo(seller.Id));
            Assert.That(mockRepository.Reviews[0].BuyerId, Is.EqualTo(buyer.Id));
        }

        [Test]
        public void EditReview_ShouldUpdateReviewInRepository()
        {
            var mockRepository = new ReviewRepositoryMock();
            var reviewsService = new ReviewsService(mockRepository);

            var seller = new User(1, "Luca", "luca@mail.com");
            var buyer = new User(2, "Cristi", "cristi@mail.com");
            string originalDescription = "I love testing.";
            string newDescription = "I love testing not.";
            float originalRating = 3.0f;
            float newRating = 4.5f;

            var review = new Review(1, originalDescription, new List<Image>(), originalRating, seller.Id, buyer.Id);
            mockRepository.CreateReview(review);

            reviewsService.EditReview(originalDescription, new List<Image>(), originalRating, seller.Id, buyer.Id, newDescription, newRating);

            Assert.That(mockRepository.Reviews.Count, Is.EqualTo(1));
            var editedReview = mockRepository.Reviews[0];
            Assert.That(editedReview.Description, Is.EqualTo(newDescription));
            Assert.That(editedReview.Rating, Is.EqualTo(newRating));
        }

        [Test]
        public void DeleteReview_ShouldRemoveReviewFromRepository()
        {
            var mockRepository = new ReviewRepositoryMock();
            var reviewsService = new ReviewsService(mockRepository);

            var seller = new User(1, "Luca", "luca@mail.com");
            var buyer = new User(2, "Cristi", "cristi@mail.com");
            string originalDescription = "I love testing.";
            float rating = 4.0f;

            var review = new Review(1, originalDescription, new List<Image>(), rating, seller.Id, buyer.Id);
            mockRepository.CreateReview(review);
            Assert.That(mockRepository.Reviews.Count, Is.EqualTo(1));

            reviewsService.DeleteReview(originalDescription, new List<Image>(), rating, seller.Id, buyer.Id);

            Assert.That(mockRepository.Reviews.Count, Is.EqualTo(0));
        }
    }
}
