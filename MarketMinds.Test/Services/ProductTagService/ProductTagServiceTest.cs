using DomainLayer.Domain;
using MarketMinds.Repositories.ProductTagRepository;
using MarketMinds.Services.ProductTagService;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MarketMinds.Test.Services.ProductTagService
{
    [TestFixture]
    public class ProductTagServiceTests
    {
        private ProductTagRepositoryMock repositoryMock;
        private MarketMinds.Services.ProductTagService.ProductTagService service;

        [SetUp]
        public void Setup()
        {
            repositoryMock = new ProductTagRepositoryMock();
            service = new MarketMinds.Services.ProductTagService.ProductTagService(repositoryMock);
        }

        [Test]
        public void GetAllProductTags_ShouldReturnAllTags()
        {
            repositoryMock.Tags.AddRange(new List<ProductTag>
            {
                new ProductTag(1, "Electronics"),
                new ProductTag(2, "Clothing"),
                new ProductTag(3, "Books")
            });

            var result = service.GetAllProductTags();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(t => t.DisplayTitle == "Electronics"), Is.True);
            Assert.That(result.Any(t => t.DisplayTitle == "Clothing"), Is.True);
            Assert.That(result.Any(t => t.DisplayTitle == "Books"), Is.True);

            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(3));
        }

        [Test]
        public void CreateProductTag_ShouldReturnCreatedTag()
        {
            string tagTitle = "Gaming";

            var result = service.CreateProductTag(tagTitle);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DisplayTitle, Is.EqualTo(tagTitle));
            Assert.That(result.Id, Is.EqualTo(1)); // First created tag will have ID 1

            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(1));
            Assert.That(repositoryMock.Tags.First().DisplayTitle, Is.EqualTo(tagTitle));
        }

        [Test]
        public void DeleteProductTag_ShouldCallRepositoryMethod()
        {
            string tagTitle = "Electronics";
            repositoryMock.Tags.Add(new ProductTag(1, tagTitle));

            service.DeleteProductTag(tagTitle);

            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(0));
            Assert.That(repositoryMock.Tags.Any(t => t.DisplayTitle == tagTitle), Is.False);
        }
    }
}