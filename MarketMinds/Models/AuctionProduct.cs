using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class AuctionProduct : Product
    {
        public DateTime StartAuctionDate { get; set; }
        public DateTime EndAuctionDate { get; set; }
        public float StartingPrice { get; set; }
        public float CurrentPrice { get; set; }

        public List<Bid> BidHistory { get; set; }

        public AuctionProduct(int id, string title, string description, User seller, ProductCondition productCondition, ProductCategory productCategory,
            List<ProductTag> productTags, List<Image> images, DateTime startAuctionDate, DateTime endAuctionDate, float startingPrice)
        {
            this.Id = id;
            this.Description = description;
            this.Title = title;
            this.Seller = seller;
            this.Condition = productCondition;
            this.Category = productCategory;
            this.Tags = productTags;
            this.Seller = seller;
            this.Images = images;
            this.StartAuctionDate = startAuctionDate;
            this.EndAuctionDate = endAuctionDate;
            this.StartingPrice = startingPrice;
            this.CurrentPrice = startingPrice;
            this.BidHistory = new List<Bid>();
        }

        public void AddBid(Bid bid)
        {
            this.CurrentPrice = bid.Price;
            this.BidHistory.Add(bid);
        }
    }
}
