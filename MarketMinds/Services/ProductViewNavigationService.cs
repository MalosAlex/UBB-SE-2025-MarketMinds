using System;
using DomainLayer.Domain;
using MarketMinds.Views.Pages;
using Microsoft.UI.Xaml;

namespace MarketMinds.Services
{
    public class ProductViewNavigationService
    {
        public Window CreateProductDetailView(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            Window detailView;

            if (product is AuctionProduct auctionProduct)
            {
                detailView = new AuctionProductView(auctionProduct);
            }
            else if (product is BorrowProduct borrowProduct)
            {
                detailView = new BorrowProductView(borrowProduct);
            }
            else if (product is BuyProduct buyProduct)
            {
                detailView = new BuyProductView(buyProduct);
            }
            else
            {
                throw new ArgumentException($"Unknown product type: {product.GetType().Name}");
            }

            return detailView;
        }

        public Window CreateSellerReviewsView(User seller)
        {
            if (seller == null)
            {
                throw new ArgumentNullException(nameof(seller));
            }

            var window = new Window();
            window.Content = new SeeSellerReviewsView(App.SeeSellerReviewsViewModel);
            App.SeeSellerReviewsViewModel.Seller = seller;
            return window;
        }
    }
}