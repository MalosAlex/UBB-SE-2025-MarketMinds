using DomainLayer.Domain;
using MarketMinds.Repositories.ProductConditionRepository;
using System.Collections.Generic;

namespace MarketMinds.Test.Services.ProductConditionService
{
    internal class ProductConditionRepositoryMock : IProductConditionRepository
    {
        public List<ProductCondition> Conditions { get; set; } = new List<ProductCondition>();
        private int currentIndex = 0;

        public ProductConditionRepositoryMock() 
        { 
            Conditions = new List<ProductCondition>();
        }

        ProductCondition IProductConditionRepository.CreateProductCondition(string displayTitle, string description)
        {
            var newCondition = new ProductCondition(
                id: ++currentIndex,
                displayTitle: displayTitle,
                description: description
            );

            Conditions.Add(newCondition);
            return newCondition;
        }

        void IProductConditionRepository.DeleteProductCondition(string displayTitle)
        {
            Conditions.RemoveAll(cond => cond.DisplayTitle == displayTitle);
        }

        List<ProductCondition> IProductConditionRepository.GetAllProductConditions()
        {
            return Conditions;
        }
    }
}
