using System.Collections.Generic;
using DomainLayer.Domain;

/// <summary>
/// Interface for managing products in the service layer.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <returns>A list of all products.</returns>
    List<Product> GetProducts();

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product to add.</param>
    void AddProduct(Product product);

    /// <summary>
    /// Deletes an existing product.
    /// </summary>
    /// <param name="product">The product to delete.</param>
    void DeleteProduct(Product product);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product with updated information.</param>
    void UpdateProduct(Product product);

    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to retrieve.</param>
    /// <returns>The product with the specified ID.</returns>
    Product GetProductById(int id);

    /// <summary>
    /// Gets a filtered and sorted list of products based on the provided criteria.
    /// </summary>
    /// <param name="selectedConditions">Conditions to filter by.</param>
    /// <param name="selectedCategories">Categories to filter by.</param>
    /// <param name="selectedTags">Tags to filter by.</param>
    /// <param name="sortCondition">How to sort the results.</param>
    /// <param name="searchQuery">Optional search term to filter by.</param>
    /// <returns>A filtered and sorted list of products.</returns>
    List<Product> GetSortedFilteredProducts(List<ProductCondition> selectedConditions, List<ProductCategory> selectedCategories, List<ProductTag> selectedTags, ProductSortType sortCondition, string searchQuery);
}

