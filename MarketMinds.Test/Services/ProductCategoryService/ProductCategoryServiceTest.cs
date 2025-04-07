using DomainLayer.Domain;
using MarketMinds.Services.ProductCategoryService;
using NUnit.Framework;

namespace MarketMinds.Test.Services.ProductCategoryService
{
    [TestFixture]
    public class ProductCategoryServiceTests
    {
        private ProductCategoryRepositoryMock mockRepository;
        private IProductCategoryService service;

        [SetUp]
        public void Setup()
        {
            mockRepository = new ProductCategoryRepositoryMock();
            service = new MarketMinds.Services.ProductCategoryService.ProductCategoryService(mockRepository);
        }

        [Test]
        public void GetAllProductCategories_ShouldReturnAllCategories()
        {
            mockRepository.Categories.Add(new ProductCategory(1, "Electronics", "Electronic devices"));
            mockRepository.Categories.Add(new ProductCategory(2, "Clothing", "Apparel items"));

            var result = service.GetAllProductCategories();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].DisplayTitle, Is.EqualTo("Electronics"));
            Assert.That(result[1].DisplayTitle, Is.EqualTo("Clothing"));
        }

        [Test]
        public void CreateProductCategory_ShouldCreateAndReturnNewCategory()
        {
            string displayTitle = "Test Category";
            string description = "Test Description";

            var result = service.CreateProductCategory(displayTitle, description);

            Assert.That(result.DisplayTitle, Is.EqualTo(displayTitle));
            Assert.That(result.Description, Is.EqualTo(description));
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(mockRepository.Categories.Count, Is.EqualTo(1));
        }

        [Test]
        public void DeleteProductCategory_ShouldRemoveCategoryFromList()
        {
            string displayTitle = "Category to Delete";
            mockRepository.Categories.Add(new ProductCategory(1, displayTitle, "Description"));
            Assert.That(mockRepository.Categories.Count, Is.EqualTo(1));

            service.DeleteProductCategory(displayTitle);

            Assert.That(mockRepository.Categories.Count, Is.EqualTo(0));
        }
    }
}