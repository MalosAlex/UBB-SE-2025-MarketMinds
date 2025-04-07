using System.Collections.Generic;
using DomainLayer.Domain;

namespace MarketMinds.Repositories.BorrowProductsRepository
{
    /// <summary>
    /// Interface for managing borrow products in the repository.
    /// </summary>
    public interface IBorrowProductsRepository : IProductsRepository
    {
        /// <summary>
        /// Adds a new borrow product to the repository.
        /// </summary>
        /// <param name="product">The product to add.</param>
        void AddProduct(Product product);

        /// <summary>
        /// Deletes a borrow product from the repository.
        /// </summary>
        /// <param name="product">The product to delete.</param>
        void DeleteProduct(Product product);

        /// <summary>
        /// Retrieves all borrow products from the repository.
        /// </summary>
        /// <returns>A list of all borrow products.</returns>
        List<Product> GetProducts();

        /// <summary>
        /// Retrieves a borrow product by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The product with the given ID.</returns>
        Product GetProductByID(int productId);

        /// <summary>
        /// Updates an existing borrow product in the repository.
        /// </summary>
        /// <param name="product">The product with updated information.</param>
        void UpdateProduct(Product product);
    }
}
