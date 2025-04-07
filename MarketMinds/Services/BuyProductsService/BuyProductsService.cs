using System.Collections.Generic;
using DomainLayer.Domain;
using MarketMinds.Repositories.BuyProductsRepository;
using MarketMinds.Services.ProductTagService;

namespace MarketMinds.Services.BuyProductsService
{
    public class BuyProductsService : ProductService, IBuyProductsService
    {
        private readonly IBuyProductsRepository buyProductsRepository;

        public BuyProductsService(IBuyProductsRepository repository) : base(repository)
        {
            buyProductsRepository = repository;
        }

        public void CreateListing(Product product)
        {
            buyProductsRepository.AddProduct(product);
        }

        public void DeleteListing(Product product)
        {
            buyProductsRepository.DeleteProduct(product);
        }

        public new List<Product> GetProducts()
        {
            return buyProductsRepository.GetProducts();
        }
    }
}