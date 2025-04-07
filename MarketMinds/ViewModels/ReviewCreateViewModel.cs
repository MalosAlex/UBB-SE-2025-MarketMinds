using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Services.ReviewService;
using MarketMinds.Services;
using Microsoft.UI.Xaml;

namespace ViewModelLayer.ViewModel
{
    public class ReviewCreateViewModel
    {
        public Review CurrentReview { get; set; }
        public ReviewsService ReviewsService { get; set; }
        public User Seller { get; set; }
        public User Buyer { get; set; }
        public string Description { get; set; }
        public List<Image> Images { get; set; }
        public float Rating { get; set; }
        public string StatusMessage { get; set; }
        private readonly ReviewCreationService reviewCreationService;
        private readonly ImageUploadService imageUploadService;

        public string ImagesString
        {
            get => reviewCreationService.FormatImagesString(Images);
            set => Images = reviewCreationService.ParseImagesString(value);
        }

        public ReviewCreateViewModel(ReviewsService reviewsService, User buyer, User seller)
        {
            ReviewsService = reviewsService;
            Buyer = buyer;
            Seller = seller;
            reviewCreationService = new ReviewCreationService(reviewsService);
            imageUploadService = new ImageUploadService();
            Images = new List<Image>();
            StatusMessage = string.Empty;
        }

        public async Task<bool> AddImage(Window window)
        {
            try
            {
                // Add temporary message
                string originalImagesString = ImagesString;
                StatusMessage = "Uploading image...";

                // Attempt to upload the image
                string updatedImagesString = await imageUploadService.AddImageToCollection(window, ImagesString);

                // Clear status if successful
                if (updatedImagesString != originalImagesString)
                {
                    ImagesString = updatedImagesString;
                    StatusMessage = "Image uploaded successfully";
                    return true;
                }
                else
                {
                    StatusMessage = "No image selected or upload cancelled";
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                StatusMessage = $"Error uploading image: {ex.Message}";
                return false;
            }
        }

        public void SubmitReview()
        {
            try
            {
                Debug.WriteLine("Submitting Review");
                StatusMessage = "Submitting review...";
                CurrentReview = reviewCreationService.CreateReview(Description, Images, Rating, Seller, Buyer);
                StatusMessage = "Review submitted successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error submitting review: {ex.Message}";
            }
        }

        public void UpdateReview()
        {
            try
            {
                StatusMessage = "Updating review...";
                reviewCreationService.UpdateReview(CurrentReview, Description, Rating);
                StatusMessage = "Review updated successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating review: {ex.Message}";
            }
        }
    }
}
