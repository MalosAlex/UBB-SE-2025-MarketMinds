using DomainLayer.Domain;
using MarketMinds.Repositories.ProductCategoryRepository;
using System.Collections.Generic;

namespace MarketMinds.Test.Services.ProductCategoryService
{
    internal class ProductCategoryRepositoryMock : IProductCategoryRepository
    {
        public List<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        private int currentIndex = 0;

        public ProductCategoryRepositoryMock()
        {
            Categories = new List<ProductCategory>();
        }

        public List<ProductCategory> GetAllProductCategories()
        {
            return Categories;
        }

        public ProductCategory CreateProductCategory(string displayTitle, string description)
        {
            var newCategory = new ProductCategory(
                id: ++currentIndex,
                displayTitle: displayTitle,
                description: description
            );

            Categories.Add(newCategory);
            return newCategory;
        }

        public void DeleteProductCategory(string displayTitle)
        {
            Categories.RemoveAll(c => c.DisplayTitle == displayTitle);
        }
    }
}