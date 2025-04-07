using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.ViewModel;
using MarketMinds;
using MarketMinds.Services;
using MarketMinds.Views.Pages;

namespace UiLayer
{
    public sealed partial class AuctionProductListView : Window
    {
        private readonly AuctionProductsViewModel auctionProductsViewModel;
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private ObservableCollection<AuctionProduct> auctionProducts;
        private CompareProductsViewModel compareProductsViewModel;
        private readonly AuctionProductListService auctionProductListService;
        private readonly AuctionSortTypeConverterService sortTypeConverterService;

        // Pagination variables
        private int currentPage = 1;
        private int itemsPerPage = 20;
        private int totalPages = 1;
        private const int BASE_PAGE = 1;
        private const int NO_ITEMS = 0;
        private List<AuctionProduct> currentFullList;

        public AuctionProductListView()
        {
            this.InitializeComponent();

            auctionProductsViewModel = MarketMinds.App.AuctionProductsViewModel;
            sortAndFilterViewModel = MarketMinds.App.AuctionProductSortAndFilterViewModel;
            compareProductsViewModel = MarketMinds.App.CompareProductsViewModel;
            auctionProductListService = new AuctionProductListService();
            sortTypeConverterService = new AuctionSortTypeConverterService();

            auctionProducts = new ObservableCollection<AuctionProduct>();
            // Initially load all auction products
            currentFullList = auctionProductsViewModel.GetAllProducts();
            ApplyFiltersAndPagination();
        }

        private void AuctionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedProduct = e.ClickedItem as AuctionProduct;
            if (selectedProduct != null)
            {
                // Create and show the detail view
                var detailView = new AuctionProductView(selectedProduct);
                detailView.Activate();
            }
        }

        // Call this method whenever a filter, sort, or search query changes.
        private void ApplyFiltersAndPagination()
        {
            var (pageItems, newTotalPages, fullList) = auctionProductListService.GetAuctionProductsPage(
                auctionProductsViewModel, sortAndFilterViewModel, currentPage, itemsPerPage);
            currentFullList = fullList;
            totalPages = newTotalPages;
            auctionProducts.Clear();
            foreach (var item in pageItems)
            {
                auctionProducts.Add(item);
            }

            // Show the empty message if no items exist
            EmptyMessageTextBlock.Visibility = auctionProductListService.ShouldShowEmptyMessage(pageItems)
                ? Visibility.Visible
                : Visibility.Collapsed;

            UpdatePaginationDisplay();
        }

        private void UpdatePaginationDisplay()
        {
            PaginationTextBlock.Text = auctionProductListService.GetPaginationText(currentPage, totalPages);
            var (canGoToPrevious, canGoToNext) = auctionProductListService.GetPaginationButtonState(
                currentPage, totalPages, BASE_PAGE);
            PreviousButton.IsEnabled = canGoToPrevious;
            NextButton.IsEnabled = canGoToNext;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > BASE_PAGE)
            {
                currentPage--;
                ApplyFiltersAndPagination();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                ApplyFiltersAndPagination();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update the search query in the view model and reapply filters
            sortAndFilterViewModel.HandleSearchQueryChange(SearchTextBox.Text);
            currentPage = BASE_PAGE;
            ApplyFiltersAndPagination();
        }

        private async void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Show a ContentDialog for filtering
            FilterDialog filterDialog = new FilterDialog(sortAndFilterViewModel);
            filterDialog.XamlRoot = Content.XamlRoot;
            var result = await filterDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Filters have been applied in the dialog; reapply them.
                currentPage = BASE_PAGE;
                ApplyFiltersAndPagination();
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the sorting dropdown visibility
            SortingComboBox.Visibility = SortingComboBox.Visibility == Visibility.Visible ?
                                         Visibility.Collapsed : Visibility.Visible;
        }

        private void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortingComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                // Use the Tag to determine which sort to apply
                var sortTag = selectedItem.Tag.ToString();
                var sortType = sortTypeConverterService.Convert(sortTag);
                if (sortType != null)
                {
                    sortAndFilterViewModel.HandleSortChange(sortType);
                    currentPage = BASE_PAGE; // Reset to first page on new sort
                    ApplyFiltersAndPagination();
                }
            }
        }

        private void AddToCompare_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedProduct = button.DataContext as Product;
            if (selectedProduct != null)
            {
                bool twoAdded = compareProductsViewModel.AddProductForCompare(selectedProduct);
                if (twoAdded == true)
                {
                    // Create a compare view
                    var compareProductsView = new CompareProductsView(compareProductsViewModel);
                    // Create a window to host the CompareProductsView page
                    var compareWindow = new Window();
                    compareWindow.Content = compareProductsView;
                    compareProductsView.SetParentWindow(compareWindow);
                    compareWindow.Activate();
                }
            }
        }
    }
}
