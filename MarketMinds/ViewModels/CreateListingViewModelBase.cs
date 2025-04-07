using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using MarketMinds.Services;

namespace ViewModelLayer.ViewModel
{
    public abstract class CreateListingViewModelBase
    {
        public string Title { get; set; }
        public ProductCategory Category { get; set; }
        public List<ProductTag> Tags { get; set; }
        public string Description { get; set; }
        public List<Image> Images { get; set; }
        public ProductCondition Condition { get; set; }
        private readonly ImageUploadService imageService;

        public CreateListingViewModelBase()
        {
            imageService = new ImageUploadService();
            Images = new List<Image>();
        }

        public string ImagesString
        {
            get => imageService.FormatImagesString(Images);
            set => Images = imageService.ParseImagesString(value);
        }
            public abstract void CreateListing(Product product);
    }
}

