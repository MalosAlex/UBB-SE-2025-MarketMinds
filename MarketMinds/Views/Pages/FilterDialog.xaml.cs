using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DomainLayer.Domain;
using BusinessLogicLayer.ViewModel;
using ViewModelLayer.ViewModel;

namespace UiLayer
{
    public partial class FilterDialog : ContentDialog
    {
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private readonly ProductTagViewModel productTagViewModel;
        private readonly ProductConditionViewModel productConditionViewModel;
        private readonly ProductCategoryViewModel productCategoryViewModel;

        // Full lists
        private List<ProductCategory> fullCategories;
        private List<ProductTag> fullTags;

        // Displayed lists for filtering with pagination
        public ObservableCollection<ProductCondition> ProductConditions { get; set; }
        public ObservableCollection<ProductCategory> DisplayedCategories { get; set; }
        public ObservableCollection<ProductTag> DisplayedTags { get; set; }

        // Pagination counts
        private int initialDisplayCount = 5;
        private int additionalDisplayCount = 10;

        public FilterDialog(SortAndFilterViewModel sortAndFilterViewModel)
        {
            this.InitializeComponent();
            this.sortAndFilterViewModel = sortAndFilterViewModel;

            // Initialize the view models.
            productConditionViewModel = MarketMinds.App.ProductConditionViewModel;
            productCategoryViewModel = MarketMinds.App.ProductCategoryViewModel;
            productTagViewModel = MarketMinds.App.ProductTagViewModel;

            // Initialize full lists
            ProductConditions = new ObservableCollection<ProductCondition>(productConditionViewModel.GetAllProductConditions());
            fullCategories = productCategoryViewModel.GetAllProductCategories();
            fullTags = productTagViewModel.GetAllProductTags();

            // Initialize displayed lists (with pagination)
            DisplayedCategories = new ObservableCollection<ProductCategory>(fullCategories.Take(initialDisplayCount));
            DisplayedTags = new ObservableCollection<ProductTag>(fullTags.Take(initialDisplayCount));

            // Bind to ListViews
            ConditionListView.ItemsSource = ProductConditions;
            CategoryListView.ItemsSource = DisplayedCategories;
            TagListView.ItemsSource = DisplayedTags;

            // Pre-select active filters from view model
            PreselectActiveFilters();

            // Update View More buttons if needed
            UpdateViewMoreButtons();

            this.PrimaryButtonClick += FilterDialog_PrimaryButtonClick;
        }

        private void PreselectActiveFilters()
        {
            // Pre-select Conditions
            foreach (var condition in sortAndFilterViewModel.SelectedConditions)
            {
                var item = ProductConditions.FirstOrDefault(c => c.DisplayTitle == condition.DisplayTitle);
                if (item != null && !ConditionListView.SelectedItems.Contains(item))
                {
                    ConditionListView.SelectedItems.Add(item);
                }
            }

            // Pre-select Categories (if they are in fullCategories)
            foreach (var category in sortAndFilterViewModel.SelectedCategories)
            {
                var item = fullCategories.FirstOrDefault(c => c.DisplayTitle == category.DisplayTitle);
                if (item != null && !CategoryListView.SelectedItems.Contains(item))
                {
                    CategoryListView.SelectedItems.Add(item);
                }
            }

            // Pre-select Tags
            foreach (var tag in sortAndFilterViewModel.SelectedTags)
            {
                var item = fullTags.FirstOrDefault(t => t.DisplayTitle == tag.DisplayTitle);
                if (item != null && !TagListView.SelectedItems.Contains(item))
                {
                    TagListView.SelectedItems.Add(item);
                }
            }
        }

        private void UpdateViewMoreButtons()
        {
            ViewMoreCategoriesButton.Visibility = fullCategories.Count > DisplayedCategories.Count ? Visibility.Visible : Visibility.Collapsed;
            ViewMoreTagsButton.Visibility = fullTags.Count > DisplayedTags.Count ? Visibility.Visible : Visibility.Collapsed;
        }

        private void FilterDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Clear existing selections in the view model
            sortAndFilterViewModel.HandleClearAllFilters();

            // Add currently selected Conditions
            foreach (ProductCondition condition in ConditionListView.SelectedItems)
            {
                sortAndFilterViewModel.HandleAddProductCondition(condition);
            }
            // Add selected Categories
            foreach (ProductCategory category in CategoryListView.SelectedItems)
            {
                sortAndFilterViewModel.HandleAddProductCategory(category);
            }
            // Add selected Tags
            foreach (ProductTag tag in TagListView.SelectedItems)
            {
                sortAndFilterViewModel.HandleAddProductTag(tag);
            }
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear selections in UI and view model
            ConditionListView.SelectedItems.Clear();
            CategoryListView.SelectedItems.Clear();
            TagListView.SelectedItems.Clear();
            sortAndFilterViewModel.HandleClearAllFilters();
        }

        // Category Search handling
        private void CategorySearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = CategorySearchBox.Text.ToLower();
            var filtered = fullCategories.Where(c => c.DisplayTitle.ToLower().Contains(query)).ToList();
            DisplayedCategories.Clear();
            foreach (var cat in filtered.Take(initialDisplayCount))
            {
                DisplayedCategories.Add(cat);
            }
            UpdateViewMoreButtons();
        }

        // Tag Search handling
        private void TagSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = TagSearchBox.Text.ToLower();
            var filtered = fullTags.Where(t => t.DisplayTitle.ToLower().Contains(query)).ToList();
            DisplayedTags.Clear();
            foreach (var tag in filtered.Take(initialDisplayCount))
            {
                DisplayedTags.Add(tag);
            }
            UpdateViewMoreButtons();
        }

        // View More for Categories
        private void ViewMoreCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            var currentCount = DisplayedCategories.Count;
            var query = CategorySearchBox.Text.ToLower();
            var filtered = fullCategories.Where(c => c.DisplayTitle.ToLower().Contains(query)).ToList();
            foreach (var cat in filtered.Skip(currentCount).Take(additionalDisplayCount))
            {
                DisplayedCategories.Add(cat);
            }
            UpdateViewMoreButtons();
        }

        // View More for Tags
        private void ViewMoreTagsButton_Click(object sender, RoutedEventArgs e)
        {
            var currentCount = DisplayedTags.Count;
            var query = TagSearchBox.Text.ToLower();
            var filtered = fullTags.Where(t => t.DisplayTitle.ToLower().Contains(query)).ToList();
            foreach (var tag in filtered.Skip(currentCount).Take(additionalDisplayCount))
            {
                DisplayedTags.Add(tag);
            }
            UpdateViewMoreButtons();
        }
    }
}
