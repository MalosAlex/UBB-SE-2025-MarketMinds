using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Repositories;
using MarketMinds.Repositories.AuctionProductsRepository;

namespace MarketMinds.Test.Services.AuctionProductsServiceTest
{
    public class AuctionProductsRepositoryMock : IAuctionProductsRepository
    {
        private List<Product> products;
        private int updateCount;
        private int deleteCount;

        public AuctionProductsRepositoryMock()
        {
            products = new List<Product>();
            updateCount = 0;
            deleteCount = 0;
        }

        public void AddProduct(Product product)
        {
            products.Add(product);
        }

        public void DeleteProduct(Product product)
        {
            products.Remove(product);
            deleteCount++;
        }

        public AuctionProduct GetProductByID(int id)
        {
            return (AuctionProduct)products.FirstOrDefault(p => p.Id == id);
        }

        Product IProductsRepository.GetProductByID(int id)
        {
            return GetProductByID(id);
        }

        public List<Product> GetProducts()
        {
            return products;
        }

        public void UpdateProduct(Product product)
        {
            updateCount++;
        }

        public int GetUpdateCount()
        {
            return updateCount;
        }

        public int GetDeleteCount()
        {
            return deleteCount;
        }
    }
}
