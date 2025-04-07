using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;

namespace MarketMinds.Services
{
    public class ReviewCalculationService
    {
        public float CalculateAverageRating(IEnumerable<Review> reviews)
        {
            if (reviews == null || !reviews.Any())
            {
                return 0;
            }
            return reviews.Average(r => r.Rating);
        }

        public int GetReviewCount(IEnumerable<Review> reviews)
        {
            return reviews?.Count() ?? 0;
        }

        public bool AreReviewsEmpty(IEnumerable<Review> reviews)
        {
            return reviews == null || !reviews.Any();
        }
    }
}