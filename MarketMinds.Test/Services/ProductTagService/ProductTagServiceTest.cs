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
        private IProductTagService service;

        [SetUp]
        public void Setup()
        {
            repositoryMock = new ProductTagRepositoryMock();
            service = new MarketMinds.Services.ProductTagService.ProductTagService(repositoryMock);
        }

        [Test]
        public void GetAllProductTags_ShouldReturnAllTags()
        {
            // Arrange
            repositoryMock.Tags.AddRange(new List<ProductTag>
            {
                new ProductTag(1, "Electronics"),
                new ProductTag(2, "Clothing"),
                new ProductTag(3, "Books")
            });

            // Act
            var result = service.GetAllProductTags();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].DisplayTitle, Is.EqualTo("Electronics"));
            Assert.That(result[1].DisplayTitle, Is.EqualTo("Clothing"));
            Assert.That(result[2].DisplayTitle, Is.EqualTo("Books"));
        }

        [Test]
        public void GetAllProductTags_WithEmptyRepository_ShouldReturnEmptyList()
        {
            // Act
            var result = service.GetAllProductTags();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void CreateProductTag_ShouldReturnCreatedTag()
        {
            // Arrange
            string tagTitle = "Gaming";

            // Act
            var result = service.CreateProductTag(tagTitle);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DisplayTitle, Is.EqualTo(tagTitle));
            Assert.That(result.Id, Is.EqualTo(1)); // First created tag will have ID 1
            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(1));
            Assert.That(repositoryMock.Tags.First().DisplayTitle, Is.EqualTo(tagTitle));
        }

        [Test]
        public void CreateProductTag_MultipleTags_ShouldAssignUniqueIds()
        {
            // Act
            var tag1 = service.CreateProductTag("Electronics");
            var tag2 = service.CreateProductTag("Clothing");
            var tag3 = service.CreateProductTag("Books");

            // Assert
            Assert.That(tag1.Id, Is.EqualTo(1));
            Assert.That(tag2.Id, Is.EqualTo(2));
            Assert.That(tag3.Id, Is.EqualTo(3));
            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(3));
            Assert.That(tag1.Id, Is.Not.EqualTo(tag2.Id));
            Assert.That(tag2.Id, Is.Not.EqualTo(tag3.Id));
        }

        [Test]
        public void DeleteProductTag_ExistingTag_ShouldRemoveTagFromRepository()
        {
            // Arrange
            string tagTitle = "Electronics";
            repositoryMock.Tags.Add(new ProductTag(1, tagTitle));
            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(1));

            // Act
            service.DeleteProductTag(tagTitle);

            // Assert
            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(0));
            Assert.That(repositoryMock.Tags.Any(t => t.DisplayTitle == tagTitle), Is.False);
        }

        [Test]
        public void DeleteProductTag_NonExistentTag_ShouldNotChangeRepository()
        {
            // Arrange
            string existingTagTitle = "Electronics";
            string nonExistentTagTitle = "Furniture";
            repositoryMock.Tags.Add(new ProductTag(1, existingTagTitle));
            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(1));

            // Act
            service.DeleteProductTag(nonExistentTagTitle);

            // Assert
            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(1));
            Assert.That(repositoryMock.Tags.First().DisplayTitle, Is.EqualTo(existingTagTitle));
        }

        [Test]
        public void DeleteProductTag_WithMultipleTagsWithSameTitle_ShouldRemoveAllMatches()
        {
            // Arrange
            string duplicateTitle = "Electronics";
            string uniqueTitle = "Books";

            repositoryMock.Tags.AddRange(new List<ProductTag>
            {
                new ProductTag(1, duplicateTitle),
                new ProductTag(2, uniqueTitle),
                new ProductTag(3, duplicateTitle)
            });
            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(3));

            // Act
            service.DeleteProductTag(duplicateTitle);

            // Assert
            Assert.That(repositoryMock.Tags.Count, Is.EqualTo(1));
            Assert.That(repositoryMock.Tags.First().DisplayTitle, Is.EqualTo(uniqueTitle));
            Assert.That(repositoryMock.Tags.Any(t => t.DisplayTitle == duplicateTitle), Is.False);
        }
    }
}