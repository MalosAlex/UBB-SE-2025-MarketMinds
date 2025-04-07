using System;
using System.Collections.Generic;
using NUnit.Framework;
using DomainLayer.Domain;
using MarketMinds.Services.BuyProductsService;
using MarketMinds.Repositories.BuyProductsRepository;
using MarketMinds.Test.Services.BuyProductsServiceTest;

namespace MarketMinds.Tests.Services.BuyProductsServiceTest
{
    [TestFixture]
    public class BuyProductsServiceTest
    {
        private BuyProductsService buyProductsService;
        private BuyProductsRepositoryMock buyProductsRepositoryMock;
        private User testSeller;
        private BuyProduct testBuyProduct;
        private AuctionProduct testInvalidProduct;

        [SetUp]
        public void Setup()
        {
            buyProductsRepositoryMock = new BuyProductsRepositoryMock();
            buyProductsService = new BuyProductsService(buyProductsRepositoryMock);

            testSeller = new User(1, "Test Seller", "seller@test.com");

            var testCondition = new ProductCondition(1, "New", "Brand new item");
            var testCategory = new ProductCategory(1, "Electronics", "Electronic devices");
            var testTags = new List<ProductTag>();
            var testImages = new List<Image>();

            testBuyProduct = new BuyProduct(
                1,
                "Test Buy Product",
                "Test Description",
                testSeller,
                testCondition,
                testCategory,
                testTags,
                testImages,
                99.99f);

            // Create an invalid product type for testing type validation
            testInvalidProduct = new AuctionProduct(
                2,
                "Test Auction Product",
                "Test Description",
                testSeller,
                testCondition,
                testCategory,
                testTags,
                testImages,
                DateTime.Now,
                DateTime.Now.AddDays(7),
                100.0f);
        }

        [Test]
        public void TestCreateListing_ValidProduct_AddsProduct()
        {
            buyProductsService.CreateListing(testBuyProduct);

            Assert.That(buyProductsRepositoryMock.GetCreateListingCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestCreateListing_InvalidProductType_ThrowsException()
        {
            Exception ex = null;
            try
            {
                buyProductsService.CreateListing(testInvalidProduct);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex, Is.InstanceOf<InvalidCastException>());
            Assert.That(buyProductsRepositoryMock.GetCreateListingCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestDeleteListing_ValidProduct_DeletesProduct()
        {
            buyProductsService.DeleteListing(testBuyProduct);

            Assert.That(buyProductsRepositoryMock.GetDeleteListingCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestGetProducts_ReturnsProductsList()
        {
            buyProductsRepositoryMock.AddProduct(testBuyProduct);

            var products = buyProductsService.GetProducts();

            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count, Is.EqualTo(1));
            Assert.That(buyProductsRepositoryMock.GetGetProductsCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestGetProductById_ValidId_ReturnsProduct()
        {
            buyProductsRepositoryMock.AddProduct(testBuyProduct);

            var product = buyProductsService.GetProductById(testBuyProduct.Id);

            Assert.That(product, Is.Not.Null);
            Assert.That(product.Id, Is.EqualTo(testBuyProduct.Id));
            Assert.That(buyProductsRepositoryMock.GetGetProductByIdCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestGetProductById_InvalidId_ReturnsNull()
        {
            var product = buyProductsService.GetProductById(999);

            Assert.That(product, Is.Null);
            Assert.That(buyProductsRepositoryMock.GetGetProductByIdCount(), Is.EqualTo(1));
        }
    }
}