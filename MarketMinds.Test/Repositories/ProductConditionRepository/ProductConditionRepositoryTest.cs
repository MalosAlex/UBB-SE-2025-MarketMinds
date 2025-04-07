using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MarketMinds.Repositories.ProductConditionRepository;
using DomainLayer.Domain;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using MarketMinds.Test.Utils;

namespace MarketMinds.Tests.ProductConditionRepositoryTest
{
    [TestFixture]
    public class ProductConditionRepositoryTest
    {
        private IProductConditionRepository productConditionRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            productConditionRepository = new ProductConditionRepository(connection);

            testDbHelper.PrepareTestDatabase();
        }

        [Test]
        public void TestGetAllProductConditions_ReturnsAllConditions()
        {
            var conditions = productConditionRepository.GetAllProductConditions();

            Assert.That(conditions, Is.Not.Null);
            Assert.That(conditions.Count, Is.EqualTo(2));
            Assert.That(conditions.Any(c => c.DisplayTitle == "New"), Is.True);
            Assert.That(conditions.Any(c => c.DisplayTitle == "Used"), Is.True);
        }

        [Test]
        public void TestCreateProductCondition_AddsNewCondition()
        {
            string title = "Refurbished";
            string description = "Item has been restored to working order";

            var newCondition = productConditionRepository.CreateProductCondition(title, description);

            Assert.That(newCondition, Is.Not.Null);
            Assert.That(newCondition.DisplayTitle, Is.EqualTo(title));
            Assert.That(newCondition.Description, Is.EqualTo(description));

            var allConditions = productConditionRepository.GetAllProductConditions();
            Assert.That(allConditions.Count, Is.EqualTo(3));
            Assert.That(allConditions.Any(c => c.DisplayTitle == title), Is.True);
        }

        [Test]
        public void TestDeleteProductCondition_RemovesCondition()
        {
            var conditionsBefore = productConditionRepository.GetAllProductConditions();
            Assert.That(conditionsBefore.Any(c => c.DisplayTitle == "New"), Is.True);

            productConditionRepository.DeleteProductCondition("New");

            var conditionsAfter = productConditionRepository.GetAllProductConditions();
            Assert.That(conditionsAfter.Count, Is.EqualTo(1));
            Assert.That(conditionsAfter.Any(c => c.DisplayTitle == "New"), Is.False);
            Assert.That(conditionsAfter.Any(c => c.DisplayTitle == "Used"), Is.True);
        }

        [Test]
        public void TestCreateProductCondition_WithDuplicateTitle_ThrowsSqlException()
        {
            string title = "New";
            string description = "New description";

            Assert.Throws<Microsoft.Data.SqlClient.SqlException>(() =>
                productConditionRepository.CreateProductCondition(title, description));
        }
    }
}