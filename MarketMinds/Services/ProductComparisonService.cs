using System;
using DomainLayer.Domain;

namespace MarketMinds.Services
{
    public class ProductComparisonService
    {
        public (Product LeftProduct, Product RightProduct, bool IsComplete) AddProduct(Product leftProduct, Product rightProduct, Product newProduct)
        {
            if (leftProduct == null)
            {
                return (newProduct, rightProduct, false);
            }
            if (newProduct != leftProduct)
            {
                return (leftProduct, newProduct, true);
            }
            return (leftProduct, rightProduct, false);
        }
    }
}