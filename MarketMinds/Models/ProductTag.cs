using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class ProductTag
    {
        public int Id { get; set; }
        public string DisplayTitle { get; set; } = string.Empty;

        public ProductTag(int id, string displayTitle)
        {
            this.Id = id;
            this.DisplayTitle = displayTitle;
        }
    }
}
