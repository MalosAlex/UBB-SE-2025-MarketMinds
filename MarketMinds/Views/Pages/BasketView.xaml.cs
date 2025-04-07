using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ViewModelLayer.ViewModel;
using DomainLayer.Domain;

namespace MarketMinds.Views.Pages
{
    public sealed partial class BasketView : Page
    {
        private BasketViewModel var_basketViewModel;
        private ObservableCollection<BasketItem> var_basketItems;
        private User var_currentUser;

        public BasketView()
        {
            this.InitializeComponent();

            // Get the current user from the app
            var_currentUser = MarketMinds.App.CurrentUser;

            // Get the BasketViewModel from the app
            var_basketViewModel = MarketMinds.App.BasketViewModel;

            // Initialize basket items as ObservableCollection for auto-UI updates
            var_basketItems = new ObservableCollection<BasketItem>();

            // Set the ListView's data source
            BasketItemsListView.ItemsSource = var_basketItems;

            // Load basket data
            LoadBasketData();
        }

        private async void ShowErrorMessage(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void LoadBasketData()
        {
            try
            {
                // Refresh basket data from the view model
                var_basketViewModel.LoadBasket();

                // Clear the current basket items
                var_basketItems.Clear();

                // Add all items from the view model
                foreach (var item in var_basketViewModel.BasketItems)
                {
                    var_basketItems.Add(item);
                }

                // Update UI elements
                UpdateUIElements();
            }
            catch (Exception ex)
            {
                // Show error message to user
                ErrorMessageTextBlock.Text = $"Error loading basket: {ex.Message}";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void UpdateUIElements()
        {
            // Update item count
            int itemCount = var_basketItems.Sum(item => item.Quantity);
            ItemCountTextBlock.Text = $"{itemCount} item{(itemCount != 1 ? "s" : string.Empty)} in your basket";

            // Update price displays
            SubtotalTextBlock.Text = $"${var_basketViewModel.Subtotal:F2}";
            DiscountTextBlock.Text = $"-${var_basketViewModel.Discount:F2}";
            TotalTextBlock.Text = $"${var_basketViewModel.TotalAmount:F2}";

            // Show empty basket message if there are no items
            if (var_basketItems.Count == 0)
            {
                EmptyBasketMessage.Visibility = Visibility.Visible;
                BasketItemsListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyBasketMessage.Visibility = Visibility.Collapsed;
                BasketItemsListView.Visibility = Visibility.Visible;
            }

            // Enable/disable checkout button based on whether checkout is possible
            CheckoutButton.IsEnabled = var_basketViewModel.CanCheckout();

            // Show any error messages
            if (!string.IsNullOrEmpty(var_basketViewModel.ErrorMessage))
            {
                ErrorMessageTextBlock.Text = var_basketViewModel.ErrorMessage;
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void HandleRemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button != null && button.CommandParameter is int itemId)
                {
                    // Find the corresponding basket item
                    var item = var_basketItems.FirstOrDefault(i => i.Id == itemId);
                    if (item != null)
                    {
                        // Remove using product ID instead of item ID
                        var_basketViewModel.RemoveProductFromBasket(item.Product.Id);
                    }
                    LoadBasketData();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to remove item: {ex.Message}");
            }
        }

        private void HandleIncreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button != null && button.CommandParameter is int itemId)
                {
                    // Find the corresponding basket item
                    var basketItem = var_basketItems.FirstOrDefault(item => item.Id == itemId);
                    if (basketItem != null)
                    {
                        // Call the view model to handle the logic
                        var_basketViewModel.IncreaseProductQuantity(basketItem.Product.Id);
                        // Reload the basket data - this is already done in the view model's method,
                        // but kept here for consistency and to ensure UI updates
                        LoadBasketData();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to increase quantity: {ex.Message}");
            }
        }

        private void HandleDecreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button != null && button.CommandParameter is int itemId)
                {
                    // Find the corresponding basket item
                    var basketItem = var_basketItems.FirstOrDefault(item => item.Id == itemId);
                    if (basketItem != null)
                    {
                        // Call the view model to handle the logic
                        var_basketViewModel.DecreaseProductQuantity(basketItem.Product.Id);
                        // Reload the basket data - this is already done in the view model's method,
                        // but kept here for consistency and to ensure UI updates
                        LoadBasketData();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to decrease quantity: {ex.Message}");
            }
        }

        private void UpdateQuantityFromTextBox(TextBox textBox)
        {
            if (textBox != null && textBox.Tag is int itemId)
            {
                string errorMessage;
                bool success = var_basketViewModel.UpdateQuantityFromText(itemId, textBox.Text, out errorMessage);

                if (!success)
                {
                    // Show error message if needed
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        ShowErrorMessage(errorMessage);
                    }

                    // Reset the text box to current value if validation failed
                    var basketItem = var_basketViewModel.GetBasketItemById(itemId);
                    if (basketItem != null)
                    {
                        textBox.Text = basketItem.Quantity.ToString();
                    }
                }
                else
                {
                    // Reload the basket data
                    LoadBasketData();
                }
            }
        }

        private void QuantityTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateQuantityFromTextBox(sender as TextBox);
        }

        private void QuantityTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                UpdateQuantityFromTextBox(sender as TextBox);
                e.Handled = true;
            }
        }

        private void HandleApplyPromoCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string promoCode = PromoCodeTextBox.Text?.Trim();
            if (!string.IsNullOrEmpty(promoCode))
            {
                try
                {
                    var_basketViewModel.ApplyPromoCode(promoCode);

                    // The promo code was applied successfully if it gets here
                    if (var_basketViewModel.Discount > 0)
                    {
                        ErrorMessageTextBlock.Text = $"Promo code '{promoCode}' applied successfully!";
                        ErrorMessageTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                        ErrorMessageTextBlock.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ErrorMessageTextBlock.Text = "Promo code applied, but no discount was awarded.";
                        ErrorMessageTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
                        ErrorMessageTextBlock.Visibility = Visibility.Visible;
                    }
                    LoadBasketData();
                }
                catch (Exception ex)
                {
                    ErrorMessageTextBlock.Text = $"Error applying promo code: {ex.Message}";
                    ErrorMessageTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
            }
            else
            {
                ErrorMessageTextBlock.Text = "Please enter a promo code.";
                ErrorMessageTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void HandleClearBasketButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Clear Basket",
                Content = "Are you sure you want to remove all items from your basket?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                XamlRoot = Content.XamlRoot
            };

            _ = dialog.ShowAsync().AsTask().ContinueWith(t =>
            {
                if (t.Result == ContentDialogResult.Primary)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        var_basketViewModel.ClearBasket();
                        LoadBasketData();
                    });
                }
            });
        }

        private void HandleCheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (var_basketViewModel.CanCheckout())
            {
                var_basketViewModel.Checkout();
                // Navigate to checkout page
                ShowCheckoutMessage();
            }
            else
            {
                ErrorMessageTextBlock.Text = "Unable to proceed to checkout. Please check your basket items.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
        }

        private async void ShowCheckoutMessage()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Checkout",
                Content = "This would navigate to the checkout process. Checkout functionality is not yet implemented.",
                CloseButtonText = "OK",
                XamlRoot = Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void HandleContinueShoppingButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back or to product listing page
            // Since this is a Page, we use Frame navigation instead of Close()
            if (this.Frame != null)
            {
                this.Frame.GoBack();
            }
        }
    }
}