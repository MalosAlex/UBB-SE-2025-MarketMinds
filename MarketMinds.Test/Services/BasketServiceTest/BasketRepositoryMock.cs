using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using MarketMinds.Repositories.BasketRepository;

namespace MarketMinds.Test.Services.BasketServiceTest
{
    public class BasketRepositoryMock : IBasketRepository
    {
        private Dictionary<int, Basket> baskets;
        private Dictionary<int, List<BasketItem>> basketItems;
        private int nextBasketId = 1;
        private int nextItemId = 1;

        private int addItemCount;
        private int removeItemCount;
        private int updateItemCount;
        private int clearBasketCount;
        private bool throwUpdateException;
        private bool throwRemoveException;
        private bool throwGetBasketException;

        public BasketRepositoryMock()
        {
            baskets = new Dictionary<int, Basket>();
            basketItems = new Dictionary<int, List<BasketItem>>();
            addItemCount = 0;
            removeItemCount = 0;
            updateItemCount = 0;
            clearBasketCount = 0;
            throwUpdateException = false;
            throwRemoveException = false;
            throwGetBasketException = false;
        }

        public Basket GetBasketByUserId(int userId)
        {
            if (throwGetBasketException)
            {
                throw new Exception("Test exception for GetBasketByUserId");
            }

            if (userId <= 0)
            {
                throw new ArgumentException("User ID cannot be negative or zero");
            }

            // Return existing basket or create new one
            if (baskets.ContainsKey(userId))
            {
                return baskets[userId];
            }

            var basket = new Basket(nextBasketId++);
            baskets[userId] = basket;
            basketItems[basket.Id] = new List<BasketItem>();
            return basket;
        }

        public List<BasketItem> GetBasketItems(int basketId)
        {
            if (basketItems.ContainsKey(basketId))
            {
                return new List<BasketItem>(basketItems[basketId]);
            }
            return new List<BasketItem>();
        }

        public void AddItemToBasket(int basketId, int productId, int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative");
            }

            if (productId <= 0)
            {
                throw new ArgumentException("Product ID cannot be negative or zero");
            }

            if (!basketItems.ContainsKey(basketId))
            {
                basketItems[basketId] = new List<BasketItem>();
            }

            var existingItem = basketItems[basketId].FirstOrDefault(i => i.Product?.Id == productId);
            if (existingItem != null)
            {
                // Update existing item
                existingItem.Quantity = quantity;
            }
            else
            {
                // Create test product
                var product = new BuyProduct(
                    productId,
                    "Test Product",
                    "Test Description",
                    new User(1, "Test Seller", "seller@test.com"),
                    new ProductCondition(1, "New", "Brand new item"),
                    new ProductCategory(1, "Electronics", "Electronic devices"),
                    new List<ProductTag>(),
                    new List<Image>(),
                    100);

                var item = new BasketItem(nextItemId++, product, quantity);
                basketItems[basketId].Add(item);
            }

            addItemCount++;
        }

        public void RemoveItemByProductId(int basketId, int productId)
        {
            if (throwRemoveException)
            {
                throw new Exception("Test exception for RemoveItemByProductId");
            }

            if (basketItems.ContainsKey(basketId))
            {
                basketItems[basketId].RemoveAll(i => i.Product?.Id == productId);
            }

            removeItemCount++;
        }

        public void UpdateItemQuantityByProductId(int basketId, int productId, int quantity)
        {
            if (throwUpdateException)
            {
                throw new Exception("Test exception for UpdateItemQuantityByProductId");
            }

            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative");
            }

            if (basketItems.ContainsKey(basketId))
            {
                var item = basketItems[basketId].FirstOrDefault(i => i.Product?.Id == productId);
                if (item != null)
                {
                    item.Quantity = quantity;
                    updateItemCount++;
                }
            }
            else
            {
                // If basket doesn't exist, still increment the counter
                // This is needed for testing purposes
                updateItemCount++;
            }
        }

        public void ClearBasket(int basketId)
        {
            if (basketItems.ContainsKey(basketId))
            {
                basketItems[basketId].Clear();
            }
            clearBasketCount++;
        }

        public int GetAddItemCount()
        {
            return addItemCount;
        }

        public int GetRemoveItemCount()
        {
            return removeItemCount;
        }

        public int GetUpdateItemCount()
        {
            return updateItemCount;
        }

        public int GetClearBasketCount()
        {
            return clearBasketCount;
        }

        public void SetupInvalidItemQuantity(int basketId)
        {
            if (!basketItems.ContainsKey(basketId))
            {
                basketItems[basketId] = new List<BasketItem>();
            }

            var product = new BuyProduct(
                1,
                "Invalid Quantity Product",
                "Product with invalid quantity",
                new User(1, "Test Seller", "seller@test.com"),
                new ProductCondition(1, "New", "Brand new item"),
                new ProductCategory(1, "Electronics", "Electronic devices"),
                new List<ProductTag>(),
                new List<Image>(),
                100);

            var invalidItem = new BasketItem(nextItemId++, product, 0);
            basketItems[basketId].Add(invalidItem);
        }

        public void SetupInvalidItemPrice(int basketId)
        {
            if (!basketItems.ContainsKey(basketId))
            {
                basketItems[basketId] = new List<BasketItem>();
            }

            // Create a test product that will cause HasValidPrice to be false
            var invalidPriceProduct = new TestProductWithNoPrice();
            invalidPriceProduct.Id = 2;
            invalidPriceProduct.Title = "Invalid Price Product";
            invalidPriceProduct.Description = "Product with invalid price";
            invalidPriceProduct.Seller = new User(1, "Test Seller", "seller@test.com");
            invalidPriceProduct.Condition = new ProductCondition(1, "New", "Brand new item");
            invalidPriceProduct.Category = new ProductCategory(1, "Electronics", "Electronic devices");
            invalidPriceProduct.Tags = new List<ProductTag>();
            invalidPriceProduct.Images = new List<Image>();

            var invalidItem = new BasketItem(nextItemId++, invalidPriceProduct, 1);
            basketItems[basketId].Add(invalidItem);
        }

        public void SetupValidBasket(int basketId)
        {
            if (!basketItems.ContainsKey(basketId))
            {
                basketItems[basketId] = new List<BasketItem>();
            }
            else
            {
                basketItems[basketId].Clear();
            }

            // Create a test product with price 100
            var product = new BuyProduct(
                3,
                "Valid Product",
                "Product with valid attributes",
                new User(1, "Test Seller", "seller@test.com"),
                new ProductCondition(1, "New", "Brand new item"),
                new ProductCategory(1, "Electronics", "Electronic devices"),
                new List<ProductTag>(),
                new List<Image>(),
                100);

            // Create a second product with price 50
            var product2 = new BuyProduct(
                4,
                "Second Product",
                "Another product with valid attributes",
                new User(1, "Test Seller", "seller@test.com"),
                new ProductCondition(1, "New", "Brand new item"),
                new ProductCategory(1, "Electronics", "Electronic devices"),
                new List<ProductTag>(),
                new List<Image>(),
                50);

            // Add items to the basket
            var validItem = new BasketItem(nextItemId++, product, 1);
            var validItem2 = new BasketItem(nextItemId++, product2, 1);

            basketItems[basketId].Add(validItem);
            basketItems[basketId].Add(validItem2);
        }

        public void SetupUpdateQuantityException()
        {
            throwUpdateException = true;
        }

        public void SetupRemoveItemException()
        {
            throwRemoveException = true;
        }

        public void SetupGetBasketException()
        {
            throwGetBasketException = true;
        }

        // Test helper class for products with no price
        private class TestProductWithNoPrice : Product
        {
            // This product type doesn't have a price property, so when used in BasketItem
            // it will have HasValidPrice = false because it's not a BuyProduct
        }
    }
}
