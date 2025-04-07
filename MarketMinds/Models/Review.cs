using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class Review
    {
        // The review doesn't take into account the product for which the review has been made,
        // it can be mentioned in the description, and images
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<Image> Images { get; set; }
        public float Rating { get; set; }
        public int SellerId { get; set; }
        public int BuyerId { get; set; }

        // public int productId { get; set; }
        public Review(int id, string description, List<Image> images, float rating, int sellerId, int buyerId)
        {
            this.Id = id;
            this.Description = description;
            this.Images = images;
            this.Rating = rating;
            this.SellerId = sellerId;
            this.BuyerId = buyerId;
        }
    }
}
