using System;
using System.Collections.Generic;
using NUnit.Framework;
using DomainLayer.Domain;
using MarketMinds.Services.BorrowProductsService;
using MarketMinds.Repositories.BorrowProductsRepository;
using MarketMinds.Test.Services.BorrowProductsServiceTest;

namespace MarketMinds.Tests.Services.BorrowProductsServiceTest
{
    [TestFixture]
    public class BorrowProductsServiceTest
    {
        private BorrowProductsService borrowProductsService;
        private BorrowProductsRepositoryMock borrowProductsRepositoryMock;
        private User testSeller;
        private BorrowProduct testBorrowProduct;
        private AuctionProduct testInvalidProduct;

        [SetUp]
        public void Setup()
        {
            borrowProductsRepositoryMock = new BorrowProductsRepositoryMock();
            borrowProductsService = new BorrowProductsService(borrowProductsRepositoryMock);

            testSeller = new User(1, "Test Seller", "seller@test.com");

            var testCondition = new ProductCondition(1, "New", "Brand new item");
            var testCategory = new ProductCategory(1, "Electronics", "Electronic devices");
            var testTags = new List<ProductTag>();
            var testImages = new List<Image>();

            DateTime timeLimit = DateTime.Now.AddDays(7);
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(5);
            float dailyRate = 20.0f;
            bool isBorrowed = false;

            testBorrowProduct = new BorrowProduct(
                1,
                "Test Borrow Product",
                "Test Description",
                testSeller,
                testCondition,
                testCategory,
                testTags,
                testImages,
                timeLimit,
                startDate,
                endDate,
                dailyRate,
                isBorrowed);

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
            borrowProductsService.CreateListing(testBorrowProduct);

            Assert.That(borrowProductsRepositoryMock.GetCreateListingCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestCreateListing_InvalidProductType_ThrowsException()
        {
            Exception ex = null;
            try
            {
                borrowProductsService.CreateListing(testInvalidProduct);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex, Is.InstanceOf<InvalidCastException>());
            Assert.That(borrowProductsRepositoryMock.GetCreateListingCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestDeleteListing_ValidProduct_DeletesProduct()
        {
            borrowProductsService.DeleteListing(testBorrowProduct);

            Assert.That(borrowProductsRepositoryMock.GetDeleteListingCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestGetProducts_ReturnsProductsList()
        {
            borrowProductsRepositoryMock.AddProduct(testBorrowProduct);

            var products = borrowProductsService.GetProducts();

            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count, Is.EqualTo(1));
            Assert.That(borrowProductsRepositoryMock.GetGetProductsCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestGetProductById_ValidId_ReturnsProduct()
        {
            borrowProductsRepositoryMock.AddProduct(testBorrowProduct);

            var product = borrowProductsService.GetProductById(testBorrowProduct.Id);

            Assert.That(product, Is.Not.Null);
            Assert.That(product.Id, Is.EqualTo(testBorrowProduct.Id));
            Assert.That(borrowProductsRepositoryMock.GetGetProductByIdCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestGetProductById_InvalidId_ReturnsNull()
        {
            var product = borrowProductsService.GetProductById(999);

            Assert.That(product, Is.Null);
            Assert.That(borrowProductsRepositoryMock.GetGetProductByIdCount(), Is.EqualTo(1));
        }
    }
}
