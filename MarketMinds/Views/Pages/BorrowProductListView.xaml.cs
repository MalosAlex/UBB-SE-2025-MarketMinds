using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.ViewModel;
using MarketMinds;
using MarketMinds.Services;
using MarketMinds.Views.Pages;

namespace UiLayer
{
    public sealed partial class BorrowProductListView : Window
    {
        private readonly BorrowProductsViewModel borrowProductsViewModel;
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private ObservableCollection<BorrowProduct> borrowProducts;
        private CompareProductsViewModel compareProductsViewModel;
        private readonly BorrowProductListService borrowProductListService;
        private readonly BorrowSortTypeConverterService sortTypeConverterService;

        // Pagination variables
        private int currentPage = 1;
        private int totalPages = 1;
        private List<BorrowProduct> currentFullList;

        public BorrowProductListView()
        {
            this.InitializeComponent();

            // Initialize view models and services
            borrowProductsViewModel = MarketMinds.App.BorrowProductsViewModel;
            sortAndFilterViewModel = MarketMinds.App.BorrowProductSortAndFilterViewModel;
            compareProductsViewModel = MarketMinds.App.CompareProductsViewModel;
            borrowProductListService = new BorrowProductListService();
            sortTypeConverterService = new BorrowSortTypeConverterService();

            borrowProducts = new ObservableCollection<BorrowProduct>();
            currentFullList = borrowProductsViewModel.GetAllProducts();
            RefreshProductList();
        }

        private void BorrowListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedProduct = e.ClickedItem as BorrowProduct;
            if (selectedProduct != null)
            {
                // Create and show the detail view
                var detailView = new BorrowProductView(selectedProduct);
                detailView.Activate();
            }
        }

        private void RefreshProductList()
        {
            var (pageItems, newTotalPages, fullList) = borrowProductListService.GetBorrowProductsPage(
                borrowProductsViewModel, sortAndFilterViewModel, currentPage);
            currentFullList = fullList;
            totalPages = newTotalPages;
            borrowProducts.Clear();
            foreach (var item in pageItems)
            {
                borrowProducts.Add(item);
            }
            // Update UI elements
            EmptyMessageTextBlock.Visibility = borrowProducts.Count == 0 ?
                Visibility.Visible : Visibility.Collapsed;
            UpdatePaginationDisplay();
        }

        private void UpdatePaginationDisplay()
        {
            PaginationTextBlock.Text = borrowProductListService.GetPaginationText(currentPage, totalPages);
            var (hasPrevious, hasNext) = borrowProductListService.GetPaginationState(currentPage, totalPages);
            PreviousButton.IsEnabled = hasPrevious;
            NextButton.IsEnabled = hasNext;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                RefreshProductList();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                RefreshProductList();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            sortAndFilterViewModel.HandleSearchQueryChange(SearchTextBox.Text);
            currentPage = 1; // Reset to first page on new search
            RefreshProductList();
        }

        private async void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterDialog filterDialog = new FilterDialog(sortAndFilterViewModel);
            filterDialog.XamlRoot = Content.XamlRoot;
            var result = await filterDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                currentPage = 1; // Reset to first page on new filter
                RefreshProductList();
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            SortingComboBox.Visibility = SortingComboBox.Visibility == Visibility.Visible ?
                                         Visibility.Collapsed : Visibility.Visible;
        }

        private void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortingComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var sortTag = selectedItem.Tag.ToString();
                var sortType = sortTypeConverterService.Convert(sortTag);
                if (sortType != null)
                {
                    sortAndFilterViewModel.HandleSortChange(sortType);
                    currentPage = 1; // Reset to first page on new sort
                    RefreshProductList();
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
