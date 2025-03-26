﻿using System;
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
using ViewModelLayer.ViewModel;
using DomainLayer.Domain;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketMinds
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompareProductsView : Window
    {
        public CompareProductsViewModel ViewModel;
        public CompareProductsView(CompareProductsViewModel viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
            this.Closed += OnWindowClosed;
            LoadImages();
            
        }

        private void OnWindowClosed(object sender, WindowEventArgs e)
        {
            // Clear product data when window closes
            ViewModel.LeftProduct = null;
            ViewModel.RightProduct = null;
        }

        public void OnSeeReviewsLeftProductClicked(object sender, RoutedEventArgs e)
        {
            App.seeSellerReviewsViewModel.seller = ViewModel.LeftProduct.Seller;
            var seeSellerReviewsView = new SeeSellerReviewsView(App.seeSellerReviewsViewModel);
            seeSellerReviewsView.Activate();
        }

        public void OnSeeReviewsRightProductClicked(object sender, RoutedEventArgs e)
        {
            App.seeSellerReviewsViewModel.seller = ViewModel.RightProduct.Seller;
            var seeSellerReviewsView = new SeeSellerReviewsView(App.seeSellerReviewsViewModel);
            seeSellerReviewsView.Activate();
        }

        private void LoadImages()
        {
            LeftImageCarousel.Items.Clear();
            RightImageCarousel.Items.Clear();

            foreach (var image in ViewModel.LeftProduct.Images)
            {
                Debug.WriteLine("Loading image: " + image.url);

                var img = new Microsoft.UI.Xaml.Controls.Image
                {
                    Source = new BitmapImage(new Uri(image.url)),
                    Stretch = Stretch.Uniform, // ✅ shows full image without cropping
                    Height = 250,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                LeftImageCarousel.Items.Add(img);
            }

            foreach (var image in ViewModel.RightProduct.Images)
            {
                Debug.WriteLine("Loading image: " + image.url);

                var img = new Microsoft.UI.Xaml.Controls.Image
                {
                    Source = new BitmapImage(new Uri(image.url)),
                    Stretch = Stretch.Uniform, // ✅ shows full image without cropping
                    Height = 250,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                RightImageCarousel.Items.Add(img);
            }
        }
    }
}
