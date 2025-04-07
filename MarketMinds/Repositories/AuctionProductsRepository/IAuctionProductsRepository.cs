using System.Collections.Generic;
using DomainLayer.Domain;

namespace MarketMinds.Repositories.AuctionProductsRepository
{
    /// <summary>
    /// Interface for managing auction products in the repository.
    /// </summary>
    public interface IAuctionProductsRepository : IProductsRepository
    {
        /// <summary>
        /// Adds a new auction product to the repository.
        /// </summary>
        /// <param name="product">The product to add.</param>
        void AddProduct(Product product);

        /// <summary>
        /// Deletes an auction product from the repository.
        /// </summary>
        /// <param name="product">The product to delete.</param>
        void DeleteProduct(Product product);

        /// <summary>
        /// Retrieves all auction products from the repository.
        /// </summary>
        /// <returns>A list of all auction products.</returns>
        List<Product> GetProducts();

        /// <summary>
        /// Retrieves an auction product by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The product with the given ID.</returns>
        AuctionProduct GetProductByID(int productId);

        /// <summary>
        /// Updates an existing auction product in the repository.
        /// </summary>
        /// <param name="product">The product with updated information.</param>
        void UpdateProduct(Product product);
    }
}
