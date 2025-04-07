using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string DisplayTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ProductCategory(int id, string displayTitle, string description)
        {
            this.Id = id;
            this.DisplayTitle = displayTitle;
            this.Description = description;
        }
    }
}
