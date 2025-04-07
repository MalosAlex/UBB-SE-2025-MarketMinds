using System.Diagnostics;
using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuctionProductView : Window
    {
        private readonly AuctionProduct product;
        private readonly User
            currentUser;
        private readonly AuctionProductsViewModel auctionProductsViewModel;

        private DispatcherTimer? countdownTimer;
        private Window? seeSellerReviewsView;
        public AuctionProductView(AuctionProduct product)
        {
            this.InitializeComponent();
            this.product = product;
            currentUser = MarketMinds.App.CurrentUser;
            auctionProductsViewModel = MarketMinds.App.AuctionProductsViewModel;
            LoadProductDetails();
            LoadImages();
            LoadBidHistory();
            StartCountdownTimer();
        }

        private void StartCountdownTimer()
        {
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
        }
        private void CountdownTimer_Tick(object? sender, object e)
        {
            string timeText = GetTimeLeft();
            TimeLeftTextBlock.Text = timeText;

            if (auctionProductsViewModel.IsAuctionEnded(product) && countdownTimer != null)
            {
                countdownTimer.Stop();
                auctionProductsViewModel.ConcludeAuction(product);
            }
        }
        private void LoadProductDetails()
        {
            // Basic Info
            TitleTextBlock.Text = product.Title;
            CategoryTextBlock.Text = product.Category.DisplayTitle;
            ConditionTextBlock.Text = product.Condition.DisplayTitle;
            StartingPriceTextBlock.Text = $"{product.StartingPrice:C}";
            CurrentPriceTextBlock.Text = $"{product.CurrentPrice:C}"; // Just an example
            TimeLeftTextBlock.Text = GetTimeLeft();

            // Seller Info
            SellerTextBlock.Text = product.Seller.Username;
            DescriptionTextBox.Text = product.Description;

            TagsItemsControl.ItemsSource = product.Tags.Select(tag =>
            {
                return new TextBlock
                {
                    Text = tag.DisplayTitle,
                    Margin = new Thickness(4),
                    Padding = new Thickness(8, 4, 8, 4),
                };
            }).ToList();
        }

        private void LoadImages()
        {
            ImageCarousel.Items.Clear();
            foreach (var image in product.Images)
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
        private void LoadBidHistory()
        {
            BidHistoryListView.ItemsSource = product.BidHistory
                .OrderByDescending(b => b.Timestamp)
                .ToList();
        }
        private string GetTimeLeft()
        {
            return auctionProductsViewModel.GetTimeLeft(product);
        }
        private void OnPlaceBidClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                auctionProductsViewModel.PlaceBid(product, currentUser, BidTextBox.Text);

                // Update UI after successful bid
                CurrentPriceTextBlock.Text = $"{product.CurrentPrice:C}";
                LoadBidHistory(); // Refresh bid list
            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private async void ShowErrorDialog(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Bid Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void OnSeeReviewsClicked(object sender, RoutedEventArgs e)
        {
            App.SeeSellerReviewsViewModel.Seller = product.Seller;
            // Create a window to host the SeeSellerReviewsView page
            var window = new Window();
            window.Content = new SeeSellerReviewsView(App.SeeSellerReviewsViewModel);
            window.Activate();
            // Store reference to window
            seeSellerReviewsView = window;
        }
    }
}