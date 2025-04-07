using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Services.AuctionProductsService;

namespace ViewModelLayer.ViewModel
{
    public class CreateAuctionListingViewModel : CreateListingViewModelBase
    {
        public AuctionProductsService? AuctionProductsService { get; set; }
        public override void CreateListing(Product product)
        {
            AuctionProductsService?.AddProduct(product);
        }
    }
}
