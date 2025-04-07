using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DomainLayer.Domain;
using MarketMinds.Repositories.AuctionProductsRepository;
using MarketMinds.Repositories.BasketRepository;
using MarketMinds.Repositories.BuyProductsRepository;
using MarketMinds.Test.Utils;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace MarketMinds.Test.Repositories.BasketRepositoryTest
{
    [TestFixture]
    public class BasketRepositoryTest
    {
        private IBasketRepository basketRepository;
        private IBuyProductsRepository buyProductRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        User testUser = new User(1, "test buyer", "test email");
        Basket testBasket;
        Product testProduct;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            basketRepository = new BasketRepository(connection);
            buyProductRepository = new BuyProductsRepository(connection);

            testDbHelper.PrepareTestDatabase();

            testProduct = new BuyProduct(1, "Test Product", "Test Description", new User(2, "test seller", "test seller email"), new ProductCondition(1, "New", "New"), new ProductCategory(1, "Electronics", "Electic"), new List<ProductTag>(), new List<Image>(), 100);
            buyProductRepository.AddProduct(testProduct);

            testBasket = basketRepository.GetBasketByUserId(testUser.Id);
        }

        [Test]
        public void TestGetBasketByUserId_InvalidUserId_ThrowsException()
        {
            int invalidUserId = -1;

            Assert.Catch<Microsoft.Data.SqlClient.SqlException>(() => basketRepository.GetBasketByUserId(invalidUserId));
        }

        [Test]
        public void TestGetBasketByUserId_ValidUserId_RetursnBasketWithItems()
        {
            int userId = testUser.Id;

            testBasket = basketRepository.GetBasketByUserId(userId);

            Assert.That(testBasket, Is.Not.Null);
        }

        [Test]
        public void TestGetBasketByUserId_CreateBasketForNewUser_ReturnsBasket()
        {
            int existingUserId = 2;

            var basket = basketRepository.GetBasketByUserId(existingUserId);

            Assert.That(basket, Is.Not.Null);
            Assert.That(basket.Id, Is.GreaterThan(0));
        }

        [Test]
        public void TestGetBasketItems_BasketHasTagsAndImages_ReturnsItemsWithTagsAndImages()
        {
            testBasket = basketRepository.GetBasketByUserId(testUser.Id);
            int basketId = testBasket.Id;


            var productTag = new ProductTag(1, "Test Tag");
            var productImage = new Image("test-image-url.jpg");

            string uniqueTitle = "Product With Tags And Images " + Guid.NewGuid().ToString().Substring(0, 8);
            var productWithTagsAndImages = new BuyProduct(
                0,
                uniqueTitle,
                "Test product that has both tags and images",
                new User(2, "test seller", "test@example.com"),
                new ProductCondition(1, "New", "New condition"),
                new ProductCategory(1, "Electronics", "Electronic devices"),
                new List<ProductTag> { productTag },
                new List<Image> { productImage },
                199.99f);

            buyProductRepository.AddProduct(productWithTagsAndImages);

            var products = buyProductRepository.GetProducts();
            var addedProduct = products.FirstOrDefault(p => p.Title == uniqueTitle);


            Assert.That(addedProduct, Is.Not.Null, "Product was not found after adding to repository");
            Assert.That(addedProduct.Id, Is.GreaterThan(0), "Product does not have a valid ID");


            int productId = addedProduct.Id;

            basketRepository.AddItemToBasket(basketId, productId, 1);


            var items = basketRepository.GetBasketItems(basketId);

            Assert.That(items, Is.Not.Null);
            Assert.That(items.Count, Is.GreaterThan(0));

            var itemWithTagsAndImages = items.FirstOrDefault(i =>
                i.Product.Title == uniqueTitle);

            Assert.That(itemWithTagsAndImages, Is.Not.Null, "Could not find the product with tags and images");

            Assert.That(itemWithTagsAndImages.Product.Tags, Is.Not.Null);
            Assert.That(itemWithTagsAndImages.Product.Tags.Count, Is.GreaterThan(0));
            var firstTag = itemWithTagsAndImages.Product.Tags[0];
            Assert.That(firstTag, Is.Not.Null);

            Assert.That(itemWithTagsAndImages.Product.Images, Is.Not.Null);
            Assert.That(itemWithTagsAndImages.Product.Images.Count, Is.GreaterThan(0));
            Assert.That(itemWithTagsAndImages.Product.Images[0].Url, Is.EqualTo("test-image-url.jpg"));
        }

        [Test]
        public void TestGetBasketByUserId_PopulatesBasketWithItems()
        {
            testBasket = basketRepository.GetBasketByUserId(testUser.Id);
            int basketId = testBasket.Id;

            basketRepository.AddItemToBasket(basketId, testProduct.Id, 3);

            Basket retrievedBasket = basketRepository.GetBasketByUserId(testUser.Id);

            List<BasketItem> basketItems = retrievedBasket.GetItems();

            Assert.That(basketItems, Is.Not.Null);
            Assert.That(basketItems.Count, Is.GreaterThan(0));

            BasketItem addedItem = basketItems.FirstOrDefault(item => item.Product.Id == testProduct.Id);

            Assert.That(addedItem, Is.Not.Null, "Item was not found in the basket's internal collection");
            Assert.That(addedItem.Quantity, Is.EqualTo(3), "Item quantity doesn't match expected value");
            Assert.That(addedItem.Product.Title, Is.EqualTo(testProduct.Title), "Product title doesn't match");
        }

        [Test]
        public void TestAddItemToBasket_InvalidProduct_ThrowsException()
        {
            int basketId = testBasket.Id;
            int productId = -1;
            int quantity = 2;

            Assert.Throws<ArgumentException>(() => basketRepository.AddItemToBasket(basketId, productId, quantity));
        }

        [Test]
        public void TestAddItemToBasket_ValidProduct_AddsItem()
        {
            testBasket = basketRepository.GetBasketByUserId(testUser.Id);

            int basketId = testBasket.Id;
            int productId = testProduct.Id;
            int quantity = 2;

            basketRepository.AddItemToBasket(basketId, productId, quantity);

            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            Assert.That(items, Is.Not.Null);
            Assert.That(items.Any(i => i.Product.Id == productId && i.Quantity == quantity), Is.True);
        }

        [Test]
        public void TestAddItemToBasket_NonExistentProduct_ThrowsException()
        {
            testBasket = basketRepository.GetBasketByUserId(testUser.Id);
            int basketId = testBasket.Id;
            int nonExistentProductId = 99999;
            int quantity = 1;

            Assert.Throws<Exception>(() => basketRepository.AddItemToBasket(basketId, nonExistentProductId, quantity));
        }

        [Test]
        public void TestAddItemToBasket_InvalidQuantity_ThrowsException()
        {
            testBasket = basketRepository.GetBasketByUserId(testUser.Id);
            int basketId = testBasket.Id;
            int productId = testProduct.Id;
            int invalidQuantity = -1;
            Assert.Throws<ArgumentException>(() => basketRepository.AddItemToBasket(basketId, productId, invalidQuantity));
        }

        [Test]
        public void TestAddItemToBasket_ExistingProduct_UpdatesQuantity()
        {
            testBasket = basketRepository.GetBasketByUserId(testUser.Id);
            int basketId = testBasket.Id;
            int productId = testProduct.Id;

            basketRepository.AddItemToBasket(basketId, productId, 2);

            basketRepository.AddItemToBasket(basketId, productId, 3);

            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            var item = items.FirstOrDefault(i => i.Product.Id == productId);

            Assert.That(item, Is.Not.Null);
            Assert.That(item.Quantity, Is.EqualTo(5));
        }

        [Test]
        public void TestGetBasketItems_InvalidBasketId_ReturnsNullItemsList()
        {
            int invalidBasketId = -1;

            List<BasketItem> items = basketRepository.GetBasketItems(invalidBasketId);

            Assert.That(items, Is.Empty);
        }

        [Test]
        public void TestGetBasketItems_ValidBasketId_ReturnsItems()
        {
            int basketId = testBasket.Id;
            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            Assert.That(items, Is.Not.Null);
            Assert.That(items.Count, Is.Not.Null);
        }

        [Test]
        public void TestUpdateItemQuantityByProductId_InvalidProductId_ThrowsException()
        {
            int basketId = testBasket.Id;
            int invalidProductId = -999;
            int newQuantity = 2;
            Assert.Throws<ArgumentException>(() => basketRepository.UpdateItemQuantityByProductId(basketId, invalidProductId, newQuantity));
        }

        [Test]
        public void TestUpdateItemQuantityByProductId_ValidProductId_UpdatesQuantity()
        {
            testBasket = basketRepository.GetBasketByUserId(testUser.Id);
            int basketId = testBasket.Id;
            int productId = testProduct.Id;

            basketRepository.AddItemToBasket(basketId, productId, 1);

            int newQuantity = 3;
            basketRepository.UpdateItemQuantityByProductId(basketId, productId, newQuantity);

            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            Assert.That(items, Is.Not.Null);

            BasketItem updatedItem = items.FirstOrDefault(i => i.Product.Id == productId);
            Assert.That(updatedItem, Is.Not.Null);
            Assert.That(updatedItem.Quantity, Is.EqualTo(newQuantity));
        }

        [Test]
        public void TestUpdateItemQuantityByProductId_InvalidQuantity_ThrowsException()
        {
            int basketId = testBasket.Id;
            int productId = testProduct.Id;
            int invalidQuantity = -999;
            Assert.Throws<ArgumentException>(() => basketRepository.UpdateItemQuantityByProductId(basketId, productId, invalidQuantity));
        }

        [Test]
        public void TestUpdateItemQuantityByProductId_InvalidBasketId_ThrowsException()
        {
            int invalidBasketId = -999;
            int productId = testProduct.Id;
            int quantity = 5;

            Assert.Throws<ArgumentException>(() => basketRepository.UpdateItemQuantityByProductId(invalidBasketId, productId, quantity));
        }

        [Test]
        public void TestClearBasket_InvalidBasketId_ThrowsException()
        {
            int invalidBasketId = -999;
            Assert.Throws<ArgumentException>(() => basketRepository.ClearBasket(invalidBasketId));
        }

        [Test]
        public void TestClearBasket_ValidBasketId_ClearsBasket()
        {
            int basketId = testBasket.Id;

            basketRepository.AddItemToBasket(basketId, testProduct.Id, 1);

            basketRepository.ClearBasket(basketId);

            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            Assert.That(items, Is.Not.Null);
            Assert.That(items.Count, Is.EqualTo(0));
        }


        [Test]
        public void TestRemoveItemByProductId_InvalidProductId_DoesNotThrow()
        {
            int basketId = testBasket.Id;
            int invalidProductId = -1;

            Assert.DoesNotThrow(() => basketRepository.RemoveItemByProductId(basketId, invalidProductId));
        }

        [Test]
        public void TestRemoveItemByProductId_ValidProductId_RemovesItem()
        {
            int basketId = testBasket.Id;
            int productId = testProduct.Id;

            basketRepository.AddItemToBasket(basketId, productId, 1);

            basketRepository.RemoveItemByProductId(basketId, productId);

            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            Assert.That(items, Is.Not.Null);
            Assert.That(items.Any(i => i.Product.Id == productId), Is.False);
        }

        [Test]
        public void TestGetBasketItems_LoadsProductsWithDataFromBasket()
        {
            testBasket = basketRepository.GetBasketByUserId(testUser.Id);
            int basketId = testBasket.Id;

            basketRepository.AddItemToBasket(basketId, testProduct.Id, 3);

            var items = basketRepository.GetBasketItems(basketId);

            Assert.That(items, Is.Not.Null);
            Assert.That(items.Count, Is.EqualTo(1));

            var basketItem = items[0];

            Assert.That(basketItem.Product, Is.Not.Null);
            Assert.That(basketItem.Product.Id, Is.EqualTo(testProduct.Id));
            Assert.That(basketItem.Product.Title, Is.EqualTo(testProduct.Title));
            Assert.That(basketItem.Quantity, Is.EqualTo(3));

            int initialItemCount = 0;

            foreach (var item in items)
            {
                testBasket.AddItem(item.Product, item.Quantity);
            }

            var updatedItems = basketRepository.GetBasketItems(basketId);
            Assert.That(updatedItems, Is.Not.Null);
            Assert.That(updatedItems.Count, Is.GreaterThan(initialItemCount));

            var addedItem = updatedItems.FirstOrDefault(i => i.Product.Id == testProduct.Id);
            Assert.That(addedItem, Is.Not.Null);
            Assert.That(addedItem.Quantity, Is.EqualTo(3));
        }
    }
}
