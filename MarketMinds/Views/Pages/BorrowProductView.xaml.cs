using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DomainLayer.Domain;
using Microsoft.UI.Xaml.Media.Imaging;
using ViewModelLayer.ViewModel;
using MarketMinds.Views.Pages;

namespace MarketMinds
{
    public sealed partial class BorrowProductView : Window
    {
        public BorrowProduct Product { get; private set; }
        private Window? seeSellerReviewsView;
        private readonly User currentUser;
        public DateTime? SelectedEndDate { get; private set; }

        public BorrowProductView(BorrowProduct product)
        {
            Debug.WriteLine($"[BorrowProductView] Constructor - Product: {product?.Title ?? "null"}");
            Debug.WriteLine($"[BorrowProductView] Product StartDate: {product?.StartDate.ToString() ?? "null"}");
            Debug.WriteLine($"[BorrowProductView] Product TimeLimit: {product?.TimeLimit.ToString() ?? "null"}");

            this.InitializeComponent();
            Product = product;
            currentUser = MarketMinds.App.CurrentUser;

            // Initialize date controls
            Debug.WriteLine("[BorrowProductView] Initializing date controls");
            try
            {
                // Ensure valid date range
                if (Product.StartDate > Product.TimeLimit)
                {
                    Debug.WriteLine("[BorrowProductView] Warning: StartDate is after TimeLimit, swapping dates");
                    var temp = Product.StartDate;
                    Product.StartDate = Product.TimeLimit;
                    Product.TimeLimit = temp;
                }

                StartDateTextBlock.Text = Product.StartDate.ToString("d");
                TimeLimitTextBlock.Text = Product.TimeLimit.ToString("d");
                // Set the DatePicker's range
                EndDatePicker.MinDate = Product.StartDate;
                EndDatePicker.MaxDate = Product.TimeLimit;
                Debug.WriteLine($"[BorrowProductView] Date controls initialized - StartDate: {StartDateTextBlock.Text}, TimeLimit: {TimeLimitTextBlock.Text}");
                Debug.WriteLine($"[BorrowProductView] DatePicker range set - Min: {EndDatePicker.MinDate}, Max: {EndDatePicker.MaxDate}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BorrowProductView] Error initializing date controls: {ex.Message}");
                Debug.WriteLine($"[BorrowProductView] Stack trace: {ex.StackTrace}");
            }

            LoadProductDetails();
            LoadImages();
        }

        private void LoadProductDetails()
        {
            // Basic Info
            TitleTextBlock.Text = Product.Title;
            CategoryTextBlock.Text = Product.Category.DisplayTitle;
            ConditionTextBlock.Text = Product.Condition.DisplayTitle;

            // Seller Info
            SellerTextBlock.Text = Product.Seller.Username;
            DescriptionTextBox.Text = Product.Description;

            // Tags
            TagsItemsControl.ItemsSource = Product.Tags.Select(tag =>
            {
                return new TextBlock
                {
                    Text = tag.DisplayTitle,
                    Margin = new Thickness(4),
                    Padding = new Thickness(8, 4, 8, 4)
                };
            }).ToList();
        }
        private void OnJoinWaitListClicked(object sender, RoutedEventArgs e)
        {
        }

        private void OnLeaveWaitListClicked(object sender, RoutedEventArgs e)
        {
        }

        private void LoadImages()
        {
            ImageCarousel.Items.Clear();
            foreach (var image in Product.Images)
            {
                var img = new Microsoft.UI.Xaml.Controls.Image
                {
                    Source = new BitmapImage(new Uri(image.Url)),
                    Stretch = Stretch.Uniform, // ✅ shows full image without cropping
                    Height = 250,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                ImageCarousel.Items.Add(img);
            }
        }

        private void OnSeeReviewsClicked(object sender, RoutedEventArgs e)
        {
            App.SeeSellerReviewsViewModel.Seller = Product.Seller;
            // Create a window to host the SeeSellerReviewsView page
            var window = new Window();
            window.Content = new SeeSellerReviewsView(App.SeeSellerReviewsViewModel);
            window.Activate();
            // Store reference to window
            seeSellerReviewsView = window;
        }

        private void EndDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            Debug.WriteLine("[EndDatePicker] DateChanged event started");
            Debug.WriteLine($"[EndDatePicker] Sender null? {sender == null}");
            Debug.WriteLine($"[EndDatePicker] Args null? {args == null}");
            Debug.WriteLine($"[EndDatePicker] Product null? {Product == null}");
            if (sender == null)
            {
                Debug.WriteLine("[EndDatePicker] Error: sender is null");
                return;
            }

            Debug.WriteLine($"[EndDatePicker] Current EndDatePicker.Date: {sender.Date?.ToString() ?? "null"}");
            Debug.WriteLine($"[EndDatePicker] Product.StartDate: {Product?.StartDate.ToString() ?? "null"}");
            Debug.WriteLine($"[EndDatePicker] Product.TimeLimit: {Product?.TimeLimit.ToString() ?? "null"}");

            try
            {
                if (EndDatePicker.Date != null)
                {
                    SelectedEndDate = EndDatePicker.Date.Value.DateTime;
                    Debug.WriteLine($"[EndDatePicker] Selected date set to: {SelectedEndDate}");
                    CalculatePriceButton.IsEnabled = true;
                }
                else
                {
                    SelectedEndDate = null;
                    Debug.WriteLine("[EndDatePicker] Selected date cleared");
                    CalculatePriceButton.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EndDatePicker] Error in date processing: {ex.Message}");
                Debug.WriteLine($"[EndDatePicker] Stack trace: {ex.StackTrace}");
            }
        }

        private void OnCalculatePriceClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedEndDate.HasValue)
            {
                // Calculate price based on days difference and daily rate
                TimeSpan duration = SelectedEndDate.Value - Product.StartDate;
                int days = duration.Days + 1; // Include both start and end dates
                float totalPrice = days * Product.DailyRate;

                PriceTextBlock.Text = totalPrice.ToString("C"); // Format as currency
            }
        }
    }
}