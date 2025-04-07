using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using ViewModelLayer.ViewModel;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using DomainLayer.Domain;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using MarketMinds.Services;

namespace MarketMinds
{
    public sealed partial class CreateReviewView : Window
    {
        public ReviewCreateViewModel ViewModel { get; set; }
        private readonly bool var_isEditing;

        public CreateReviewView(ReviewCreateViewModel viewModel, Review? review = null)
        {
            ViewModel = viewModel;
            var_isEditing = review != null;
            if (var_isEditing)
            {
                ViewModel.CurrentReview = review;
                ViewModel.Description = review.Description;
                ViewModel.Rating = review.Rating;
                ViewModel.Images = review.Images;
            }
            this.InitializeComponent();
            this.Closed += OnWindowClosed;
            // Update status message text block
            if (StatusMessageTextBlock != null)
            {
                UpdateStatusMessage();
            }
        }

        private async void HandleAddImage_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.AddImage(this);
            UpdateStatusMessage();
        }

        private void HandleSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                Debug.WriteLine("ViewModel is null!");
                return;
            }

            if (ViewModel.ImagesString.Contains("uploading...", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("Please wait for image upload to complete.");
                return;
            }

            if (var_isEditing)
            {
                ViewModel.UpdateReview();
            }
            else
            {
                ViewModel.SubmitReview();
            }

            UpdateStatusMessage();

            // Allow a moment to see the status message before closing
            Task.Delay(1000).ContinueWith(_ => this.DispatcherQueue.TryEnqueue(() => this.Close()));
        }

        private void OnWindowClosed(object sender, Microsoft.UI.Xaml.WindowEventArgs e)
        {
            // Clear ViewModel properties when the window is closed
            ViewModel.Description = string.Empty;
            ViewModel.ImagesString = string.Empty;
            ViewModel.Rating = 0;
        }

        private async void OnUploadImageClick(object sender, RoutedEventArgs e)
        {
            await ViewModel.AddImage(this);
            UpdateStatusMessage();
        }
        private void UpdateStatusMessage()
        {
            if (StatusMessageTextBlock != null)
            {
                StatusMessageTextBlock.Text = ViewModel.StatusMessage;
            }
        }
    }
}