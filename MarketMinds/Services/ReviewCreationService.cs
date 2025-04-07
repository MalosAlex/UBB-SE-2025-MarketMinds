using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using MarketMinds.Services.ReviewService;

namespace MarketMinds.Services
{
    public class ReviewCreationService
    {
        private readonly ReviewsService reviewsService;
        private readonly ImageUploadService imageService;

        public ReviewCreationService(ReviewsService reviewsService)
        {
            this.reviewsService = reviewsService;
            this.imageService = new ImageUploadService();
        }

        public Review CreateReview(string description, List<Image> images, float rating, User seller, User buyer)
        {
            if (seller == null || buyer == null || reviewsService == null)
            {
                throw new ArgumentException("One of the required objects is null!");
            }

            reviewsService.AddReview(description, images, rating, seller, buyer);
            return new Review(-1, description, images, rating, seller.Id, buyer.Id);
        }

        public void UpdateReview(Review currentReview, string newDescription, float newRating)
        {
            if (currentReview == null)
            {
                throw new ArgumentException("Current review cannot be null");
            }

            reviewsService.EditReview(
                currentReview.Description,
                currentReview.Images,
                currentReview.Rating,
                currentReview.SellerId,
                currentReview.BuyerId,
                newDescription,
                newRating);
        }

        public void EditReview(Review review, string description, float rating)
        {
            if (review == null)
            {
                throw new ArgumentException("Review cannot be null");
            }

            reviewsService.EditReview(
                review.Description,
                review.Images,
                review.Rating,
                review.SellerId,
                review.BuyerId,
                description,
                rating);
        }

        public void DeleteReview(Review review)
        {
            if (review == null)
            {
                throw new ArgumentException("Review cannot be null");
            }

            reviewsService.DeleteReview(
                review.Description,
                review.Images,
                review.Rating,
                review.SellerId,
                review.BuyerId);
        }

        public List<Image> ParseImagesString(string imagesString)
        {
            return imageService.ParseImagesString(imagesString);
        }

        public string FormatImagesString(List<Image> images)
        {
            return imageService.FormatImagesString(images);
        }
    }
}