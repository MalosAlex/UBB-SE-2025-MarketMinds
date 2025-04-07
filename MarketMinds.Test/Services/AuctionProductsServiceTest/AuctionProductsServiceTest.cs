using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DomainLayer.Domain;
using MarketMinds.Services.AuctionProductsService;
using MarketMinds.Repositories;
using MarketMinds.Repositories.AuctionProductsRepository;
using MarketMinds.Test.Services.AuctionProductsServiceTest;

namespace MarketMinds.Tests.Services.AuctionProductsServiceTest
{
    [TestFixture]
    public class AuctionProductsServiceTest
    {
        private AuctionProductsService auctionProductsService;
        private AuctionProductsRepositoryMock auctionProductsRepositoryMock;

        User testSeller;
        User testBidder;
        ProductCondition testProductCondition;
        ProductCategory testProductCategory;
        List<ProductTag> testProductTags;
        AuctionProduct testAuction;

        [SetUp]
        public void Setup()
        {
            auctionProductsRepositoryMock = new AuctionProductsRepositoryMock();
            auctionProductsService = new AuctionProductsService(auctionProductsRepositoryMock);

            testSeller = new User(1, "test seller", "seller@test.com");
            testBidder = new User(2, "test bidder", "bidder@test.com");
            testBidder.Balance = 1000;
            testProductCondition = new ProductCondition(1, "Test", "Test");
            testProductCategory = new ProductCategory(1, "test", "test");
            testProductTags = new List<ProductTag>();

            testAuction = new AuctionProduct(
                1,
                "Test Auction",
                "Test Description",
                testSeller,
                testProductCondition,
                testProductCategory,
                testProductTags,
                new List<Image>(),
                DateTime.Now.AddDays(-1),
                DateTime.Now.AddDays(1),
                100);
        }

        [Test]
        public void TestPlaceBid_ValidBid_PlacesBid()
        {
            float bidAmount = 200;

            auctionProductsService.PlaceBid(testAuction, testBidder, bidAmount);

            Assert.That(testAuction.CurrentPrice, Is.EqualTo(bidAmount));
            Assert.That(testBidder.Balance, Is.EqualTo(800));
            Assert.That(testAuction.BidHistory.Count, Is.EqualTo(1));
            Assert.That(testAuction.BidHistory[0].Bidder, Is.EqualTo(testBidder));
            Assert.That(testAuction.BidHistory[0].Price, Is.EqualTo(bidAmount));
            Assert.That(auctionProductsRepositoryMock.GetUpdateCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestPlaceBid_BidTooLow_ThrowsException()
        {
            float initialBid = 200;
            auctionProductsService.PlaceBid(testAuction, testBidder, initialBid);

            auctionProductsRepositoryMock = new AuctionProductsRepositoryMock();
            auctionProductsService = new AuctionProductsService(auctionProductsRepositoryMock);

            float lowBidAmount = 150;

            Console.WriteLine($"Current price: {testAuction.CurrentPrice}, Bid amount: {lowBidAmount}");

            Exception ex = null;
            try
            {
                auctionProductsService.PlaceBid(testAuction, testBidder, lowBidAmount);
            }
            catch (Exception e)
            {
                ex = e;
                Console.WriteLine($"Exception caught: {e.Message}");
            }

            Assert.That(ex, Is.Not.Null, "Expected an exception for bid too low but none was thrown");
            if (ex != null)
            {
                Assert.That(ex.Message, Does.Contain("Bid must be at least"));
            }
            Assert.That(auctionProductsRepositoryMock.GetUpdateCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestPlaceBid_InsufficientBalance_ThrowsException()
        {
            testBidder.Balance = 50;
            float bidAmount = 200;

            Exception ex = null;
            try
            {
                auctionProductsService.PlaceBid(testAuction, testBidder, bidAmount);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Insufficient balance"));
            Assert.That(auctionProductsRepositoryMock.GetUpdateCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestPlaceBid_AuctionEnded_ThrowsException()
        {
            testAuction.EndAuctionDate = DateTime.Now.AddDays(-1);
            float bidAmount = 200;

            Exception ex = null;
            try
            {
                auctionProductsService.PlaceBid(testAuction, testBidder, bidAmount);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Auction already ended"));
            Assert.That(auctionProductsRepositoryMock.GetUpdateCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestPlaceBid_MultipleBids_RefundsPreviousBidder()
        {
            User firstBidder = new User(3, "first bidder", "first@test.com");
            firstBidder.Balance = 500;
            float firstBidAmount = 150;

            auctionProductsService.PlaceBid(testAuction, firstBidder, firstBidAmount);

            Assert.That(firstBidder.Balance, Is.EqualTo(350));
            Assert.That(testAuction.CurrentPrice, Is.EqualTo(firstBidAmount));

            float secondBidAmount = 200;
            auctionProductsService.PlaceBid(testAuction, testBidder, secondBidAmount);

            Assert.That(firstBidder.Balance, Is.EqualTo(500)); // Refunded
            Assert.That(testBidder.Balance, Is.EqualTo(800));
            Assert.That(testAuction.CurrentPrice, Is.EqualTo(secondBidAmount));
            Assert.That(testAuction.BidHistory.Count, Is.EqualTo(2));
            Assert.That(auctionProductsRepositoryMock.GetUpdateCount(), Is.EqualTo(2));
        }

        [Test]
        public void TestPlaceBid_BidNearEndTime_ExtendsAuctionTime()
        {
            testAuction.EndAuctionDate = DateTime.Now.AddMinutes(3);
            DateTime originalEndTime = testAuction.EndAuctionDate;

            auctionProductsService.PlaceBid(testAuction, testBidder, 200);

            Assert.That(testAuction.EndAuctionDate, Is.GreaterThan(originalEndTime));
            Assert.That(auctionProductsRepositoryMock.GetUpdateCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestConcludeAuction_ValidAuction_DeletesAuction()
        {
            auctionProductsService.ConcludeAuction(testAuction);

            Assert.That(auctionProductsRepositoryMock.GetDeleteCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestCreateListing_CoverLines()
        {
            auctionProductsService.CreateListing(testAuction);
            Assert.That(auctionProductsRepositoryMock.GetProducts().Count, Is.EqualTo(1));
            Assert.That(auctionProductsRepositoryMock.GetProducts()[0], Is.EqualTo(testAuction));
        }
    }
}