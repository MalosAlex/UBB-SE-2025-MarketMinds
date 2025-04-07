using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Repositories.ProductConditionRepository;

namespace MarketMinds.Services.ProductConditionService
{
    public class ProductConditionService : IProductConditionService
    {
        private IProductConditionRepository repository;

        public ProductConditionService(IProductConditionRepository repository)
        {
            this.repository = repository;
        }

        public List<ProductCondition> GetAllProductConditions()
        {
            return repository.GetAllProductConditions();
        }

        public ProductCondition CreateProductCondition(string displayTitle, string description)
        {
            return repository.CreateProductCondition(displayTitle, description);
        }

        public void DeleteProductCondition(string displayTitle)
        {
            repository.DeleteProductCondition(displayTitle);
        }
    }
}
