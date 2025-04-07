using DomainLayer.Domain;
using MarketMinds.Services;
using MarketMinds.Services.AuctionProductsService;
using MarketMinds.Services.BorrowProductsService;
using MarketMinds.Services.BuyProductsService;
using Moq;
using NUnit.Framework;
using System;

namespace MarketMinds.Test.Services
{
    [TestFixture]
    public class ListingCreationServiceTest
    {
        private Mock<IBuyProductsService> _mockBuyService;
        private Mock<IBorrowProductsService> _mockBorrowService;
        private Mock<IAuctionProductsService> _mockAuctionService;
        private ListingCreationService _service;
        private Product _testProduct;

        [SetUp]
        public void Setup()
        {
            _mockBuyService = new Mock<IBuyProductsService>();
            _mockBorrowService = new Mock<IBorrowProductsService>();
            _mockAuctionService = new Mock<IAuctionProductsService>();

            _service = new ListingCreationService(
                _mockBuyService.Object,
                _mockBorrowService.Object,
                _mockAuctionService.Object);
        }

        [Test]
        public void CreateMarketListing_BuyType_CallsBuyProductsService()
        {
            _service.CreateMarketListing(_testProduct, "buy");

            _mockBuyService.Verify(s => s.CreateListing(_testProduct), Times.Once);
            _mockBorrowService.Verify(s => s.CreateListing(It.IsAny<Product>()), Times.Never);
            _mockAuctionService.Verify(s => s.CreateListing(It.IsAny<Product>()), Times.Never);
        }

        [Test]
        public void CreateMarketListing_BorrowType_CallsBorrowProductsService()
        {
            _service.CreateMarketListing(_testProduct, "borrow");

            _mockBuyService.Verify(s => s.CreateListing(It.IsAny<Product>()), Times.Never);
            _mockBorrowService.Verify(s => s.CreateListing(_testProduct), Times.Once);
            _mockAuctionService.Verify(s => s.CreateListing(It.IsAny<Product>()), Times.Never);
        }

        [Test]
        public void CreateMarketListing_AuctionType_CallsAuctionProductsService()
        {
            _service.CreateMarketListing(_testProduct, "auction");

            _mockBuyService.Verify(s => s.CreateListing(It.IsAny<Product>()), Times.Never);
            _mockBorrowService.Verify(s => s.CreateListing(It.IsAny<Product>()), Times.Never);
            _mockAuctionService.Verify(s => s.CreateListing(_testProduct), Times.Once);
        }

        [Test]
        public void CreateMarketListing_InvalidType_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                _service.CreateMarketListing(_testProduct, "invalid_type"));

            Assert.That(exception.Message, Does.Contain("Invalid listing type"));
        }

        [Test]
        public void CreateMarketListing_CaseInsensitive_WorksCorrectly()
        {
            _service.CreateMarketListing(_testProduct, "BUY");

            _mockBuyService.Verify(s => s.CreateListing(_testProduct), Times.Once);
        }
    }
}