using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class Bid
    {
        public User Bidder { get; set; }
        public float Price { get; set; }

        public DateTime Timestamp { get; set; }

        public Bid(User bidder, float price, DateTime timestamp)
        {
            Bidder = bidder;
            Price = price;
            Timestamp = timestamp;
        }
    }
}
