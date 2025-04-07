using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Repositories.ReviewRepository;

namespace MarketMinds.Services.ReviewService
{
    public class ReviewsService : IReviewsService
    {
        public IReviewRepository Repository;

        public ReviewsService(IReviewRepository repository)
        {
            Repository = repository;
        }

        public ObservableCollection<Review> GetReviewsBySeller(User seller)
        {
            ObservableCollection<Review> reviews = Repository.GetAllReviewsBySeller(seller);
            // add to Review date and sort by dates
            // reviews.Sort()
            return reviews;
        }

        public ObservableCollection<Review> GetReviewsByBuyer(User buyer)
        {
            ObservableCollection<Review> reviews = Repository.GetAllReviewsByBuyer(buyer);
            // same here
            return reviews;
        }

        public void AddReview(string description, List<Image> images, float rating, User seller, User buyer)
        {
            Repository.CreateReview(new Review(-1, description, images, rating, seller.Id, buyer.Id));
        }

        public void EditReview(string description, List<Image> images, float rating, int sellerid, int buyerid, string newDescription, float newRating)
        {
            // change in uml aswell
            Repository.EditReview(new Review(-1, description, images, rating, sellerid, buyerid), newRating, newDescription);
        }

        public void DeleteReview(string description, List<Image> images, float rating, int sellerid, int buyerid)
        {
            Repository.DeleteReview(new Review(-1, description, images, rating, sellerid, buyerid));
        }
    }
}
