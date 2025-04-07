using DomainLayer.Domain;
using MarketMinds.Services;
using NUnit.Framework;

namespace MarketMinds.Test.Services
{
    [TestFixture]
    public class SortTypeConverterServiceTest
    {
        private SortTypeConverterService _converter;

        [SetUp]
        public void Setup()
        {
            _converter = new SortTypeConverterService();
        }

        [Test]
        public void Convert_PriceAsc_ReturnsCorrectSortType()
        {
            string sortTag = "PriceAsc";
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExternalAttributeFieldTitle, Is.EqualTo("Price"));
            Assert.That(result.InternalAttributeFieldTitle, Is.EqualTo("Price"));
            Assert.That(result.IsAscending, Is.True);
        }

        [Test]
        public void Convert_PriceDesc_ReturnsCorrectSortType()
        {
            string sortTag = "PriceDesc";
            var result = _converter.Convert(sortTag);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExternalAttributeFieldTitle, Is.EqualTo("Price"));
            Assert.That(result.InternalAttributeFieldTitle, Is.EqualTo("Price"));
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