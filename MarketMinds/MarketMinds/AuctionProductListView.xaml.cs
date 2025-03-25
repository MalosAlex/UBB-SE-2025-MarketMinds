using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.ViewModel;

namespace UiLayer
{
    public sealed partial class AuctionProductListView : Window
    {
        private readonly AuctionProductsViewModel auctionProductsViewModel;
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private ObservableCollection<AuctionProduct> auctionProducts;

        // Pagination variables
        private int currentPage = 1;
        private int itemsPerPage = 20;
        private int totalPages = 1;
        private List<AuctionProduct> currentFullList; 

        public AuctionProductListView()
        {
            this.InitializeComponent();

            auctionProductsViewModel = MarketMinds.App.auctionProductsViewModel;
            sortAndFilterViewModel = MarketMinds.App.auctionProductSortAndFilterViewModel;

            auctionProducts = new ObservableCollection<AuctionProduct>();
            // Initially load all auction products
            currentFullList = auctionProductsViewModel.GetAllAuctionProducts();
            ApplyFiltersAndPagination();
        }

        // Call this method whenever a filter, sort, or search query changes.
        private void ApplyFiltersAndPagination()
        {
            // Get the filtered and sorted list from view model
            // (Assume handleSearch returns List<Product> that we can cast to AuctionProduct)
            var filteredProducts = sortAndFilterViewModel.handleSearch().Cast<AuctionProduct>().ToList();
            currentFullList = filteredProducts;

            // Reset current page if necessary
            currentPage = 1;
            totalPages = (int)Math.Ceiling(currentFullList.Count / (double)itemsPerPage);
            LoadCurrentPage();
        }

        private void LoadCurrentPage()
        {
            var pageItems = currentFullList
                                .Skip((currentPage - 1) * itemsPerPage)
                                .Take(itemsPerPage)
                                .ToList();

            auctionProducts.Clear();
            foreach (var item in pageItems)
                auctionProducts.Add(item);

            UpdatePaginationDisplay();
        }

        private void UpdatePaginationDisplay()
        {
            PaginationTextBlock.Text = $"Page {currentPage} of {totalPages + 1}";
            PreviousButton.IsEnabled = currentPage > 1;
            NextButton.IsEnabled = currentPage < totalPages;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadCurrentPage();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadCurrentPage();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update the search query in the view model and reapply filters
            sortAndFilterViewModel.handleSearchQueryChange(SearchTextBox.Text);
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
                var sortType = ParseSortType(sortTag);
                if (sortType != null)
                {
                    sortAndFilterViewModel.handleSortChange(sortType);
                    ApplyFiltersAndPagination();
                }
            }
        }

        private ProductSortType ParseSortType(string sortTag)
        {
            // Adjust the external/internal titles as needed.
            switch (sortTag)
            {
                case "SellerRatingAsc":
                    return new ProductSortType("Seller Rating", "SellerRating", true);
                case "SellerRatingDesc":
                    return new ProductSortType("Seller Rating", "SellerRating", false);
                case "StartingPriceAsc":
                    return new ProductSortType("Starting Price", "StartingPrice", true);
                case "StartingPriceDesc":
                    return new ProductSortType("Starting Price", "StartingPrice", false);
                case "CurrentPriceAsc":
                    return new ProductSortType("Current Price", "CurrentPrice", true);
                case "CurrentPriceDesc":
                    return new ProductSortType("Current Price", "CurrentPrice", false);
                default:
                    return null;
            }
        }
    }
}
