using DomainLayer.Domain;
using MarketMinds.Services;
using NUnit.Framework;

namespace MarketMinds.Test.Services.AuctionSortTypeConverterService
{
    [TestFixture]
    public class AuctionSortTypeConverterServiceTest
    {
        private MarketMinds.Services.AuctionSortTypeConverterService _converter;

        [SetUp]
        public void Setup()
        {
            _converter = new MarketMinds.Services.AuctionSortTypeConverterService();
        }

        [Test]
        public void Convert_SellerRatingAsc_ReturnsCorrectSortType()
        {
            string sortTag = "SellerRatingAsc";
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExternalAttributeFieldTitle, Is.EqualTo("Seller Rating"));
            Assert.That(result.InternalAttributeFieldTitle, Is.EqualTo("SellerRating"));
            Assert.That(result.IsAscending, Is.True);
        }

        [Test]
        public void Convert_SellerRatingDesc_ReturnsCorrectSortType()
        {
            string sortTag = "SellerRatingDesc";
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExternalAttributeFieldTitle, Is.EqualTo("Seller Rating"));
            Assert.That(result.InternalAttributeFieldTitle, Is.EqualTo("SellerRating"));
            Assert.That(result.IsAscending, Is.False);
        }

        [Test]
        public void Convert_StartingPriceAsc_ReturnsCorrectSortType()
        {
            string sortTag = "StartingPriceAsc";
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExternalAttributeFieldTitle, Is.EqualTo("Starting Price"));
            Assert.That(result.InternalAttributeFieldTitle, Is.EqualTo("StartingPrice"));
            Assert.That(result.IsAscending, Is.True);
        }

        [Test]
        public void Convert_StartingPriceDesc_ReturnsCorrectSortType()
        {
            string sortTag = "StartingPriceDesc";
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExternalAttributeFieldTitle, Is.EqualTo("Starting Price"));
            Assert.That(result.InternalAttributeFieldTitle, Is.EqualTo("StartingPrice"));
            Assert.That(result.IsAscending, Is.False);
        }

        [Test]
        public void Convert_CurrentPriceAsc_ReturnsCorrectSortType()
        {
            string sortTag = "CurrentPriceAsc";
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExternalAttributeFieldTitle, Is.EqualTo("Current Price"));
            Assert.That(result.InternalAttributeFieldTitle, Is.EqualTo("CurrentPrice"));
            Assert.That(result.IsAscending, Is.True);
        }

        [Test]
        public void Convert_CurrentPriceDesc_ReturnsCorrectSortType()
        {
            string sortTag = "CurrentPriceDesc";
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExternalAttributeFieldTitle, Is.EqualTo("Current Price"));
            Assert.That(result.InternalAttributeFieldTitle, Is.EqualTo("CurrentPrice"));
            Assert.That(result.IsAscending, Is.False);
        }

        [Test]
        public void Convert_InvalidSortTag_ReturnsNull()
        {
            string sortTag = "InvalidSortTag";
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_EmptyString_ReturnsNull()
        {
            string sortTag = "";
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_NullString_ReturnsNull()
        {
            string sortTag = null;
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Null);
        }
    }
}