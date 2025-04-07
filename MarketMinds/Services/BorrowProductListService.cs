using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MarketMinds.Services
{
    public class BorrowProductListService
    {
        private const int ItemsPerPage = 20;

        public (List<BorrowProduct> pageItems, int totalPages, List<BorrowProduct> fullList) GetBorrowProductsPage(
            ViewModelLayer.ViewModel.BorrowProductsViewModel borrowProductsViewModel,
            BusinessLogicLayer.ViewModel.SortAndFilterViewModel sortAndFilterViewModel,
            int currentPage)
        {
            // Retrieve filtered and sorted products
            var filteredProducts = sortAndFilterViewModel.HandleSearch()
                                        .Cast<BorrowProduct>().ToList();
            var fullList = filteredProducts;
            int totalPages = (int)Math.Ceiling(fullList.Count / (double)ItemsPerPage);
            // Get current page items
            var pageItems = fullList
                .Skip((currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();
            return (pageItems, totalPages, fullList);
        }
        public (bool hasPrevious, bool hasNext) GetPaginationState(int currentPage, int totalPages)
        {
            return (currentPage > 1, currentPage < totalPages);
        }
        public string GetPaginationText(int currentPage, int totalPages)
        {
            return totalPages == 0 ?
                $"Page {currentPage} of 1" :
                $"Page {currentPage} of {totalPages}";
        }
    }
}