using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class BuyProduct : Product
    {
        public float Price { get; set; }
        public BuyProduct(int id, string title, string description, User seller, ProductCondition productCondition, ProductCategory productCategory,
            List<ProductTag> productTags, List<Image> images, float price)
        {
            this.Id = id;
            this.Title = title;
            this.Description = description;
            this.Seller = seller;
            this.Condition = productCondition;
            this.Category = productCategory;
            this.Tags = productTags;
            this.Seller = seller;
            this.Images = images;
            this.Price = price;
        }
    }
}
