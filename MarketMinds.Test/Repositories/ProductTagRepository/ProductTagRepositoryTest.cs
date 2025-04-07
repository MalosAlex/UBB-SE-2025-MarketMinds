using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MarketMinds.Repositories.ProductTagRepository;
using DomainLayer.Domain;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using MarketMinds.Test.Utils;

namespace MarketMinds.Tests.ProductTagRepositoryTest
{
    [TestFixture]
    public class ProductTagRepositoryTest
    {
        private IProductTagRepository productTagRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            productTagRepository = new ProductTagRepository(connection);

            testDbHelper.PrepareTestDatabase();
        }

        [Test]
        public void TestGetAllProductTags_ReturnsAllTags()
        {
            var tags = productTagRepository.GetAllProductTags();

            Assert.That(tags, Is.Not.Null);
            Assert.That(tags.Count, Is.EqualTo(3));
            Assert.That(tags.Any(t => t.DisplayTitle == "Tech"), Is.True);
            Assert.That(tags.Any(t => t.DisplayTitle == "Home"), Is.True);
            Assert.That(tags.Any(t => t.DisplayTitle == "Vintage"), Is.True);
        }

        [Test]
        public void TestCreateProductTag_AddsNewTag()
        {
            string title = "Gaming";

            var newTag = productTagRepository.CreateProductTag(title);

            Assert.That(newTag, Is.Not.Null);
            Assert.That(newTag.DisplayTitle, Is.EqualTo(title));

            var allTags = productTagRepository.GetAllProductTags();
            Assert.That(allTags.Count, Is.EqualTo(4));
            Assert.That(allTags.Any(t => t.DisplayTitle == title), Is.True);
        }

        [Test]
        public void TestDeleteProductTag_RemovesTag()
        {
            var tagsBefore = productTagRepository.GetAllProductTags();
            Assert.That(tagsBefore.Any(t => t.DisplayTitle == "Tech"), Is.True);

            productTagRepository.DeleteProductTag("Tech");

            var tagsAfter = productTagRepository.GetAllProductTags();
            Assert.That(tagsAfter.Count, Is.EqualTo(2));
            Assert.That(tagsAfter.Any(t => t.DisplayTitle == "Tech"), Is.False);
            Assert.That(tagsAfter.Any(t => t.DisplayTitle == "Home"), Is.True);
            Assert.That(tagsAfter.Any(t => t.DisplayTitle == "Vintage"), Is.True);
        }

        [Test]
        public void TestCreateProductTag_WithDuplicateTitle_ThrowsSqlException()
        {
            string title = "Tech";

            Assert.Throws<Microsoft.Data.SqlClient.SqlException>(() =>
                productTagRepository.CreateProductTag(title));
        }
    }
}