using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ViewModelLayer.ViewModel;
using DomainLayer.Domain;
using MarketMinds;
using MarketMinds.Services;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Newtonsoft.Json;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using Microsoft.Extensions.Configuration;

namespace UiLayer
{
    public sealed partial class CreateListingView : Page
    {
        private CreateListingViewModelBase viewModel;
        private ProductCategoryViewModel productCategoryViewModel;
        private ProductConditionViewModel productConditionViewModel;
        private ProductTagViewModel productTagViewModel;
        private TextBox titleTextBox;
        private TextBlock titleErrorTextBlock;
        private ComboBox categoryComboBox;
        private TextBlock categoryErrorTextBlock;
        private TextBox descriptionTextBox;
        private TextBlock descriptionErrorTextBlock;
        private TextBox tagsTextBox;
        private TextBlock tagsErrorTextBlock;
        private ListView tagsListView;
        private TextBox imagesTextBox;
        private ComboBox conditionComboBox;
        private TextBlock conditionErrorTextBlock;
        private ObservableCollection<string> tags;
        private Button uploadImageButton;
        private TextBlock imagesTextBlock;
        private TextBlock categoryTextBlock;
        private TextBlock conditionTextBlock;
        private MainWindow window;
        private readonly string imgurClientId;
        private static readonly HttpClient HttpClient = new HttpClient();
        private const int NO_ITEMS = 0;
        private const int BASE_PAGE = 1;
        private const int NO_REVIEWS = 0;
        private const int NO_MB = 10;
        private const int NO_MB_LIMIT = 10 * 1024 * 1024;
        private const int MAX_RETRIES = 3;
        private const int MAX_RETRY_DELAY = 2;
        private const int MAX_CLIENT_ID_LENGTH = 20;
        private const int NO_RETRY = 0;
        private readonly TagManagementService tagManagementService;
        private readonly ImageUploadService imageUploadService;
        private readonly ListingFormValidationService validationService;

        public CreateListingView(MainWindow mainWindow)
        {
            this.InitializeComponent();
            imgurClientId = App.Configuration.GetSection("ImgurSettings:ClientId").Value;
            titleTextBox = new TextBox { PlaceholderText = "Title" };
            titleErrorTextBlock = new TextBlock { Text = "Title cannot be empty.", Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed };
            categoryTextBlock = new TextBlock { Text = "Select Category" };
            categoryComboBox = new ComboBox();
            categoryErrorTextBlock = new TextBlock { Text = "Please select a category.", Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed };
            descriptionTextBox = new TextBox { PlaceholderText = "Description" };
            descriptionErrorTextBlock = new TextBlock { Text = "Description cannot be empty.", Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed };
            tagsTextBox = new TextBox { PlaceholderText = "Tags" };
            tagsErrorTextBlock = new TextBlock { Text = "Please add at least one tag.", Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed };
            tagsListView = new ListView();
            imagesTextBox = new TextBox { PlaceholderText = "Images" };
            conditionTextBlock = new TextBlock { Text = "Select Condition" };
            conditionComboBox = new ComboBox();
            conditionErrorTextBlock = new TextBlock { Text = "Please select a condition.", Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed };
            tags = new ObservableCollection<string>();
            uploadImageButton = new Button { Content = "Upload Image", Width = 150 };
            uploadImageButton.Click += OnUploadImageClick;
            imagesTextBlock = new TextBlock { TextWrapping = TextWrapping.Wrap };

            // Use singleton instances from App class
            productCategoryViewModel = App.ProductCategoryViewModel;
            productConditionViewModel = App.ProductConditionViewModel;
            productTagViewModel = App.ProductTagViewModel;

            // Initialize services
            tagManagementService = new TagManagementService(productTagViewModel);
            imageUploadService = new ImageUploadService();
            validationService = new ListingFormValidationService();

            // Load categories and conditions into ComboBoxes
            LoadCategories();
            LoadConditions();

            // Set up event handler for tagsTextBox
            tagsTextBox.KeyDown += TagsTextBox_KeyDown;

            // Set up the ListView item template
            tagsListView.ItemClick += TagsListView_ItemClick;
            tagsListView.IsItemClickEnabled = true;

            tagsListView.ItemsSource = tags;
            window = mainWindow;
        }

        private void LoadCategories()
        {
            List<ProductCategory> categories = productCategoryViewModel.GetAllProductCategories();
            categoryComboBox.ItemsSource = categories;
            categoryComboBox.DisplayMemberPath = "DisplayTitle";
            categoryComboBox.SelectedValuePath = "Id";
        }

        private void LoadConditions()
        {
            List<ProductCondition> conditions = productConditionViewModel.GetAllProductConditions();
            conditionComboBox.ItemsSource = conditions;
            conditionComboBox.DisplayMemberPath = "DisplayTitle";
            conditionComboBox.SelectedValuePath = "Id";
        }

        private void ListingTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listingTypeErrorTextBlock.Visibility = Visibility.Collapsed;

            var selectedType = (e.AddedItems[0] as ComboBoxItem)?.Content.ToString();
            FormContainer.Children.Clear();

            switch (selectedType)
            {
                case "Buy":
                    viewModel = new CreateBuyListingViewModel { BuyProductsService = App.BuyProductsService };
                    AddBuyProductFields();
                    break;
                case "Borrow":
                    viewModel = new CreateBorrowListingViewModel { BorrowProductsService = App.BorrowProductsService };
                    AddBorrowProductFields();
                    break;
                case "Auction":
                    viewModel = new CreateAuctionListingViewModel { AuctionProductsService = App.AuctionProductsService };
                    AddAuctionProductFields();
                    break;
            }
        }

        private void AddCommonFields()
        {
            FormContainer.Children.Add(titleTextBox);
            FormContainer.Children.Add(titleErrorTextBlock);
            FormContainer.Children.Add(categoryTextBlock);
            FormContainer.Children.Add(categoryComboBox);
            FormContainer.Children.Add(categoryErrorTextBlock);
            FormContainer.Children.Add(descriptionTextBox);
            FormContainer.Children.Add(descriptionErrorTextBlock);
            FormContainer.Children.Add(tagsTextBox);
            FormContainer.Children.Add(tagsErrorTextBlock);
            FormContainer.Children.Add(tagsListView);
            FormContainer.Children.Add(uploadImageButton);
            FormContainer.Children.Add(imagesTextBlock);
            FormContainer.Children.Add(conditionTextBlock);
            FormContainer.Children.Add(conditionComboBox);
            FormContainer.Children.Add(conditionErrorTextBlock);
        }

        private void AddBuyProductFields()
        {
            AddCommonFields();
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Price", Name = "PriceTextBox" });
        }

        private void AddBorrowProductFields()
        {
            AddCommonFields();
            var timeLimistDatePicker = new CalendarDatePicker
            {
                PlaceholderText = "Time Limit",
                Name = "TimeLimitDatePicker",
                MinDate = DateTimeOffset.Now
            };
            FormContainer.Children.Add(timeLimistDatePicker);
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Daily Rate", Name = "DailyRateTextBox" });
        }

        private void AddAuctionProductFields()
        {
            AddCommonFields();
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Starting Price", Name = "StartingPriceTextBox" });
            var endAuctionDatePicker = new CalendarDatePicker
            {
                PlaceholderText = "End Auction Date",
                Name = "EndAuctionDatePicker",
                MinDate = DateTimeOffset.Now
            };
            FormContainer.Children.Add(endAuctionDatePicker);
        }

        private void TagsTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string tag = tagsTextBox.Text.Trim();
                if (tagManagementService.AddTagToCollection(tag, tags))
                {
                    tagsTextBox.Text = string.Empty;
                }
            }
        }

        private void TagsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            string tag = e.ClickedItem as string;
            tagManagementService.RemoveTagFromCollection(tag, tags);
        }

        private async void OnUploadImageClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string updatedImagesString = await imageUploadService.AddImageToCollection(window.CreateListingViewWindow, viewModel.ImagesString);
                if (updatedImagesString != viewModel.ImagesString)
                {
                    viewModel.ImagesString = updatedImagesString;
                    imagesTextBlock.Text = viewModel.ImagesString;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("Image Upload Error", ex.Message);
            }
        }

        private async Task ShowErrorDialog(string title, string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await errorDialog.ShowAsync();
        }

        private async void ShowSuccessMessage(string message)
        {
            ContentDialog successDialog = new ContentDialog
            {
                Title = "Success",
                Content = message,
                CloseButtonText = "Ok",
                XamlRoot = this.XamlRoot
            };
            await successDialog.ShowAsync();
        }

        private void CreateListingButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset error messages
            titleErrorTextBlock.Visibility = Visibility.Collapsed;
            categoryErrorTextBlock.Visibility = Visibility.Collapsed;
            descriptionErrorTextBlock.Visibility = Visibility.Collapsed;
            tagsErrorTextBlock.Visibility = Visibility.Collapsed;
            conditionErrorTextBlock.Visibility = Visibility.Collapsed;
            listingTypeErrorTextBlock.Visibility = Visibility.Collapsed;

            // Check if a listing type is selected
            if (ListingTypeComboBox.SelectedItem == null)
            {
                listingTypeErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            // Collect common data
            string title = titleTextBox.Text;
            ProductCategory category = (ProductCategory)categoryComboBox.SelectedItem;
            string description = descriptionTextBox.Text;
            ProductCondition condition = (ProductCondition)conditionComboBox.SelectedItem;

            // Validate common fields
            if (!validationService.ValidateCommonFields(title, category, description, tags, condition, out string errorMessage, out string errorField))
            {
                switch (errorField)
                {
                    case "Title":
                        titleErrorTextBlock.Text = errorMessage;
                        titleErrorTextBlock.Visibility = Visibility.Visible;
                        break;
                    case "Category":
                        categoryErrorTextBlock.Text = errorMessage;
                        categoryErrorTextBlock.Visibility = Visibility.Visible;
                        break;
                    case "Description":
                        descriptionErrorTextBlock.Text = errorMessage;
                        descriptionErrorTextBlock.Visibility = Visibility.Visible;
                        break;
                    case "Tags":
                        tagsErrorTextBlock.Text = errorMessage;
                        tagsErrorTextBlock.Visibility = Visibility.Visible;
                        break;
                    case "Condition":
                        conditionErrorTextBlock.Text = errorMessage;
                        conditionErrorTextBlock.Visibility = Visibility.Visible;
                        break;
                }
                return;
            }

            // Convert string tags to ProductTag objects
            List<ProductTag> productTags = tagManagementService.ConvertStringTagsToProductTags(tags);

            // Collect specific data based on the selected type
            if (viewModel is CreateBuyListingViewModel)
            {
                string priceText = ((TextBox)FormContainer.FindName("PriceTextBox")).Text;
                if (!validationService.ValidateBuyProductFields(priceText, out float price))
                {
                    titleErrorTextBlock.Text = "Please enter a valid price.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                var product = new BuyProduct(0, title, description, App.CurrentUser, condition, category, productTags, viewModel.Images, price);
                viewModel.CreateListing(product);
            }
            else if (viewModel is CreateBorrowListingViewModel)
            {
                var timeLimitDatePicker = (CalendarDatePicker)FormContainer.FindName("TimeLimitDatePicker");
                if (!timeLimitDatePicker.Date.HasValue)
                {
                    titleErrorTextBlock.Text = "Please select a time limit.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                DateTime endDate = timeLimitDatePicker.Date.Value.DateTime;

                if (!float.TryParse(((TextBox)FormContainer.FindName("DailyRateTextBox")).Text, out float dailyRate))
                {
                    titleErrorTextBlock.Text = "Please enter a valid daily rate.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                var product = new BorrowProduct(0, title, description, App.CurrentUser, condition, category, productTags, viewModel.Images, DateTime.Now, endDate, endDate, dailyRate, false);
                viewModel.CreateListing(product);
            }
            else if (viewModel is CreateAuctionListingViewModel)
            {
                if (!float.TryParse(((TextBox)FormContainer.FindName("StartingPriceTextBox")).Text, out float startingPrice))
                {
                    titleErrorTextBlock.Text = "Please enter a valid starting price.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                if (!((CalendarDatePicker)FormContainer.FindName("EndAuctionDatePicker")).Date.HasValue)
                {
                    titleErrorTextBlock.Text = "Please select an end auction date.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                DateTime endAuctionDate = ((CalendarDatePicker)FormContainer.FindName("EndAuctionDatePicker")).Date.Value.DateTime;

                var product = new AuctionProduct(0, title, description, App.CurrentUser, condition, category, productTags, viewModel.Images, DateTime.Now, endAuctionDate, startingPrice);
                viewModel.CreateListing(product);
            }
            ShowSuccessMessage("Listing created successfully!");
        }
    }
}

