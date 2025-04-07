using DomainLayer.Domain;

namespace MarketMinds.Services.AuctionProductsService
{
    /// <summary>
    /// Interface for the auction products service.
    /// </summary>
    public interface IAuctionProductsService
    {
        /// <summary>
        /// Creates a new product listing.
        /// </summary>
        /// <param name="product">The product to add.</param>
        void CreateListing(Product product);

        /// <summary>
        /// Places a bid on an auction product.
        /// </summary>
        /// <param name="auction">The auction product.</param>
        /// <param name="bidder">The user placing the bid.</param>
        /// <param name="bidAmount">The amount of the bid.</param>
        void PlaceBid(AuctionProduct auction, User bidder, float bidAmount);

        /// <summary>
        /// Concludes an auction.
        /// </summary>
        /// <param name="auction">The auction product to conclude.</param>
        void ConcludeAuction(AuctionProduct auction);

        /// <summary>
        /// Gets the formatted time left in the auction or "Auction Ended" if it has ended.
        /// </summary>
        /// <param name="auction">The auction product.</param>
        /// <returns>A formatted string representing the time left.</returns>
        string GetTimeLeft(AuctionProduct auction);

        /// <summary>
        /// Checks if an auction has ended.
        /// </summary>
        /// <param name="auction">The auction product.</param>
        /// <returns>True if the auction has ended, false otherwise.</returns>
        bool IsAuctionEnded(AuctionProduct auction);
    }
}
