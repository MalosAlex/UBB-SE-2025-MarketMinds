using DomainLayer.Domain;
using MarketMinds.Repositories.BorrowProductsRepository;
using MarketMinds.Services.ProductTagService;

namespace MarketMinds.Services.BorrowProductsService;

public class BorrowProductsService : ProductService, IBorrowProductsService
{
    private IBorrowProductsRepository borrowProductsRepository;

    public BorrowProductsService(IBorrowProductsRepository repository) : base(repository)
    {
        borrowProductsRepository = repository;
    }

    public void CreateListing(Product product)
    {
        borrowProductsRepository.AddProduct(product);
    }

    public void DeleteListing(Product product)
    {
        borrowProductsRepository.DeleteProduct(product);
    }
}