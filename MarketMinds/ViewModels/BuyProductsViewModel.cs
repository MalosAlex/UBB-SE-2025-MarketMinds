using System.Collections.Generic;
using DomainLayer.Domain;
using MarketMinds.Services.BuyProductsService;

namespace ViewModelLayer.ViewModel;

public class BuyProductsViewModel
{
    private readonly BuyProductsService buyProductsService;

    public BuyProductsViewModel(BuyProductsService buyProductsService)
    {
        this.buyProductsService = buyProductsService;
    }

    public List<BuyProduct> GetAllProducts()
    {
        var buyProducts = new List<BuyProduct>();
        foreach (var product in buyProductsService.GetProducts())
        {
            buyProducts.Add((BuyProduct)product);
        }
        return buyProducts;
    }
}