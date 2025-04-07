using DomainLayer.Domain;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketMinds.Test.Services.ProductTagService
{
    [TestFixture]
    internal class ProductServiceTests
    {
        private class TestProduct : Product
        {
        }

        [Test]
        public void GetProducts_ShouldReturnAllProducts()
        {
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            var product1 = CreateSampleProduct(1, "Laptop");
            var product2 = CreateSampleProduct(2, "Smartphone");
            var product3 = CreateSampleProduct(3, "Tablet");

            mockRepository.AddProduct(product1);
            mockRepository.AddProduct(product2);
            mockRepository.AddProduct(product3);

            var result = productService.GetProducts();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.Any(p => p.Id == 1 && p.Title == "Laptop"), Is.True);
            Assert.That(result.Any(p => p.Id == 2 && p.Title == "Smartphone"), Is.True);
            Assert.That(result.Any(p => p.Id == 3 && p.Title == "Tablet"), Is.True);
        }

        [Test]
        public void GetProductById_ShouldReturnCorrectProduct()
        {
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            var product1 = CreateSampleProduct(1, "Laptop");
            var product2 = CreateSampleProduct(2, "Smartphone");

            mockRepository.AddProduct(product1);
            mockRepository.AddProduct(product2);

            var result = productService.GetProductById(2);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(2));
            Assert.That(result.Title, Is.EqualTo("Smartphone"));
        }

        [Test]
        public void AddProduct_ShouldAddProductToRepository()
        {
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);
            var product = CreateSampleProduct(1, "Headphones");

            productService.AddProduct(product);

            Assert.That(mockRepository.Products.Count, Is.EqualTo(1));
            Assert.That(mockRepository.Products[0].Id, Is.EqualTo(1));
            Assert.That(mockRepository.Products[0].Title, Is.EqualTo("Headphones"));
        }

        [Test]
        public void DeleteProduct_ShouldRemoveProductFromRepository()
        {
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);
            var product = CreateSampleProduct(1, "Keyboard");

            mockRepository.AddProduct(product);
            Assert.That(mockRepository.Products.Count, Is.EqualTo(1));

            productService.DeleteProduct(product);

            Assert.That(mockRepository.Products.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetSortedFilteredProducts_EmptyProductList_ReturnsEmptyList()
        {
            var products = new List<Product>();
            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(products, null, null, null, null, null);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetSortedFilteredProducts_WithNullFirstParameter_ShouldThrowException()
        {
            Assert.Throws<NullReferenceException>(() =>
                MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(null, null, null, null, null, null));
        }

        [Test]
        public void GetSortedFilteredProducts_WithMultipleConditions_ShouldFilterByAllConditions()
        {
            var condition1 = new ProductCondition(1, "New", "Brand new");
            var condition2 = new ProductCondition(2, "Used", "Used item");

            var product1 = CreateSampleProduct(1, "Product1", condition: condition1);
            var product2 = CreateSampleProduct(2, "Product2", condition: condition2);
            var product3 = CreateSampleProduct(3, "Product3", condition: condition1);

            var products = new List<Product> { product1, product2, product3 };
            var selectedConditions = new List<ProductCondition> { condition1, condition2 };

            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, selectedConditions, null, null, null, null);

            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetSortedFilteredProducts_WithMultipleCategories_ShouldFilterByAllCategories()
        {
            var category1 = new ProductCategory(1, "Electronics", "Electronic devices");
            var category2 = new ProductCategory(2, "Clothing", "Apparel");

            var product1 = CreateSampleProduct(1, "Product1", category: category1);
            var product2 = CreateSampleProduct(2, "Product2", category: category2);
            var product3 = CreateSampleProduct(3, "Product3", category: category1);

            var products = new List<Product> { product1, product2, product3 };
            var selectedCategories = new List<ProductCategory> { category1, category2 };

            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, null, selectedCategories, null, null, null);

            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetSortedFilteredProducts_WithMultipleTags_ShouldFilterByAnyTag()
        {
            var tag1 = new ProductTag(1, "Wireless");
            var tag2 = new ProductTag(2, "Bluetooth");

            var product1 = CreateSampleProduct(1, "Product1");
            product1.Tags.Add(tag1);

            var product2 = CreateSampleProduct(2, "Product2");
            product2.Tags.Add(tag2);

            var product3 = CreateSampleProduct(3, "Product3");
            product3.Tags.Add(tag1);
            product3.Tags.Add(tag2);

            var products = new List<Product> { product1, product2, product3 };
            var selectedTags = new List<ProductTag> { tag1, tag2 };

            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, null, null, selectedTags, null, null);

            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetSortedFilteredProducts_WithCaseInsensitiveSearch_ShouldFindMatches()
        {
            var products = new List<Product>
            {
                CreateSampleProduct(1, "LAPTOP Computer"),
                CreateSampleProduct(2, "Desktop computer"),
                CreateSampleProduct(3, "Tablet")
            };

            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, null, null, null, null, "computer");

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(p => p.Title.ToLower().Contains("computer")), Is.True);
        }

        [Test]
        public void GetSortedFilteredProducts_WithAllFilterTypes_ShouldFilterCorrectly()
        {
            var category = new ProductCategory(1, "Electronics", "Electronic devices");
            var condition = new ProductCondition(1, "New", "Brand new");
            var tag = new ProductTag(1, "Wireless");

            var product1 = CreateSampleProduct(1, "Gaming Computer", category, condition);
            product1.Tags.Add(tag);

            var product2 = CreateSampleProduct(2, "Wireless Mouse", category, condition);
            product2.Tags.Add(tag);

            var product3 = CreateSampleProduct(3, "Wired Keyboard", category, condition);

            var product4 = CreateSampleProduct(4, "Used Laptop", category, new ProductCondition(2, "Used", "Used item"));
            product4.Tags.Add(tag);

            var products = new List<Product> { product1, product2, product3, product4 };

            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products,
                new List<ProductCondition> { condition },
                new List<ProductCategory> { category },
                new List<ProductTag> { tag },
                null,
                "wireless"
            );

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Wireless Mouse"));
        }

        [Test]
        public void GetSortedFilteredProducts_WithSortingId_ShouldSortById()
        {
            var products = new List<Product>
            {
                CreateSampleProduct(1, "Product A"),
                CreateSampleProduct(2, "Product B"),
                CreateSampleProduct(3, "Product C")
            };

            var sortCondition = new ProductSortType("Id", "Id", true);

            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, null, null, null, sortCondition, null);

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[2].Id, Is.EqualTo(3));
        }

        [Test]
        public void GetSortedFilteredProducts_WithSortingDescendingId_ShouldSortByIdDescending()
        {
            var products = new List<Product>
            {
                CreateSampleProduct(1, "Product A"),
                CreateSampleProduct(2, "Product B"),
                CreateSampleProduct(3, "Product C")
            };

            var sortCondition = new ProductSortType("Id", "Id", false);

            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, null, null, null, sortCondition, null);

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Id, Is.EqualTo(3));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[2].Id, Is.EqualTo(1));
        }

        [Test]
        public void GetSortedFilteredProducts_WithSortingAndFiltering_ShouldFilterThenSort()
        {
            var tag1 = new ProductTag(1, "Office");
            var tag2 = new ProductTag(2, "Gaming");

            var product1 = CreateSampleProduct(1, "Office Keyboard");
            product1.Tags.Add(tag1);

            var product2 = CreateSampleProduct(2, "Office Chair");
            product2.Tags.Add(tag1);

            var product3 = CreateSampleProduct(3, "Gaming Mouse");
            product3.Tags.Add(tag2);

            var products = new List<Product> { product1, product2, product3 };
            var selectedTags = new List<ProductTag> { tag1 };
            var sortCondition = new ProductSortType("Title", "Title", true);

            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, null, null, selectedTags, sortCondition, null);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Title, Is.EqualTo("Office Chair"));
            Assert.That(result[1].Title, Is.EqualTo("Office Keyboard"));
        }

        [Test]
        public void GetSortedFilteredProducts_WithFilteringAndNullSort_ShouldOnlyFilter()
        {
            var condition = new ProductCondition(1, "New", "Brand new");

            var product1 = CreateSampleProduct(1, "Product A", condition: condition);
            var product2 = CreateSampleProduct(2, "Product B", condition: new ProductCondition(2, "Used", "Used item"));
            var product3 = CreateSampleProduct(3, "Product C", condition: condition);

            var products = new List<Product> { product1, product2, product3 };
            var selectedConditions = new List<ProductCondition> { condition };

            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, selectedConditions, null, null, null, null);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(p => p.Condition.Id == condition.Id), Is.True);
        }

        [Test]
        public void GetSortedFilteredProducts_WithPartialWordSearch_ShouldMatchPartialWords()
        {
            var products = new List<Product>
            {
                CreateSampleProduct(1, "Gaming Laptop"),
                CreateSampleProduct(2, "Business Desktop"),
                CreateSampleProduct(3, "Graphic Tablet")
            };

            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, null, null, null, null, "top");

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(p => p.Title.ToLower().Contains("top")), Is.True);
        }

        [Test]
        public void GetSortedFilteredProducts_WithComplexSorting_ShouldHandleComplexProperties()
        {
            var category1 = new ProductCategory(1, "A Category", "First category");
            var category2 = new ProductCategory(2, "B Category", "Second category");

            var product1 = CreateSampleProduct(1, "Product 1", category2);
            var product2 = CreateSampleProduct(2, "Product 2", category1);
            var product3 = CreateSampleProduct(3, "Product 3", category1);

            var products = new List<Product> { product1, product2, product3 };
            var sortCondition = new ProductSortType("CategoryName", "Category.DisplayTitle", true);
            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, null, null, null, sortCondition, null);

            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetSortedFilteredProducts_WithSpecificConditionFilter_ShouldReturnMatchingProducts()
        {
            // Arrange
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            var newCondition = new ProductCondition(1, "New", "Brand new");
            var usedCondition = new ProductCondition(2, "Used", "Used item");
            var refurbishedCondition = new ProductCondition(3, "Refurbished", "Refurbished item");

            var product1 = CreateSampleProduct(1, "Laptop", condition: newCondition);
            var product2 = CreateSampleProduct(2, "Phone", condition: usedCondition);
            var product3 = CreateSampleProduct(3, "Tablet", condition: refurbishedCondition);
            var product4 = CreateSampleProduct(4, "Desktop", condition: newCondition);

            mockRepository.AddProduct(product1);
            mockRepository.AddProduct(product2);
            mockRepository.AddProduct(product3);
            mockRepository.AddProduct(product4);

            var selectedConditions = new List<ProductCondition> { newCondition };

            // Act
            var result = productService.GetSortedFilteredProducts(selectedConditions, null, null, null, null);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(p => p.Condition.Id == newCondition.Id), Is.True);
            Assert.That(result.Select(p => p.Title).Contains("Laptop"), Is.True);
            Assert.That(result.Select(p => p.Title).Contains("Desktop"), Is.True);
        }

        [Test]
        public void GetSortedFilteredProducts_WithSpecificCategoryFilter_ShouldReturnMatchingProducts()
        {
            // Arrange
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            var electronicsCategory = new ProductCategory(1, "Electronics", "Electronic devices");
            var clothingCategory = new ProductCategory(2, "Clothing", "Apparel items");

            var product1 = CreateSampleProduct(1, "Laptop", category: electronicsCategory);
            var product2 = CreateSampleProduct(2, "T-shirt", category: clothingCategory);
            var product3 = CreateSampleProduct(3, "Jeans", category: clothingCategory);
            var product4 = CreateSampleProduct(4, "Headphones", category: electronicsCategory);

            mockRepository.AddProduct(product1);
            mockRepository.AddProduct(product2);
            mockRepository.AddProduct(product3);
            mockRepository.AddProduct(product4);

            var selectedCategories = new List<ProductCategory> { clothingCategory };

            // Act
            var result = productService.GetSortedFilteredProducts(null, selectedCategories, null, null, null);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(p => p.Category.Id == clothingCategory.Id), Is.True);
            Assert.That(result.Select(p => p.Title).Contains("T-shirt"), Is.True);
            Assert.That(result.Select(p => p.Title).Contains("Jeans"), Is.True);
        }

        [Test]
        public void GetSortedFilteredProducts_WithSpecificTagFilter_ShouldReturnMatchingProducts()
        {
            // Arrange
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            var wirelessTag = new ProductTag(1, "Wireless");
            var bluetoothTag = new ProductTag(2, "Bluetooth");
            var waterproofTag = new ProductTag(3, "Waterproof");

            var product1 = CreateSampleProduct(1, "Headphones");
            product1.Tags.Add(wirelessTag);
            product1.Tags.Add(bluetoothTag);

            var product2 = CreateSampleProduct(2, "Speaker");
            product2.Tags.Add(bluetoothTag);
            product2.Tags.Add(waterproofTag);

            var product3 = CreateSampleProduct(3, "Mouse");
            product3.Tags.Add(wirelessTag);

            var product4 = CreateSampleProduct(4, "Keyboard");
            product4.Tags.Add(wirelessTag);

            mockRepository.AddProduct(product1);
            mockRepository.AddProduct(product2);
            mockRepository.AddProduct(product3);
            mockRepository.AddProduct(product4);

            var selectedTags = new List<ProductTag> { wirelessTag };

            // Act
            var result = productService.GetSortedFilteredProducts(null, null, selectedTags, null, null);

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.All(p => p.Tags.Any(t => t.Id == wirelessTag.Id)), Is.True);
            Assert.That(result.Select(p => p.Title).Contains("Headphones"), Is.True);
            Assert.That(result.Select(p => p.Title).Contains("Mouse"), Is.True);
            Assert.That(result.Select(p => p.Title).Contains("Keyboard"), Is.True);
        }

        [Test]
        public void GetSortedFilteredProducts_WithSearchQuery_ShouldReturnMatchingProducts()
        {
            // Arrange
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            mockRepository.AddProduct(CreateSampleProduct(1, "Gaming Laptop"));
            mockRepository.AddProduct(CreateSampleProduct(2, "Business Laptop"));
            mockRepository.AddProduct(CreateSampleProduct(3, "Gaming Desktop"));
            mockRepository.AddProduct(CreateSampleProduct(4, "Smartphone"));

            // Act
            var result = productService.GetSortedFilteredProducts(null, null, null, null, "laptop");

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(p => p.Title.ToLower().Contains("laptop")), Is.True);
            Assert.That(result.Select(p => p.Title).Contains("Gaming Laptop"), Is.True);
            Assert.That(result.Select(p => p.Title).Contains("Business Laptop"), Is.True);
        }

        [Test]
        public void GetSortedFilteredProducts_WithTitleSorting_ShouldSortByTitle()
        {
            // Arrange
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            mockRepository.AddProduct(CreateSampleProduct(1, "Zebra"));
            mockRepository.AddProduct(CreateSampleProduct(2, "Apple"));
            mockRepository.AddProduct(CreateSampleProduct(3, "Banana"));
            mockRepository.AddProduct(CreateSampleProduct(4, "Xylophone"));

            var sortCondition = new ProductSortType("Title", "Title", true);

            // Act
            var result = productService.GetSortedFilteredProducts(null, null, null, sortCondition, null);

            // Assert
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result[0].Title, Is.EqualTo("Apple"));
            Assert.That(result[1].Title, Is.EqualTo("Banana"));
            Assert.That(result[2].Title, Is.EqualTo("Xylophone"));
            Assert.That(result[3].Title, Is.EqualTo("Zebra"));
        }

        [Test]
        public void GetSortedFilteredProducts_WithTitleSortingDescending_ShouldSortByTitleDescending()
        {
            // Arrange
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            mockRepository.AddProduct(CreateSampleProduct(1, "Zebra"));
            mockRepository.AddProduct(CreateSampleProduct(2, "Apple"));
            mockRepository.AddProduct(CreateSampleProduct(3, "Banana"));
            mockRepository.AddProduct(CreateSampleProduct(4, "Xylophone"));

            var sortCondition = new ProductSortType("Title", "Title", false);

            // Act
            var result = productService.GetSortedFilteredProducts(null, null, null, sortCondition, null);

            // Assert
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result[0].Title, Is.EqualTo("Zebra"));
            Assert.That(result[1].Title, Is.EqualTo("Xylophone"));
            Assert.That(result[2].Title, Is.EqualTo("Banana"));
            Assert.That(result[3].Title, Is.EqualTo("Apple"));
        }

        [Test]
        public void GetSortedFilteredProducts_WithMultipleFilters_ShouldApplyAllFilters()
        {
            // Arrange
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            var electronicsCategory = new ProductCategory(1, "Electronics", "Electronic devices");
            var newCondition = new ProductCondition(1, "New", "Brand new");
            var wirelessTag = new ProductTag(1, "Wireless");

            var product1 = CreateSampleProduct(1, "Wireless Gaming Mouse", electronicsCategory, newCondition);
            product1.Tags.Add(wirelessTag);

            var product2 = CreateSampleProduct(2, "Wired Gaming Mouse", electronicsCategory, newCondition);

            var product3 = CreateSampleProduct(3, "Wireless Keyboard", electronicsCategory, newCondition);
            product3.Tags.Add(wirelessTag);

            mockRepository.AddProduct(product1);
            mockRepository.AddProduct(product2);
            mockRepository.AddProduct(product3);

            var selectedCategories = new List<ProductCategory> { electronicsCategory };
            var selectedConditions = new List<ProductCondition> { newCondition };
            var selectedTags = new List<ProductTag> { wirelessTag };

            // Act
            var result = productService.GetSortedFilteredProducts(selectedConditions, selectedCategories, selectedTags, null, "mouse");

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Wireless Gaming Mouse"));
        }

        [Test]
        public void GetSortedFilteredProducts_WithEmptyResults_ShouldReturnEmptyList()
        {
            // Arrange
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            var product1 = CreateSampleProduct(1, "Laptop");
            var product2 = CreateSampleProduct(2, "Desktop");

            mockRepository.AddProduct(product1);
            mockRepository.AddProduct(product2);

            // Act
            var result = productService.GetSortedFilteredProducts(null, null, null, null, "nonexistent");

            // Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetSortedFilteredProducts_WithNullRepository_ShouldHandleGracefully()
        {
            // This test directly calls the static method without a repository

            // Arrange
            var product1 = CreateSampleProduct(1, "Product A");
            var product2 = CreateSampleProduct(2, "Product B");
            var products = new List<Product> { product1, product2 };

            // Act
            var result = MarketMinds.Services.ProductTagService.ProductService.GetSortedFilteredProducts(
                products, null, null, null, null, null);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        private Product CreateSampleProduct(int id, string title, ProductCategory category = null, ProductCondition condition = null)
        {
            return new TestProduct
            {
                Id = id,
                Title = title,
                Description = "Test product description",
                Category = category ?? new ProductCategory(1, "Default Category", "Default category description"),
                Condition = condition ?? new ProductCondition(1, "New", "Unused product in original packaging"),
                Tags = new List<ProductTag>(),
                Images = new List<Image>()
            };
        }

        [Test]
        public void UpdateProduct_ShouldUpdateProductInRepository()
        {
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            var productToUpdate = CreateSampleProduct(1, "Initial Name");
            mockRepository.AddProduct(productToUpdate);

            productToUpdate.Title = "Updated Name";

            productService.UpdateProduct(productToUpdate);

            var updatedProduct = mockRepository.Products.FirstOrDefault(p => p.Id == 1);
            Assert.That(updatedProduct, Is.Not.Null);
            Assert.That(updatedProduct.Title, Is.EqualTo("Updated Name"));
            Assert.That(mockRepository.Products.Count, Is.EqualTo(1));
        }
    }
}