using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class Image
    {
        public string Url { get; set; } = string.Empty;

        public Image(string url)
        {
            this.Url = url;
        }
    }
}
