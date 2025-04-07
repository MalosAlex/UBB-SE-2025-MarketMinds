using System;
using System.Collections.Generic;
using DomainLayer.Domain;
using MarketMinds.Services.AuctionProductsService;
using MarketMinds.Services;
using MarketMinds.Services.ProductTagService;

namespace ViewModelLayer.ViewModel;

public class AuctionProductsViewModel
{
    private readonly IProductService auctionProductsService;
    private readonly AuctionValidationService auctionValidationService;

    public AuctionProductsViewModel(IProductService auctionProductsService)
    {
        this.auctionProductsService = auctionProductsService;
        this.auctionValidationService = new AuctionValidationService((IAuctionProductsService)auctionProductsService);
    }

    public List<AuctionProduct> GetAllProducts()
    {
        var auctionProducts = new List<AuctionProduct>();
        foreach (var product in auctionProductsService.GetProducts())
        {
            auctionProducts.Add((AuctionProduct)product);
        }
        return auctionProducts;
    }

    public void PlaceBid(AuctionProduct product, User bidder, string enteredBidText)
    {
        auctionValidationService.ValidateAndPlaceBid(product, bidder, enteredBidText);
    }

    public void ConcludeAuction(AuctionProduct product)
    {
        auctionValidationService.ValidateAndConcludeAuction(product);
    }

    public string GetTimeLeft(AuctionProduct product)
    {
        return ((IAuctionProductsService)auctionProductsService).GetTimeLeft(product);
    }

    public bool IsAuctionEnded(AuctionProduct product)
    {
        return ((IAuctionProductsService)auctionProductsService).IsAuctionEnded(product);
    }
}