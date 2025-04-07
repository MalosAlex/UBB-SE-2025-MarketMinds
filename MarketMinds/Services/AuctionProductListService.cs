using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.ViewModel;

namespace MarketMinds.Services
{
    public class AuctionProductListService
    {
        private const int NO_ITEMS = 0;

        public (List<AuctionProduct> pageItems, int totalPages, List<AuctionProduct> fullList) GetAuctionProductsPage(
            ViewModelLayer.ViewModel.AuctionProductsViewModel auctionProductsViewModel,
            BusinessLogicLayer.ViewModel.SortAndFilterViewModel sortAndFilterViewModel,
            int currentPage,
            int itemsPerPage)
        {
            var filteredProducts = sortAndFilterViewModel.HandleSearch().Cast<AuctionProduct>().ToList();
            var fullList = filteredProducts;

            int totalPages = (int)Math.Ceiling(fullList.Count / (double)itemsPerPage);

            var pageItems = fullList
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            return (pageItems, totalPages, fullList);
        }

        public bool ShouldShowEmptyMessage(List<AuctionProduct> pageItems)
        {
            return pageItems.Count == NO_ITEMS;
        }

        public (bool canGoToPrevious, bool canGoToNext) GetPaginationButtonState(int currentPage, int totalPages, int basePage)
        {
            bool canGoToPrevious = currentPage > basePage;
            bool canGoToNext = currentPage < totalPages;
            return (canGoToPrevious, canGoToNext);
        }

        public string GetPaginationText(int currentPage, int totalPages)
        {
            return totalPages == NO_ITEMS ?
                $"Page {currentPage} of 1" :
                $"Page {currentPage} of {totalPages}";
        }
    }
}