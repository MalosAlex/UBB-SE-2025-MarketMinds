using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using MarketMinds.Services.BasketService;

namespace ViewModelLayer.ViewModel
{
    public class BasketViewModel
    {
        private const int NODISCOUNT = 0;
        private const int DEFAULTQUANTITY = 1;
        private User currentUser;
        private readonly BasketService basketService;
        private Basket basket;

        public List<BasketItem> BasketItems { get; private set; }
        public float Subtotal { get; private set; }
        public float Discount { get; private set; }
        public float TotalAmount { get; private set; }
        public string PromoCode { get; set; }
        public string ErrorMessage { get; set; }

        public BasketViewModel(User currentUser, BasketService basketService)
        {
            this.currentUser = currentUser;
            this.basketService = basketService;
            BasketItems = new List<BasketItem>();
            PromoCode = string.Empty;
            ErrorMessage = string.Empty;
        }

        public void LoadBasket()
        {
            try
            {
                basket = basketService.GetBasketByUser(currentUser);
                BasketItems = basket.GetItems();

                // Use service to calculate totals instead of local method
                UpdateTotals();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load basket: {ex.Message}";
            }
        }

        public void AddToBasket(int productId)
        {
            try
            {
                basketService.AddProductToBasket(currentUser.Id, productId, DEFAULTQUANTITY);
                LoadBasket();
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to add product to basket: {ex.Message}";
            }
        }

        public void RemoveProductFromBasket(int productId)
        {
            try
            {
                basketService.RemoveProductFromBasket(currentUser.Id, productId);
                LoadBasket();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to remove product: {ex.Message}";
            }
        }

        public void UpdateProductQuantity(int productId, int quantity)
        {
            try
            {
                if (quantity > BasketService.MaxQuantityPerItem)
                {
                    ErrorMessage = $"Quantity cannot exceed {BasketService.MaxQuantityPerItem}";

                    basketService.UpdateProductQuantity(currentUser.Id, productId, BasketService.MaxQuantityPerItem);
                }
                else
                {
                    ErrorMessage = string.Empty;
                    basketService.UpdateProductQuantity(currentUser.Id, productId, quantity);
                }
                LoadBasket();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update quantity: {ex.Message}";
            }
        }

        public BasketItem GetBasketItemById(int itemId)
        {
            return BasketItems.FirstOrDefault(item => item.Id == itemId);
        }

        public bool UpdateQuantityFromText(int itemId, string quantityText, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                // Validate input using service
                if (!basketService.ValidateQuantityInput(quantityText, out int newQuantity))
                {
                    errorMessage = "Please enter a valid quantity";
                    return false;
                }

                // Find the corresponding basket item
                var basketItem = GetBasketItemById(itemId);
                if (basketItem == null)
                {
                    errorMessage = "Item not found";
                    return false;
                }

                // Update the quantity through proper service call
                UpdateProductQuantity(basketItem.Product.Id, newQuantity);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to update quantity: {ex.Message}";
                return false;
            }
        }

        public void ApplyPromoCode(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    ErrorMessage = "Please enter a promo code.";
                    return;
                }

                basketService.ApplyPromoCode(basket.Id, code);
                PromoCode = code;

                // Update totals with the new promo code
                UpdateTotals();
                ErrorMessage = $"Promo code '{code}' applied successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to apply promo code: {ex.Message}";
                Discount = NODISCOUNT;
                TotalAmount = Subtotal;
            }
        }

        public void ClearBasket()
        {
            try
            {
                basketService.ClearBasket(currentUser.Id);
                LoadBasket();
                PromoCode = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to clear basket: {ex.Message}";
            }
        }

        public bool CanCheckout()
        {
            return basketService.ValidateBasketBeforeCheckOut(basket.Id);
        }

        public void Checkout()
        {
            if (!CanCheckout())
            {
                ErrorMessage = "Cannot checkout. Please check your basket items.";
                return;
            }

            ErrorMessage = string.Empty;
        }

        public void DecreaseProductQuantity(int productId)
        {
            try
            {
                ErrorMessage = string.Empty;
                basketService.DecreaseProductQuantity(currentUser.Id, productId);
                LoadBasket();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to decrease quantity: {ex.Message}";
            }
        }

        public void IncreaseProductQuantity(int productId)
        {
            try
            {
                ErrorMessage = string.Empty;
                basketService.IncreaseProductQuantity(currentUser.Id, productId);
                LoadBasket();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to increase quantity: {ex.Message}";
            }
        }

        // New method to update totals using the service
        private void UpdateTotals()
        {
            try
            {
                BasketTotals totals = basketService.CalculateBasketTotals(basket.Id, PromoCode);
                Subtotal = totals.Subtotal;
                Discount = totals.Discount;
                TotalAmount = totals.TotalAmount;
            }
            catch (Exception ex)
            {
                // Handle calculation errors
                ErrorMessage = $"Failed to calculate totals: {ex.Message}";
                // Set default values
                Subtotal = BasketItems.Sum(item => item.GetPrice());
                Discount = NODISCOUNT;
                TotalAmount = Subtotal;
            }
        }
    }
}