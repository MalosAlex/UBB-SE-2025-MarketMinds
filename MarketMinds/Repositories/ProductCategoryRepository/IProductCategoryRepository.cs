using System.Collections.Generic;
using DomainLayer.Domain;

namespace MarketMinds.Repositories.ProductCategoryRepository
{
    /// <summary>
    /// Interface for ProductCategoryRepository to manage product category operations.
    /// </summary>
    public interface IProductCategoryRepository
    {
        /// <summary>
        /// Returns all the product categories.
        /// </summary>
        /// <returns>A list of all product categories.</returns>
        List<ProductCategory> GetAllProductCategories();

        /// <summary>
        /// Creates a new product category.
        /// </summary>
        /// <param name="displayTitle">The display title of the product category.</param>
        /// <param name="description">The description of the product category.</param>
        /// <returns>The created product category.</returns>
        ProductCategory CreateProductCategory(string displayTitle, string description);

        /// <summary>
        /// Deletes a product category by its display title.
        /// </summary>
        /// <param name="displayTitle">The display title of the product category to delete.</param>
        void DeleteProductCategory(string displayTitle);
    }
}
