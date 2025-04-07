using System;
using System.Collections.Generic;
using NUnit.Framework;
using DomainLayer.Domain;
using MarketMinds.Services;
using MarketMinds.Views.Pages;
using Microsoft.UI.Xaml;
using Moq;
using BusinessLogicLayer.ViewModel;

namespace MarketMinds.Test.Services.ProductViewNavigationService
{
    [TestFixture]
    public class ProductViewNavigationServiceTest
    {
        private MarketMinds.Services.ProductViewNavigationService _navigationService;
        private User _seller;

        [SetUp]
        public void Setup()
        {
            _navigationService = new MarketMinds.Services.ProductViewNavigationService();
            _seller = new User(1, "TestSeller", "test@example.com");
        }

        [Test]
        public void CreateProductDetailView_WithNullProduct_ThrowsArgumentNullException()
        {
            Assert.That(() => _navigationService.CreateProductDetailView(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CreateSellerReviewsView_WithNullSeller_ThrowsArgumentNullException()
        {
            Assert.That(() => _navigationService.CreateSellerReviewsView(null), Throws.TypeOf<ArgumentNullException>());
        }
    }
}
