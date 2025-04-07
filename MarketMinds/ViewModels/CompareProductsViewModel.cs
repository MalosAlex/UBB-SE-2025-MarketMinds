using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Services;

namespace ViewModelLayer.ViewModel
{
    public class CompareProductsViewModel
    {
        public Product LeftProduct;
        public Product RightProduct;
        private readonly ProductComparisonService comparisonService;

        public CompareProductsViewModel()
        {
            comparisonService = new ProductComparisonService();
        }

        public bool AddProductForCompare(Product product)
        {
            var result = comparisonService.AddProduct(LeftProduct, RightProduct, product);
            LeftProduct = result.LeftProduct;
            RightProduct = result.RightProduct;
            return result.IsComplete;
        }
    }
}