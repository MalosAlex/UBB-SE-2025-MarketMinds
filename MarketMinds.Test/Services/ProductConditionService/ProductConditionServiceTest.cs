using DomainLayer.Domain;
using MarketMinds.Services.ProductConditionService;
using NUnit.Framework;

namespace MarketMinds.Test.Services.ProductConditionService
{
    [TestFixture]
    internal class ProductConditionServiceTest
    {
        private ProductConditionRepositoryMock mockRepository;
        private IProductConditionService service;

        [SetUp]
        public void Setup()
        {
            mockRepository = new ProductConditionRepositoryMock();
            service = new MarketMinds.Services.ProductConditionService.ProductConditionService(mockRepository);
        }

        [Test]
        public void GetAllProductConditions_ShouldReturnAllConditions()
        {
            mockRepository.Conditions.Add(new ProductCondition(1, "New", "Never used"));
            mockRepository.Conditions.Add(new ProductCondition(2, "Used", "Shows use marks"));

            var result = service.GetAllProductConditions();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].DisplayTitle, Is.EqualTo("New"));
            Assert.That(result[1].DisplayTitle, Is.EqualTo("Used"));
            Assert.That(result[0].Description, Is.EqualTo("Never used"));
            Assert.That(result[1].Description, Is.EqualTo("Shows use marks"));
        }

        [Test]
        public void CreateProductCategory_ShouldCreateAndReturnNewCategory()
        {
            string displayTitle = "New";
            string description = "Never used";

            var result = service.CreateProductCondition(displayTitle, description);

            Assert.That(result.DisplayTitle, Is.EqualTo(displayTitle));
            Assert.That(result.Description, Is.EqualTo(description));
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(mockRepository.Conditions.Count, Is.EqualTo(1));
        }

        [Test]
        public void DeleteProductCategory_ShouldRemoveCategoryFromList()
        {
            string displayTitle = "Condition to Delete";
            mockRepository.Conditions.Add(new ProductCondition(1, displayTitle, "Description"));
            Assert.That(mockRepository.Conditions.Count, Is.EqualTo(1));

            service.DeleteProductCondition(displayTitle);

            Assert.That(mockRepository.Conditions.Count, Is.EqualTo(0));
        }
    }
}
