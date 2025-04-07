using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MarketMinds.Repositories.BorrowProductsRepository;
using DomainLayer.Domain;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using MarketMinds.Test.Utils;
using Microsoft.Data.SqlClient;

namespace MarketMinds.Tests.BorrowProductsRepositoryTest
{
    [TestFixture]
    public class BorrowProductRepositoryTest
    {
        private IBorrowProductsRepository borrowProductRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        User testSeller = new User(1, "test buyer", "test email");
        ProductCondition testProductCondition = new ProductCondition(1, "Test", "Test");
        ProductCategory testProductCategory = new ProductCategory(1, "test", "test");
        List<ProductTag> testProductTags = new List<ProductTag>();
        List<Image> testProductImages = new List<Image>();
        Product borrowProduct, auctionProduct;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            borrowProductRepository = new BorrowProductsRepository(connection);

            testDbHelper.PrepareTestDatabase();

            testProductTags.Add(new ProductTag(1, "Tag1"));
            testProductImages.Add(new Image("https://example.com/image1.jpg"));

            DateTime timeLimit = DateTime.Now.AddDays(7);
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(5);
            bool isBorrowed = false;
            float dailyRate = 20.0f;

            borrowProduct = new BorrowProduct(1, "Test Product", "Test Description", testSeller,
                testProductCondition, testProductCategory, testProductTags, testProductImages,
                timeLimit, startDate, endDate, dailyRate, isBorrowed);

            auctionProduct = new AuctionProduct(1, "Test Product", "Test Description", testSeller, testProductCondition, testProductCategory, testProductTags, new List<Image>(), new DateTime(2025, 04, 04), new DateTime(2030, 04, 04), 100);
        }

        [Test]
        public void TestGetProducts_ReturnsListOfProducts()
        {
            var products = borrowProductRepository.GetProducts();

            Assert.That(products, Is.Not.Null);
            Assert.That(products.GetType(), Is.EqualTo(typeof(List<Product>)));
        }

        [Test]
        public void TestAddProduct_WrongProductType_ThrowsException()
        {
            Assert.Throws<InvalidCastException>(() => borrowProductRepository.AddProduct(auctionProduct));
        }

        [Test]
        public void TestAddProduct_NullProduct_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => borrowProductRepository.AddProduct(null));
        }

        [Test]
        public void TestAddProduct_ValidProduct_AddsProduct()
        {
            Assert.DoesNotThrow(() => borrowProductRepository.AddProduct(borrowProduct));
        }

        [Test]
        public void TestGetProductByID_ValidID_ReturnsProduct()
        {
            borrowProductRepository.AddProduct(borrowProduct);

            int productId = 1;
            var retrievedProduct = borrowProductRepository.GetProductByID(productId);

            Assert.That(retrievedProduct, Is.Not.Null);
            Assert.That(retrievedProduct.Id, Is.EqualTo(productId));
        }

        [Test]
        public void TestGetProductByID_InvalidID_ReturnsNull()
        {
            var retrievedProduct = borrowProductRepository.GetProductByID(999);
            Assert.That(retrievedProduct, Is.Null);
        }

        [Test]
        public void TestDeleteProduct_ValidProduct_DeletesProduct()
        {
            // Create a product specifically for deletion with no images or tags
            DateTime timeLimit = DateTime.Now.AddDays(7);
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(5);
            bool isBorrowed = false;
            float dailyRate = 20.0f;

            var productToDelete = new BorrowProduct(
                0,
                "Product To Delete",
                "This product will be deleted",
                testSeller,
                testProductCondition,
                testProductCategory,
                new List<ProductTag>(),
                new List<Image>(),
                timeLimit,
                startDate,
                endDate,
                dailyRate,
                isBorrowed);

            borrowProductRepository.AddProduct(productToDelete);

            // Get products to find the actual ID
            var products = borrowProductRepository.GetProducts();
            var addedProduct = products.FirstOrDefault(p => p.Title == "Product To Delete");

            Assert.That(addedProduct, Is.Not.Null);

            // Delete associated images and tags first
            DeleteProductImages(addedProduct.Id);
            DeleteProductTags(addedProduct.Id);

            // Now delete the product
            Assert.DoesNotThrow(() => borrowProductRepository.DeleteProduct(addedProduct));
        }

        private void DeleteProductImages(int productId)
        {
            string query = "DELETE FROM BorrowProductImages WHERE product_id = @productId";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@productId", productId);
                cmd.ExecuteNonQuery();
            }
            connection.CloseConnection();
        }

        private void DeleteProductTags(int productId)
        {
            string query = "DELETE FROM BorrowProductProductTags WHERE product_id = @productId";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@productId", productId);
                cmd.ExecuteNonQuery();
            }
            connection.CloseConnection();
        }

        [Test]
        public void TestUpdateProduct_NotImplemented()
        {
            borrowProductRepository.UpdateProduct(borrowProduct);
        }

        [Test]
        public void TestAddProduct_WithTagsAndImages_AddsTagsAndImages()
        {
            var uniqueProductTitle = "Test Product With Tags And Images " + Guid.NewGuid().ToString().Substring(0, 8);
            var productTag1 = new ProductTag(1, "TestTag1");
            var productTag2 = new ProductTag(2, "TestTag2");
            var productImage1 = new Image("https://example.com/image1.jpg");
            var productImage2 = new Image("https://example.com/image2.jpg");

            var testTags = new List<ProductTag> { productTag1, productTag2 };
            var testImages = new List<Image> { productImage1, productImage2 };

            DateTime timeLimit = DateTime.Now.AddDays(10);
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(7);
            bool isBorrowed = false;
            float dailyRate = 25.0f;

            var productWithTagsAndImages = new BorrowProduct(
                0,
                uniqueProductTitle,
                "Product with tags and images for testing",
                testSeller,
                testProductCondition,
                testProductCategory,
                testTags,
                testImages,
                timeLimit,
                startDate,
                endDate,
                dailyRate,
                isBorrowed);

            borrowProductRepository.AddProduct(productWithTagsAndImages);

            var products = borrowProductRepository.GetProducts();
            var retrievedProduct = products.FirstOrDefault(p => p.Title == uniqueProductTitle) as BorrowProduct;

            Assert.That(retrievedProduct, Is.Not.Null);
            Assert.That(retrievedProduct.Tags, Is.Not.Null);
            Assert.That(retrievedProduct.Tags.Count, Is.EqualTo(2));
            Assert.That(retrievedProduct.Images, Is.Not.Null);
            Assert.That(retrievedProduct.Images.Count, Is.EqualTo(2));
            Assert.That(retrievedProduct.Images[0].Url, Is.EqualTo("https://example.com/image1.jpg"));
            Assert.That(retrievedProduct.Images[1].Url, Is.EqualTo("https://example.com/image2.jpg"));
        }

        [Test]
        public void TestGetProducts_LoadsAllProductDetails()
        {
            var uniqueProductTitle = "Test Product Data Loading " + Guid.NewGuid().ToString().Substring(0, 8);

            var productTag = new ProductTag(1, "LoadingTestTag");
            var productImage = new Image("https://example.com/loading-test.jpg");

            DateTime timeLimit = DateTime.Now.AddDays(14);
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(10);
            bool isBorrowed = true;
            float dailyRate = 30.0f;

            var productForDataLoading = new BorrowProduct(
                0,
                uniqueProductTitle,
                "Product for testing data loading",
                testSeller,
                testProductCondition,
                testProductCategory,
                new List<ProductTag> { productTag },
                new List<Image> { productImage },
                timeLimit,
                startDate,
                endDate,
                dailyRate,
                isBorrowed);

            borrowProductRepository.AddProduct(productForDataLoading);

            var products = borrowProductRepository.GetProducts();
            var retrievedProduct = products.FirstOrDefault(p => p.Title == uniqueProductTitle) as BorrowProduct;

            Assert.That(retrievedProduct, Is.Not.Null);
            Assert.That(retrievedProduct.Title, Is.EqualTo(uniqueProductTitle));
            Assert.That(retrievedProduct.Description, Is.EqualTo("Product for testing data loading"));
            Assert.That(retrievedProduct.Seller, Is.Not.Null);
            Assert.That(retrievedProduct.Seller.Id, Is.EqualTo(testSeller.Id));
            Assert.That(retrievedProduct.Condition, Is.Not.Null);
            Assert.That(retrievedProduct.Condition.Id, Is.EqualTo(testProductCondition.Id));
            Assert.That(retrievedProduct.Category, Is.Not.Null);
            Assert.That(retrievedProduct.Category.Id, Is.EqualTo(testProductCategory.Id));
            Assert.That(retrievedProduct.TimeLimit.Date, Is.EqualTo(timeLimit.Date));
            Assert.That(retrievedProduct.StartDate.Date, Is.EqualTo(startDate.Date));
            Assert.That(retrievedProduct.EndDate.Date, Is.EqualTo(endDate.Date));
            Assert.That(retrievedProduct.IsBorrowed, Is.EqualTo(isBorrowed));
            Assert.That(Math.Abs(retrievedProduct.DailyRate - dailyRate) < 0.001);
        }

        [Test]
        public void TestGetProductByID_LoadsTagsAndImages()
        {
            var uniqueProductTitle = "Product For GetById " + Guid.NewGuid().ToString().Substring(0, 8);

            var productTag = new ProductTag(1, "GetByIdTag");
            var productImage = new Image("https://example.com/getbyid-test.jpg");

            DateTime timeLimit = DateTime.Now.AddDays(5);
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(3);
            bool isBorrowed = false;
            float dailyRate = 15.0f;

            var productForGetById = new BorrowProduct(
                0,
                uniqueProductTitle,
                "Product for testing GetById with tags/images",
                testSeller,
                testProductCondition,
                testProductCategory,
                new List<ProductTag> { productTag },
                new List<Image> { productImage },
                timeLimit,
                startDate,
                endDate,
                dailyRate,
                isBorrowed);

            borrowProductRepository.AddProduct(productForGetById);

            var products = borrowProductRepository.GetProducts();
            var addedProduct = products.FirstOrDefault(p => p.Title == uniqueProductTitle);

            Assert.That(addedProduct, Is.Not.Null);
            Assert.That(addedProduct.Id, Is.GreaterThan(0));

            var retrievedProductById = borrowProductRepository.GetProductByID(addedProduct.Id) as BorrowProduct;

            Assert.That(retrievedProductById, Is.Not.Null);
            Assert.That(retrievedProductById.Title, Is.EqualTo(uniqueProductTitle));
            Assert.That(retrievedProductById.Tags, Is.Not.Null);
            Assert.That(retrievedProductById.Tags.Count, Is.EqualTo(1));
            Assert.That(retrievedProductById.Images, Is.Not.Null);
            Assert.That(retrievedProductById.Images.Count, Is.EqualTo(1));
            Assert.That(retrievedProductById.Images[0].Url, Is.EqualTo("https://example.com/getbyid-test.jpg"));
        }
    }
}
