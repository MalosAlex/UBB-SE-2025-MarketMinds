using System;
using System.Collections.Generic;
using NUnit.Framework;
using DomainLayer.Domain;
using MarketMinds.Services.BasketService;
using MarketMinds.Repositories.BasketRepository;
using MarketMinds.Test.Services.BasketServiceTest;

namespace MarketMinds.Tests.Services.BasketServiceTest
{
    [TestFixture]
    public class BasketServiceTest
    {
        private BasketService basketService;
        private BasketRepositoryMock basketRepositoryMock;
        private User testUser;
        private int testUserId = 1;
        private int testProductId = 10;

        [SetUp]
        public void Setup()
        {
            basketRepositoryMock = new BasketRepositoryMock();
            basketService = new BasketService(basketRepositoryMock);
            testUser = new User(testUserId, "Test User", "test@example.com");
        }

        [Test]
        public void TestAddProductToBasket_ValidParameters_AddsProduct()
        {
            int quantity = 3;

            basketService.AddProductToBasket(testUserId, testProductId, quantity);

            Assert.That(basketRepositoryMock.GetAddItemCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestAddProductToBasket_QuantityExceedsMax_LimitsQuantity()
        {
            int quantity = BasketService.MaxQuantityPerItem + 5;

            basketService.AddProductToBasket(testUserId, testProductId, quantity);

            Assert.That(basketRepositoryMock.GetAddItemCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestAddProductToBasket_InvalidUserId_ThrowsException()
        {
            int invalidUserId = 0;

            Exception ex = null;
            try
            {
                basketService.AddProductToBasket(invalidUserId, testProductId, 1);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid user ID"));
            Assert.That(basketRepositoryMock.GetAddItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestAddProductToBasket_InvalidProductId_ThrowsException()
        {
            int invalidProductId = 0;

            Exception ex = null;
            try
            {
                basketService.AddProductToBasket(testUserId, invalidProductId, 1);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid product ID"));
            Assert.That(basketRepositoryMock.GetAddItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestGetBasketByUser_ValidUser_ReturnsBasket()
        {
            var basket = basketService.GetBasketByUser(testUser);

            Assert.That(basket, Is.Not.Null);
        }

        [Test]
        public void TestGetBasketByUser_NullUser_ThrowsException()
        {
            Exception ex = null;
            try
            {
                basketService.GetBasketByUser(null);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Valid user must be provided"));
        }

        [Test]
        public void TestRemoveProductFromBasket_ValidParameters_RemovesProduct()
        {
            basketService.RemoveProductFromBasket(testUserId, testProductId);

            Assert.That(basketRepositoryMock.GetRemoveItemCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestRemoveProductFromBasket_InvalidUserId_ThrowsException()
        {
            int invalidUserId = 0;

            Exception ex = null;
            try
            {
                basketService.RemoveProductFromBasket(invalidUserId, testProductId);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid user ID"));
            Assert.That(basketRepositoryMock.GetRemoveItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestUpdateProductQuantity_ValidParameters_UpdatesQuantity()
        {
            basketService.AddProductToBasket(testUserId, testProductId, 1);

            int addCount = basketRepositoryMock.GetAddItemCount();
            Assert.That(addCount, Is.EqualTo(1));

            int quantity = 5;
            basketService.UpdateProductQuantity(testUserId, testProductId, quantity);

            Assert.That(basketRepositoryMock.GetUpdateItemCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestUpdateProductQuantity_QuantityExceedsMax_LimitsQuantity()
        {
            basketService.AddProductToBasket(testUserId, testProductId, 1);

            int addCount = basketRepositoryMock.GetAddItemCount();
            Assert.That(addCount, Is.EqualTo(1));

            int quantity = BasketService.MaxQuantityPerItem + 5;
            basketService.UpdateProductQuantity(testUserId, testProductId, quantity);

            Assert.That(basketRepositoryMock.GetUpdateItemCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestUpdateProductQuantity_QuantityZero_RemovesProduct()
        {
            int quantity = 0;

            basketService.UpdateProductQuantity(testUserId, testProductId, quantity);

            Assert.That(basketRepositoryMock.GetRemoveItemCount(), Is.EqualTo(1));
            Assert.That(basketRepositoryMock.GetUpdateItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestUpdateProductQuantity_InvalidUserId_ThrowsException()
        {
            int invalidUserId = 0;

            Exception ex = null;
            try
            {
                basketService.UpdateProductQuantity(invalidUserId, testProductId, 1);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid user ID"));
            Assert.That(basketRepositoryMock.GetUpdateItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestClearBasket_ValidUserId_ClearsBasket()
        {
            basketService.ClearBasket(testUserId);

            Assert.That(basketRepositoryMock.GetClearBasketCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestClearBasket_InvalidUserId_ThrowsException()
        {
            int invalidUserId = 0;

            Exception ex = null;
            try
            {
                basketService.ClearBasket(invalidUserId);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid user ID"));
            Assert.That(basketRepositoryMock.GetClearBasketCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestValidateBasketBeforeCheckOut_EmptyBasket_ReturnsFalse()
        {
            int basketId = 1;

            bool result = basketService.ValidateBasketBeforeCheckOut(basketId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TestApplyPromoCode_ValidCode_AppliesDiscount()
        {
            int basketId = 1;
            string validCode = "DISCOUNT10";

            basketService.ApplyPromoCode(basketId, validCode);

            float discount = basketService.GetPromoCodeDiscount(validCode, 100);
            Assert.That(discount, Is.EqualTo(10));
        }

        [Test]
        public void TestApplyPromoCode_InvalidCode_ThrowsException()
        {
            int basketId = 1;
            string invalidCode = "INVALID";

            Exception ex = null;
            try
            {
                basketService.ApplyPromoCode(basketId, invalidCode);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid promo code"));
        }

        [Test]
        public void TestValidateBasketBeforeCheckOut_InvalidBasketId_ThrowsException()
        {
            int invalidBasketId = 0;

            Exception ex = Assert.Throws<ArgumentException>(() => basketService.ValidateBasketBeforeCheckOut(invalidBasketId));

            Assert.That(ex.Message, Is.EqualTo("Invalid basket ID"));
        }

        [Test]
        public void TestValidateBasketBeforeCheckOut_ItemWithZeroQuantity_ReturnsFalse()
        {
            int basketId = 1;
            basketRepositoryMock.SetupInvalidItemQuantity(basketId);

            bool result = basketService.ValidateBasketBeforeCheckOut(basketId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TestValidateBasketBeforeCheckOut_ItemWithInvalidPrice_ReturnsFalse()
        {
            int basketId = 1;
            basketRepositoryMock.SetupInvalidItemPrice(basketId);

            bool result = basketService.ValidateBasketBeforeCheckOut(basketId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TestValidateBasketBeforeCheckOut_ValidBasket_ReturnsTrue()
        {
            int basketId = 1;
            basketRepositoryMock.SetupValidBasket(basketId);

            bool result = basketService.ValidateBasketBeforeCheckOut(basketId);

            Assert.That(result, Is.True);
        }

        [Test]
        public void TestApplyPromoCode_InvalidBasketId_ThrowsException()
        {
            int invalidBasketId = 0;
            string code = "DISCOUNT10";

            Exception ex = Assert.Throws<ArgumentException>(() => basketService.ApplyPromoCode(invalidBasketId, code));

            Assert.That(ex.Message, Is.EqualTo("Invalid basket ID"));
        }

        [Test]
        public void TestApplyPromoCode_EmptyCode_ThrowsException()
        {
            int basketId = 1;
            string emptyCode = "";

            Exception ex = Assert.Throws<ArgumentException>(() => basketService.ApplyPromoCode(basketId, emptyCode));

            Assert.That(ex.Message, Is.EqualTo("Promo code cannot be empty"));
        }

        [Test]
        public void TestApplyPromoCode_NullCode_ThrowsException()
        {
            int basketId = 1;
            string nullCode = null;

            Exception ex = Assert.Throws<ArgumentException>(() => basketService.ApplyPromoCode(basketId, nullCode));

            Assert.That(ex.Message, Is.EqualTo("Promo code cannot be empty"));
        }

        [Test]
        public void TestApplyPromoCode_WhitespaceCode_ThrowsException()
        {
            int basketId = 1;
            string whitespaceCode = "   ";

            Exception ex = Assert.Throws<ArgumentException>(() => basketService.ApplyPromoCode(basketId, whitespaceCode));

            Assert.That(ex.Message, Is.EqualTo("Promo code cannot be empty"));
        }

        [Test]
        public void TestUpdateProductQuantity_InvalidProductId_ThrowsException()
        {
            int invalidProductId = 0;

            Exception ex = Assert.Throws<ArgumentException>(() => basketService.UpdateProductQuantity(testUserId, invalidProductId, 1));

            Assert.That(ex.Message, Is.EqualTo("Invalid product ID"));
            Assert.That(basketRepositoryMock.GetUpdateItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestUpdateProductQuantity_NegativeQuantity_ThrowsException()
        {
            int negativeQuantity = -1;

            Exception ex = Assert.Throws<ArgumentException>(() => basketService.UpdateProductQuantity(testUserId, testProductId, negativeQuantity));

            Assert.That(ex.Message, Is.EqualTo("Quantity cannot be negative"));
            Assert.That(basketRepositoryMock.GetUpdateItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestUpdateProductQuantity_RepositoryThrowsException_ThrowsInvalidOperationException()
        {
            basketRepositoryMock.SetupUpdateQuantityException();

            Exception ex = Assert.Throws<InvalidOperationException>(() => basketService.UpdateProductQuantity(testUserId, testProductId, 1));

            Assert.That(ex.Message, Does.Contain("Could not update quantity:"));
        }

        [Test]
        public void TestRemoveProductFromBasket_InvalidProductId_ThrowsException()
        {
            int invalidProductId = 0;

            Exception ex = Assert.Throws<ArgumentException>(() => basketService.RemoveProductFromBasket(testUserId, invalidProductId));

            Assert.That(ex.Message, Is.EqualTo("Invalid product ID"));
            Assert.That(basketRepositoryMock.GetRemoveItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestRemoveProductFromBasket_RepositoryThrowsException_ThrowsInvalidOperationException()
        {
            basketRepositoryMock.SetupRemoveItemException();

            Exception ex = Assert.Throws<InvalidOperationException>(() => basketService.RemoveProductFromBasket(testUserId, testProductId));

            Assert.That(ex.Message, Does.Contain("Could not remove product:"));
        }

        [Test]
        public void TestGetBasketByUser_RepositoryThrowsException_ThrowsApplicationException()
        {
            basketRepositoryMock.SetupGetBasketException();

            Exception ex = Assert.Throws<ApplicationException>(() => basketService.GetBasketByUser(testUser));

            Assert.That(ex.Message, Is.EqualTo("Failed to retrieve user's basket"));
        }

        [Test]
        public void TestGetPromoCodeDiscount_EmptyCode_ReturnsZero()
        {
            string emptyCode = "";
            float subtotal = 100;

            float discount = basketService.GetPromoCodeDiscount(emptyCode, subtotal);

            Assert.That(discount, Is.EqualTo(0));
        }

        [Test]
        public void TestGetPromoCodeDiscount_NullCode_ReturnsZero()
        {
            string nullCode = null;
            float subtotal = 100;

            float discount = basketService.GetPromoCodeDiscount(nullCode, subtotal);

            Assert.That(discount, Is.EqualTo(0));
        }

        [Test]
        public void TestGetPromoCodeDiscount_WhitespaceCode_ReturnsZero()
        {
            string whitespaceCode = "   ";
            float subtotal = 100;

            float discount = basketService.GetPromoCodeDiscount(whitespaceCode, subtotal);

            Assert.That(discount, Is.EqualTo(0));
        }

        [Test]
        public void TestCalculateBasketTotals_ValidBasket_ReturnsCorrectTotals()
        {
            // Setup a basket with products
            int basketId = 1;

            // Create a mock basket with items (product with price 100 and product with price 50)
            basketRepositoryMock.SetupValidBasket(basketId);

            // Calculate totals without promo code
            BasketTotals totals = basketService.CalculateBasketTotals(basketId, null);

            // Verify results - expected subtotal is 150 (100 + 50)
            Assert.That(totals, Is.Not.Null);
            Assert.That(totals.Subtotal, Is.EqualTo(150));
            Assert.That(totals.Discount, Is.EqualTo(0));
            Assert.That(totals.TotalAmount, Is.EqualTo(150));
        }

        [Test]
        public void TestCalculateBasketTotals_WithValidPromoCode_AppliesDiscount()
        {
            // Setup a basket with products
            int basketId = 1;
            string validPromoCode = "DISCOUNT10";

            // Create a mock basket with items (total value 150)
            basketRepositoryMock.SetupValidBasket(basketId);

            // Calculate totals with promo code
            BasketTotals totals = basketService.CalculateBasketTotals(basketId, validPromoCode);

            // Verify results - 10% discount on 150 = 15
            Assert.That(totals, Is.Not.Null);
            Assert.That(totals.Subtotal, Is.EqualTo(150));
            Assert.That(totals.Discount, Is.EqualTo(15));
            Assert.That(totals.TotalAmount, Is.EqualTo(135));
        }

        [Test]
        public void TestCalculateBasketTotals_WithInvalidPromoCode_NoDiscount()
        {
            // Setup a basket with products
            int basketId = 1;
            string invalidPromoCode = "INVALID";

            // Create a mock basket with items (total value 150)
            basketRepositoryMock.SetupValidBasket(basketId);

            // Calculate totals with invalid promo code
            BasketTotals totals = basketService.CalculateBasketTotals(basketId, invalidPromoCode);

            // Verify results - no discount applied
            Assert.That(totals, Is.Not.Null);
            Assert.That(totals.Subtotal, Is.EqualTo(150));
            Assert.That(totals.Discount, Is.EqualTo(0));
            Assert.That(totals.TotalAmount, Is.EqualTo(150));
        }

        [Test]
        public void TestCalculateBasketTotals_EmptyBasket_ReturnsZeroTotals()
        {
            // Setup an empty basket
            int basketId = 2;

            // Calculate totals for empty basket
            BasketTotals totals = basketService.CalculateBasketTotals(basketId, null);

            // Verify results
            Assert.That(totals, Is.Not.Null);
            Assert.That(totals.Subtotal, Is.EqualTo(0));
            Assert.That(totals.Discount, Is.EqualTo(0));
            Assert.That(totals.TotalAmount, Is.EqualTo(0));
        }

        [Test]
        public void TestCalculateBasketTotals_InvalidBasketId_ThrowsException()
        {
            // Setup an invalid basket ID
            int invalidBasketId = -1;

            // Attempt to calculate totals with invalid basket ID
            Assert.Throws<ArgumentException>(() => basketService.CalculateBasketTotals(invalidBasketId, null));
        }
    }
}
