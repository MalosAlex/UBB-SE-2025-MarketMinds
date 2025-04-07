using System.Collections.Generic;
using DomainLayer.Domain;

namespace MarketMinds.Repositories.BuyProductsRepository
{
    /// <summary>
    /// Interface for managing buy products in the repository.
    /// </summary>
    public interface IBuyProductsRepository : IProductsRepository
    {
        /// <summary>
        /// Adds a new buy product to the repository.
        /// </summary>
        /// <param name="product">The product to add.</param>
        void AddProduct(Product product);

        /// <summary>
        /// Deletes a buy product from the repository.
        /// </summary>
        /// <param name="product">The product to delete.</param>
        void DeleteProduct(Product product);

        /// <summary>
        /// Retrieves all buy products from the repository.
        /// </summary>
        /// <returns>A list of all buy products.</returns>
        List<Product> GetProducts();

        /// <summary>
        /// Retrieves a buy product by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The product with the given ID.</returns>
        Product GetProductByID(int productId);

        /// <summary>
        /// Updates an existing buy product in the repository.
        /// </summary>
        /// <param name="product">The product with updated information.</param>
        void UpdateProduct(Product product);
    }
}
