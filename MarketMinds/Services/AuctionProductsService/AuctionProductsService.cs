using System.Threading.Tasks;
using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using DomainLayer.Domain;
using MarketMinds.Repositories.AuctionProductsRepository;
using MarketMinds.Services.ProductTagService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MarketMinds.Services.AuctionProductsService
{
    public class AuctionProductsService : ProductService, IAuctionProductsService
    {
        private IAuctionProductsRepository auctionRepository;
        public AuctionProductsService(IAuctionProductsRepository repository) : base(repository)
        {
            auctionRepository = repository;
        }

        public void CreateListing(Product product)
        {
            AddProduct(product);
        }

        public void PlaceBid(AuctionProduct auction, User bidder, float bidAmount)
        {
            ValidateBid(auction, bidder, bidAmount);

            bidder.Balance -= bidAmount;

            RefundPreviousBidder(auction);

            var bid = new Bid(bidder, bidAmount, DateTime.Now);
            auction.AddBid(bid);
            auction.CurrentPrice = bidAmount;

            ExtendAuctionTime(auction);

            auctionRepository.UpdateProduct(auction);
        }
        private void ValidateBid(AuctionProduct auction, User bidder, float bidAmount)
        {
            float minBid = auction.BidHistory.Count == 0 ? auction.StartingPrice : auction.CurrentPrice + 1;

            if (bidAmount < minBid)
            {
                throw new Exception($"Bid must be at least ${minBid}");
            }

            if (bidAmount > bidder.Balance)
            {
                throw new Exception("Insufficient balance");
            }

            if (DateTime.Now > auction.EndAuctionDate)
            {
                throw new Exception("Auction already ended");
            }
        }

        private void RefundPreviousBidder(AuctionProduct auction)
        {
            if (auction.BidHistory.Count > 0)
            {
                var previousBid = auction.BidHistory.Last();
                previousBid.Bidder.Balance += previousBid.Price;
            }
        }
        private void ExtendAuctionTime(AuctionProduct auction)
        {
            var timeRemaining = auction.EndAuctionDate - DateTime.Now;

            if (timeRemaining.TotalMinutes < 5)
            {
                auction.EndAuctionDate = auction.EndAuctionDate.AddMinutes(1);
            }
        }

        public void ConcludeAuction(AuctionProduct auction)
        {
            auctionRepository.DeleteProduct(auction);
        }

        public string GetTimeLeft(AuctionProduct auction)
        {
            var timeLeft = auction.EndAuctionDate - DateTime.Now;
            return timeLeft > TimeSpan.Zero ? timeLeft.ToString(@"dd\:hh\:mm\:ss") : "Auction Ended";
        }

        public bool IsAuctionEnded(AuctionProduct auction)
        {
            return DateTime.Now >= auction.EndAuctionDate;
        }
    }
}
