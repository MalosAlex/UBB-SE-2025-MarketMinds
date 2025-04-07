using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Services.ProductTagService;

namespace BusinessLogicLayer.ViewModel
{
    public class SortAndFilterViewModel
    {
        private ProductService productService;

        public List<ProductCondition> SelectedConditions { get; set; }
        public List<ProductCategory> SelectedCategories { get; set; }
        public List<ProductTag> SelectedTags { get; set; }
        public ProductSortType? SortCondition { get; set; }
        public string SearchQuery { get; set; }
        public SortAndFilterViewModel(ProductService productService)
        {
            this.productService = productService;

            this.SelectedConditions = new List<ProductCondition>();
            this.SelectedCategories = new List<ProductCategory>();
            this.SelectedTags = new List<ProductTag>();
            this.SortCondition = null;
            this.SearchQuery = string.Empty;
        }

        public List<Product> HandleSearch()
        {
            return productService.GetSortedFilteredProducts(
                SelectedConditions,
                SelectedCategories,
                SelectedTags,
                SortCondition,
                SearchQuery);
        }

        public void HandleClearAllFilters()
        {
            this.SelectedConditions.Clear();
            this.SelectedCategories.Clear();
            this.SelectedTags.Clear();
            this.SortCondition = null;
            this.SearchQuery = string.Empty;
        }

        public void HandleSortChange(ProductSortType newSortCondition)
        {
            this.SortCondition = newSortCondition;
        }

        public void HandleSearchQueryChange(string searchQuery)
        {
            this.SearchQuery = searchQuery;
        }

        public void HandleAddProductCondition(ProductCondition condition)
        {
            this.SelectedConditions.Add(condition);
        }

        public void HandleRemoveProductCondition(ProductCondition condition)
        {
            this.SelectedConditions.Remove(condition);
        }

        public void HandleAddProductCategory(ProductCategory category)
        {
            this.SelectedCategories.Add(category);
        }

        public void HandleRemoveProductCategory(ProductCategory category)
        {
            this.SelectedCategories.Remove(category);
        }

        public void HandleAddProductTag(ProductTag tag)
        {
            this.SelectedTags.Add(tag);
        }

        public void HandleRemoveProductTag(ProductTag tag)
        {
            this.SelectedTags.Remove(tag);
        }
    }
}
