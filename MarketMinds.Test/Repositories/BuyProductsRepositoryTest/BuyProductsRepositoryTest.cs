using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MarketMinds.Repositories.BuyProductsRepository;
using DomainLayer.Domain;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using MarketMinds.Test.Utils;

namespace MarketMinds.Tests.BuyProductsRepositoryTest
{
    [TestFixture]
    public class BuyProductRepositoryTest
    {
        private IBuyProductsRepository buyProductRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        User testSeller = new User(1, "test buyer", "test email");
        ProductCondition testProductCondition = new ProductCondition(1, "Test", "Test");
        ProductCategory testProductCategory = new ProductCategory(1, "test", "test");
        List<ProductTag> testProductTags = new List<ProductTag>();
        Product buyProduct, auctionProduct;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            buyProductRepository = new BuyProductsRepository(connection);

            testDbHelper.PrepareTestDatabase();

            buyProduct = new BuyProduct(1, "Test Product", "Test Description", testSeller, testProductCondition, testProductCategory, testProductTags, new List<Image>(), 100);
            auctionProduct = new AuctionProduct(1, "Test Product", "Test Description", testSeller, testProductCondition, testProductCategory, testProductTags, new List<Image>(), new DateTime(2025, 04, 04), new DateTime(2030, 04, 04), 100);
        }

        [Test]
        public void TestGetProducts_ReturnsListOfProducts()
        {
            var products = buyProductRepository.GetProducts();

            Assert.That(products, Is.Not.Null);
            Assert.That(products.GetType(), Is.EqualTo(typeof(List<Product>)));
        }

        [Test]
        public void TestAddProduct_WrongProductType_ThrowsException()
        {
            Assert.Throws<InvalidCastException>(() => buyProductRepository.AddProduct(auctionProduct));
        }

        [Test]
        public void TestAddProduct_NullProduct_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => buyProductRepository.AddProduct(null));
        }

        [Test]
        public void TestAddProduct_ValidProduct_AddsProduct()
        {
            Assert.DoesNotThrow(() => buyProductRepository.AddProduct(buyProduct));
        }

        [Test]
        public void TestGetProductByID_ValidID_ReturnsProduct()
        {
            buyProductRepository.AddProduct(buyProduct);

            int productId = 1;
            var retrievedProduct = buyProductRepository.GetProductByID(productId);

            Assert.That(retrievedProduct, Is.Not.Null);
            Assert.That(retrievedProduct.Id, Is.EqualTo(productId));
        }

        [Test]
        public void TestGetProductByID_InvalidID_ReturnsNull()
        {
            var retrievedProduct = buyProductRepository.GetProductByID(999);
            Assert.That(retrievedProduct, Is.Null);
        }

        [Test]
        public void TestDeleteProduct_ValidProduct_DeletesProduct()
        {
            buyProductRepository.AddProduct(buyProduct);

            Assert.DoesNotThrow(() => buyProductRepository.DeleteProduct(buyProduct));

            var retrievedProduct = buyProductRepository.GetProductByID(buyProduct.Id);
            Assert.That(retrievedProduct, Is.Null);
        }

        [Test]
        public void TestUpdateProduct_NotImplemented()
        {
            buyProductRepository.UpdateProduct(buyProduct);
        }
    }
}
