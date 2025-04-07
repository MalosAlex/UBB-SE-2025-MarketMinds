using DomainLayer.Domain;

namespace MarketMinds.Services.BuyProductsService
{
    /// <summary>
    /// Interface for the buy products service.
    /// </summary>
    public interface IBuyProductsService
    {
        /// <summary>
        /// Creates a new product listing.
        /// </summary>
        /// <param name="product">The product to add.</param>
        void CreateListing(Product product);

        /// <summary>
        /// Deletes an existing product listing.
        /// </summary>
        /// <param name="product">The product to delete.</param>
        void DeleteListing(Product product);
    }
}
