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
using ViewModelLayer.ViewModel;
using DomainLayer.Domain;
using Microsoft.UI.Xaml.Media.Imaging;
using MarketMinds.Views.Pages;
using MarketMinds.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace MarketMinds.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompareProductsView : Page
    {
        public CompareProductsViewModel ViewModel;
        private Window parentWindow;
        private readonly ProductViewNavigationService navigationService;

        public CompareProductsView(CompareProductsViewModel viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
            navigationService = new ProductViewNavigationService();
            LoadImages();
        }

        public void OnSeeReviewsLeftProductClicked(object sender, RoutedEventArgs e)
        {
            App.SeeSellerReviewsViewModel.Seller = ViewModel.LeftProduct.Seller;
            // Create a window to host the SeeSellerReviewsView page
            var window = new Window();
            window.Content = new SeeSellerReviewsView(App.SeeSellerReviewsViewModel);
            window.Activate();
        }

        public void OnSeeReviewsRightProductClicked(object sender, RoutedEventArgs e)
        {
            App.SeeSellerReviewsViewModel.Seller = ViewModel.RightProduct.Seller;
            // Create a window to host the SeeSellerReviewsView page
            var window = new Window();
            window.Content = new SeeSellerReviewsView(App.SeeSellerReviewsViewModel);
            window.Activate();
        }

        private void LoadImages()
        {
            LeftImageCarousel.Items.Clear();
            RightImageCarousel.Items.Clear();

            foreach (var image in ViewModel.LeftProduct.Images)
            {
                Debug.WriteLine("Loading image: " + image.Url);

                var img = new Microsoft.UI.Xaml.Controls.Image
                {
                    Source = new BitmapImage(new Uri(image.Url)),
                    Stretch = Stretch.Uniform, // ✅ shows full image without cropping
                    Height = 200,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                LeftImageCarousel.Items.Add(img);
            }

            foreach (var image in ViewModel.RightProduct.Images)
            {
                Debug.WriteLine("Loading image: " + image.Url);

                var img = new Microsoft.UI.Xaml.Controls.Image
                {
                    Source = new BitmapImage(new Uri(image.Url)),
                    Stretch = Stretch.Uniform, // ✅ shows full image without cropping
                    Height = 250,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                RightImageCarousel.Items.Add(img);
            }
        }

        private void OnSelectLeftProductClicked(object sender, RoutedEventArgs e)
        {
            if (parentWindow == null)
            {
                return;
            }
            // Checks if leftProduct is auctionProduct and if it is assigns it to product
            if (ViewModel.LeftProduct is AuctionProduct auctionProduct)
            {
                var detailView = new AuctionProductView(auctionProduct);
                detailView.Activate();
                parentWindow.Close();
            }
            else if (ViewModel.LeftProduct is BorrowProduct borrowProduct)
            {
                var detailView = new BorrowProductView(borrowProduct);
                detailView.Activate();
                parentWindow.Close();
            }
            else
            {
                BuyProduct buyProduct = (BuyProduct)ViewModel.LeftProduct;
                var detailView = new BuyProductView(buyProduct);
                detailView.Activate();
                parentWindow.Close();
            }
        }

        private void OnSelectRightProductClicked(object sender, RoutedEventArgs e)
        {
            if (parentWindow == null)
            {
                return;
            }
            // Checks if RightProduct is auctionProduct and if it is assigns it to product
            if (ViewModel.RightProduct is AuctionProduct auctionProduct)
            {
                var detailView = new AuctionProductView(auctionProduct);
                detailView.Activate();
                parentWindow.Close();
            }
            else if (ViewModel.RightProduct is BorrowProduct borrowProduct)
            {
                var detailView = new BorrowProductView(borrowProduct);
                detailView.Activate();
                parentWindow.Close();
            }
            else
            {
                BuyProduct buyProduct = (BuyProduct)ViewModel.RightProduct;
                var detailView = new BuyProductView(buyProduct);
                detailView.Activate();
                parentWindow.Close();
            }
        }
        // Method to set the parent window
        public void SetParentWindow(Window window)
        {
            parentWindow = window;
        }

        private void ViewProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                var detailView = navigationService.CreateProductDetailView(product);
                detailView.Activate();
            }
        }

        private void ViewSeller_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                var sellerReviewsView = navigationService.CreateSellerReviewsView(product.Seller);
                sellerReviewsView.Activate();
            }
        }
    }
}
