using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.ViewModel; // Assuming the ViewModel namespace

namespace MarketMinds.Services
{
    public class BuyProductListService
    {
        public (IEnumerable<BuyProduct> currentPageProducts, int totalPages, List<BuyProduct> fullList) GetBuyProductsPage(ViewModelLayer.ViewModel.BuyProductsViewModel buyProductsViewModel, BusinessLogicLayer.ViewModel.SortAndFilterViewModel sortAndFilterViewModel, int currentPage)
        {
            var filteredProducts = sortAndFilterViewModel.HandleSearch();
            var fullList = filteredProducts.Cast<BuyProduct>().ToList();

            var paginationService = new ProductPaginationService();
            var (currentPageProducts, totalPages) = paginationService.GetPaginatedProducts(fullList, currentPage);
            return (currentPageProducts, totalPages, fullList);
        }
    }
}