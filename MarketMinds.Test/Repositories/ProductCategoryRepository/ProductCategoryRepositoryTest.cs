using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MarketMinds.Repositories.ProductCategoryRepository;
using DomainLayer.Domain;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using MarketMinds.Test.Utils;
using Microsoft.Data.SqlClient;

namespace MarketMinds.Tests.ProductCategoryRepositoryTest
{
    [TestFixture]
    public class ProductCategoryRepositoryTest
    {
        private IProductCategoryRepository productCategoryRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            productCategoryRepository = new ProductCategoryRepository(connection);

            testDbHelper.PrepareTestDatabase();
        }

        [Test]
        public void TestGetAllProductCategories_ReturnsAllCategories()
        {
            var categories = productCategoryRepository.GetAllProductCategories();

            Assert.That(categories, Is.Not.Null);
            Assert.That(categories.Count, Is.EqualTo(2));
            Assert.That(categories.Any(c => c.DisplayTitle == "Electronics"), Is.True);
            Assert.That(categories.Any(c => c.DisplayTitle == "Furniture"), Is.True);
        }

        [Test]
        public void TestCreateProductCategory_AddsNewCategory()
        {
            string title = "Books";
            string description = "All types of books";

            var newCategory = productCategoryRepository.CreateProductCategory(title, description);

            Assert.That(newCategory, Is.Not.Null);
            Assert.That(newCategory.DisplayTitle, Is.EqualTo(title));
            Assert.That(newCategory.Description, Is.EqualTo(description));

            var allCategories = productCategoryRepository.GetAllProductCategories();
            Assert.That(allCategories.Count, Is.EqualTo(3));
            Assert.That(allCategories.Any(c => c.DisplayTitle == title), Is.True);
        }

        [Test]
        public void TestDeleteProductCategory_RemovesCategory()
        {
            var categoriesBefore = productCategoryRepository.GetAllProductCategories();
            Assert.That(categoriesBefore.Any(c => c.DisplayTitle == "Electronics"), Is.True);

            productCategoryRepository.DeleteProductCategory("Electronics");

            var categoriesAfter = productCategoryRepository.GetAllProductCategories();
            Assert.That(categoriesAfter.Count, Is.EqualTo(1));
            Assert.That(categoriesAfter.Any(c => c.DisplayTitle == "Electronics"), Is.False);
            Assert.That(categoriesAfter.Any(c => c.DisplayTitle == "Furniture"), Is.True);
        }

        [Test]
        public void TestCreateProductCategory_WithDuplicateTitle_ThrowsSqlException()
        {
            string title = "Electronics";
            string description = "New description";

            Assert.Throws<SqlException>(() =>
                productCategoryRepository.CreateProductCategory(title, description));
        }
    }
}