using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    // Represents an item in the shopping basket with its associated product, quantity, and price
    public class BasketItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public bool HasValidPrice { get; private set; }

        public string FormattedPrice => $"${Price:F2}";

        private const int DEFAULT_PRICE = 0;

        public BasketItem(int id, Product product, int quantity)
        {
            this.Id = id;
            this.Product = product;
            this.Quantity = quantity;

            // Set the price based on the product type (only BuyProduct is allowed in basket)
            if (product is BuyProduct buyProduct)
            {
                this.Price = buyProduct.Price;
                this.HasValidPrice = true;
            }
            else
            {
                // Invalid product type
                this.Price = DEFAULT_PRICE;
                this.HasValidPrice = false;
            }
        }

        public float GetPrice()
        {
            // Calculates the total price for this basket item (unit price × quantity)
            return (float)(Price * Quantity);
        }
    }
}
