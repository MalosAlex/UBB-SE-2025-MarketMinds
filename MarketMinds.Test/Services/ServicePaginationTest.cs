using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using MarketMinds.Services;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace MarketMinds.Test.Services
{
    [TestFixture]
    internal class ProductPaginationServiceTest
    {
        private ProductPaginationService paginationService;
        private const int ItemsPerPage = 3; // for testing purposes

        [SetUp]
        public void SetUp()
        {
            paginationService = new ProductPaginationService(ItemsPerPage);
        }

        #region GetPaginatedProducts Tests

        [Test]
        public void GetPaginatedProducts_NormalCase_ReturnsCorrectPageAndTotalPages()
        {
            // Arrange
            List<int> products = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };

            // Act: Request page 2 (should return items 4,5,6)
            var (currentPage, totalPages) = paginationService.GetPaginatedProducts(products, 2);

            // Assert
            Assert.That(currentPage.Count, Is.EqualTo(ItemsPerPage));
            CollectionAssert.AreEqual(new List<int> { 4, 5, 6 }, currentPage);
            // Total pages: 8 / 3 = 2.67, ceil -> 3 pages.
            Assert.That(totalPages, Is.EqualTo(3));
        }

        [Test]
        public void GetPaginatedProducts_CurrentPageExceedsTotalPages_ReturnsLastPage()
        {
            // Arrange
            List<int> products = new List<int> { 10, 20, 30, 40, 50 };

            // Act: Request page 10 which is greater than total pages (5/3 = 1.67, ceil -> 2)
            var (currentPage, totalPages) = paginationService.GetPaginatedProducts(products, 10);

            // Assert
            // Expected last page: page 2 should have 2 items (items 4 and 5)
            Assert.That(currentPage.Count, Is.EqualTo(2));
            CollectionAssert.AreEqual(new List<int> { 40, 50 }, currentPage);
            Assert.That(totalPages, Is.EqualTo(2));
        }

        [Test]
        public void GetPaginatedProducts_AllProductsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            List<int> products = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => paginationService.GetPaginatedProducts(products, 1));
            Assert.That(ex.ParamName, Is.EqualTo("allProducts"));
        }

        [Test]
        public void GetPaginatedProducts_CurrentPageLessThanBase_ThrowsArgumentException()
        {
            // Arrange
            List<int> products = new List<int> { 1, 2, 3 };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => paginationService.GetPaginatedProducts(products, 0));
            Assert.That(ex.ParamName, Is.EqualTo("currentPage"));
        }

        #endregion

        #region ApplyFilters Tests

        [Test]
        public void ApplyFilters_NormalCase_ReturnsFilteredProducts()
        {
            // Arrange
            List<int> products = new List<int> { 1, 2, 3, 4, 5 };
            // Filter to return only even numbers.
            Func<int, bool> evenFilter = p => p % 2 == 0;

            // Act
            var filteredProducts = paginationService.ApplyFilters(products, evenFilter);

            // Assert
            CollectionAssert.AreEqual(new List<int> { 2, 4 }, filteredProducts);
        }

        [Test]
        public void ApplyFilters_ProductsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            List<int> products = null;
            Func<int, bool> predicate = p => true;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => paginationService.ApplyFilters(products, predicate));
            Assert.That(ex.ParamName, Is.EqualTo("products"));
        }

        [Test]
        public void ApplyFilters_PredicateIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            List<int> products = new List<int> { 1, 2, 3 };

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => paginationService.ApplyFilters(products, null));
            Assert.That(ex.ParamName, Is.EqualTo("filterPredicate"));
        }

        #endregion
    }
}
