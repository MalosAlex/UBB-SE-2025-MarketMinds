using System;
using DomainLayer.Domain;
using MarketMinds.Services.BuyProductsService;
using MarketMinds.Services.BorrowProductsService;
using MarketMinds.Services.AuctionProductsService;

namespace MarketMinds.Services
{
    public class ListingCreationService
    {
        private readonly IBuyProductsService buyProductsService;
        private readonly IBorrowProductsService borrowProductsService;
        private readonly IAuctionProductsService auctionProductsService;

        public ListingCreationService(
            IBuyProductsService buyProductsService,
            IBorrowProductsService borrowProductsService,
            IAuctionProductsService auctionProductsService)
        {
            this.buyProductsService = buyProductsService;
            this.borrowProductsService = borrowProductsService;
            this.auctionProductsService = auctionProductsService;
        }

        public void CreateMarketListing(Product product, string listingType)
        {
            switch (listingType.ToLower())
            {
                case "buy":
                    buyProductsService.CreateListing(product);
                    break;
                case "borrow":
                    borrowProductsService.CreateListing(product);
                    break;
                case "auction":
                    auctionProductsService.CreateListing(product);
                    break;
                default:
                    throw new ArgumentException($"Invalid listing type: {listingType}");
            }
        }
    }
}