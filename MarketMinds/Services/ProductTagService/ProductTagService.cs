using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Repositories.ProductTagRepository;

namespace MarketMinds.Services.ProductTagService
{
    public class ProductTagService : IProductTagService
    {
        private IProductTagRepository repository;

        public ProductTagService(IProductTagRepository repository)
        {
            this.repository = repository;
        }

        public List<ProductTag> GetAllProductTags()
        {
            return repository.GetAllProductTags();
        }

        public ProductTag CreateProductTag(string displayTitle)
        {
            return repository.CreateProductTag(displayTitle);
        }

        public void DeleteProductTag(string displayTitle)
        {
            repository.DeleteProductTag(displayTitle);
        }
    }
}
