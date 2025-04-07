using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    // Represents a shopping basket which contains basket items
    public class Basket
    {
        public int Id { get; set; }
        private List<BasketItem> items;

        public Basket(int id)
        {
            this.Id = id;
            this.items = new List<BasketItem>();
        }

        public void AddItem(Product product, int quantity)
        {
            // Adds a product to the basket with the specified quantity
            // If the product already exists in the basket, its quantity is increased

            // Check if the product is already in the basket
            BasketItem existingItem = items.FirstOrDefault(i => i.Product.Id == product.Id);

            if (existingItem != null)
            {
                // Update quantity of existing item
                existingItem.Quantity += quantity;
            }
            else
            {
                // Add new item
                BasketItem newItem = new(items.Count + 1, product, quantity);
                items.Add(newItem);
            }
        }

        public List<BasketItem> GetItems()
        {
            // Gets a copy of all items in the basket
            // Return a copy to prevent external modification
            return new List<BasketItem>(items);
        }
    }
}
