using System;
using DomainLayer.Domain;
using MarketMinds.Services.AuctionProductsService;

namespace MarketMinds.Services
{
    public class AuctionValidationService
    {
        private readonly IAuctionProductsService auctionProductsService;

        public AuctionValidationService(IAuctionProductsService auctionProductsService)
        {
            this.auctionProductsService = auctionProductsService;
        }

        public void ValidateAndPlaceBid(AuctionProduct product, User bidder, string enteredBidText)
        {
            if (!float.TryParse(enteredBidText, out float bidAmount))
            {
                throw new ArgumentException("Invalid bid amount");
            }

            auctionProductsService.PlaceBid(product, bidder, bidAmount);
        }

        public void ValidateAndConcludeAuction(AuctionProduct product)
        {
            if (product == null)
            {
                throw new ArgumentException("Product cannot be null");
            }

            auctionProductsService.ConcludeAuction(product);
        }
    }
}