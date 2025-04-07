using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MarketMinds.Repositories.AuctionProductsRepository;
using DomainLayer.Domain;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using MarketMinds.Test.Utils;
using MarketMinds.Repositories;

namespace MarketMinds.Tests.AuctionProductsRepositoryTest
{
    [TestFixture]
    public class AuctionProductRepositoryTest
    {
        private IAuctionProductsRepository auctionProductRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        User testSeller = new User(1, "test buyer", "test email");
        ProductCondition testProductCondition = new ProductCondition(1, "Test", "Test");
        ProductCategory testProductCategory = new ProductCategory(1, "test", "test");
        List<ProductTag> testProductTags = new List<ProductTag>();
        Product auctionProduct, buyProduct;
        List<Image> images = new List<Image>();

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            auctionProductRepository = new AuctionProductsRepository(connection);

            testDbHelper.PrepareTestDatabase();

            testProductTags.Add(new ProductTag(1, "testtag1"));
            testProductTags.Add(new ProductTag(2, "testtag2"));
            images.Add(new Image("https://i.imgur.com/YYXgjHM.jpeg"));
            auctionProduct = new AuctionProduct(1, "Test Product", "Test Description", testSeller, testProductCondition, testProductCategory, testProductTags, images, new DateTime(2025, 04, 04), new DateTime(2030, 04, 04), 100);
            buyProduct = new BuyProduct(1, "Test Product", "Test Description", testSeller, testProductCondition, testProductCategory, testProductTags, images, 100);
        }

        [Test]
        public void TestGetProducts_ReturnsListOfProducts()
        {
            var products = auctionProductRepository.GetProducts();

            Assert.That(products, Is.Not.Null);
            Assert.That(products.GetType(), Is.EqualTo(typeof(List<Product>)));
        }

        [Test]
        public void TestGetProducts_ProcessesDataTableRows_CreatesAuctionProducts()
        {
            auctionProductRepository.AddProduct(auctionProduct);

            var products = auctionProductRepository.GetProducts();

            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count, Is.GreaterThan(0));

            var retrievedProduct = products.FirstOrDefault(p => p.Id == auctionProduct.Id) as AuctionProduct;
            Assert.That(retrievedProduct, Is.Not.Null);
            Assert.That(retrievedProduct.Title, Is.EqualTo(auctionProduct.Title));
            Assert.That(retrievedProduct.Description, Is.EqualTo(auctionProduct.Description));
            Assert.That(retrievedProduct.Seller.Id, Is.EqualTo(auctionProduct.Seller.Id));
            Assert.That(retrievedProduct.Condition.Id, Is.EqualTo(auctionProduct.Condition.Id));
            Assert.That(retrievedProduct.Category.Id, Is.EqualTo(auctionProduct.Category.Id));
        }

        [Test]
        public void TestGetProducts_CorrectlyConvertsDataTypes()
        {
            auctionProductRepository.AddProduct(auctionProduct);

            var products = auctionProductRepository.GetProducts();
            var retrievedProduct = products.FirstOrDefault(p => p.Id == auctionProduct.Id) as AuctionProduct;

            Assert.That(retrievedProduct, Is.Not.Null);

            var auctionProductCasted = auctionProduct as AuctionProduct;
            Assert.That(retrievedProduct.StartingPrice, Is.EqualTo(auctionProductCasted.StartingPrice));
            Assert.That(retrievedProduct.StartingPrice, Is.TypeOf<float>());

            Assert.That(retrievedProduct.StartAuctionDate, Is.EqualTo(auctionProductCasted.StartAuctionDate));
            Assert.That(retrievedProduct.EndAuctionDate, Is.EqualTo(auctionProductCasted.EndAuctionDate));
        }

        [Test]
        public void TestGetProducts_WithMultipleProducts_ReturnsAllProducts()
        {
            var secondProduct = new AuctionProduct(
                2,
                "Second Auction",
                "Another Description",
                testSeller,
                testProductCondition,
                testProductCategory,
                testProductTags,
                images,
                new DateTime(2025, 05, 05),
                new DateTime(2030, 05, 05),
                200);

            auctionProductRepository.AddProduct(auctionProduct);
            auctionProductRepository.AddProduct(secondProduct);

            var products = auctionProductRepository.GetProducts();

            Assert.That(products.Count, Is.GreaterThanOrEqualTo(2));

            var firstRetrieved = products.FirstOrDefault(p => p.Id == auctionProduct.Id);
            var secondRetrieved = products.FirstOrDefault(p => p.Id == secondProduct.Id);

            Assert.That(firstRetrieved, Is.Not.Null);
            Assert.That(secondRetrieved, Is.Not.Null);

            Assert.That(firstRetrieved.Title, Is.EqualTo(auctionProduct.Title));
            Assert.That(secondRetrieved.Title, Is.EqualTo(secondProduct.Title));
        }

        [Test]
        public void TestAddProduct_WrongProductType_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => auctionProductRepository.AddProduct(buyProduct));
        }

        [Test]
        public void TestAddProduct_ValidProduct_AddsProduct()
        {
            Assert.DoesNotThrow(() => auctionProductRepository.AddProduct(auctionProduct));
        }

        [Test]
        public void TestGetProductByID_ValidID_ReturnsProduct()
        {
            auctionProductRepository.AddProduct(auctionProduct);

            int productId = 1;
            var retrievedProduct = auctionProductRepository.GetProductByID(productId);

            Assert.That(retrievedProduct, Is.Not.Null);
            Assert.That(retrievedProduct.Id, Is.EqualTo(productId));
        }

        [Test]
        public void TestIProductsRepositoryGetProductByID_ForwardsToGetProductByID()
        {
            auctionProductRepository.AddProduct(auctionProduct);

            IProductsRepository productsRepository = auctionProductRepository;

            int productId = 1;
            var retrievedProduct = productsRepository.GetProductByID(productId);

            var directRetrievedProduct = auctionProductRepository.GetProductByID(productId);

            Assert.That(retrievedProduct, Is.Not.Null);
            Assert.That(retrievedProduct.Id, Is.EqualTo(productId));

            Assert.That(retrievedProduct.Id, Is.EqualTo(directRetrievedProduct.Id));
            Assert.That(retrievedProduct.Title, Is.EqualTo(directRetrievedProduct.Title));
            Assert.That(retrievedProduct.Description, Is.EqualTo(directRetrievedProduct.Description));

            Assert.That(retrievedProduct, Is.InstanceOf<AuctionProduct>());
        }

        [Test]
        public void TestGetProductByID_InvalidID_ReturnsNull()
        {
            var retrievedProduct = auctionProductRepository.GetProductByID(999);
            Assert.That(retrievedProduct, Is.Null);
        }

        [Test]
        public void TestUpdateProduct_NullProduct_ThrowsException()
        {
            Product product = null;
            Assert.Throws<ArgumentException>(() => auctionProductRepository.UpdateProduct(product));
        }

        [Test]
        public void TestUpdateProductPrice_ValidProductPrice_UpdatesProductPrice()
        {
            auctionProductRepository.AddProduct(auctionProduct);

            AuctionProduct updatedProduct = new AuctionProduct(auctionProduct.Id, auctionProduct.Title, auctionProduct.Description, auctionProduct.Seller, auctionProduct.Condition, auctionProduct.Category, auctionProduct.Tags, auctionProduct.Images, new DateTime(2025, 04, 04), new DateTime(2030, 04, 04), 1000f);

            Assert.DoesNotThrow(() => auctionProductRepository.UpdateProduct(auctionProduct));

            updatedProduct = auctionProductRepository.GetProductByID(auctionProduct.Id);
            Assert.That(updatedProduct, Is.Not.Null, "Product was not found after update");
            Assert.That(updatedProduct.CurrentPrice, Is.EqualTo(100));
        }

        [Test]
        public void TestDeleteProduct_WrongProductType_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => auctionProductRepository.DeleteProduct(buyProduct));
        }

        [Test]
        public void TestDeleteProduct_ValidProduct_DeletesProduct()
        {
            Assert.DoesNotThrow(() => auctionProductRepository.DeleteProduct(auctionProduct));

            var retrievedProduct = auctionProductRepository.GetProductByID(auctionProduct.Id);
            Assert.That(retrievedProduct, Is.Null);
        }
    }
}