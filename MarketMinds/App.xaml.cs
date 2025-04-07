using System;
using System.IO;
using Microsoft.UI.Xaml;
using BusinessLogicLayer.ViewModel;
using DataAccessLayer;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using Microsoft.Extensions.Configuration;
using MarketMinds.Repositories.AuctionProductsRepository;
using MarketMinds.Repositories.BasketRepository;
using MarketMinds.Repositories.BorrowProductsRepository;
using MarketMinds.Repositories.BuyProductsRepository;
using MarketMinds.Repositories.ProductCategoryRepository;
using MarketMinds.Repositories.ProductConditionRepository;
using MarketMinds.Repositories.ProductTagRepository;
using MarketMinds.Repositories.ReviewRepository;
using MarketMinds.Services.AuctionProductsService;
using MarketMinds.Services.BorrowProductsService;
using MarketMinds.Services.BasketService;
using MarketMinds.Services.BuyProductsService;
using MarketMinds.Services.ProductCategoryService;
using MarketMinds.Services.ProductConditionService;
using MarketMinds.Services.ReviewService;
using MarketMinds.Services.ProductTagService;
using MarketMinds.Services;

namespace MarketMinds
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration Configuration;
        public static DataBaseConnection DatabaseConnection;
        // Repository declarations
        public static ProductCategoryRepository ProductCategoryRepository;
        public static ProductConditionRepository ProductConditionRepository;
        public static ProductTagRepository ProductTagRepository;
        public static AuctionProductsRepository AuctionProductsRepository;
        public static BorrowProductsRepository BorrowProductsRepository;
        public static BuyProductsRepository BuyProductsRepository;
        public static ReviewRepository ReviewRepository;
        public static BasketRepository BasketRepository;

        // Service declarations
        public static ProductService ProductService;
        public static BuyProductsService BuyProductsService;
        public static BorrowProductsService BorrowProductsService;
        public static AuctionProductsService AuctionProductsService;
        public static ProductCategoryService CategoryService;
        public static ProductTagService TagService;
        public static ProductConditionService ConditionService;
        public static ReviewsService ReviewsService;
        public static BasketService BasketService;

        // ViewModel declarations
        public static BuyProductsViewModel BuyProductsViewModel { get; private set; }
        public static BorrowProductsViewModel BorrowProductsViewModel { get; private set; }
        public static AuctionProductsViewModel AuctionProductsViewModel { get; private set; }
        public static ProductCategoryViewModel ProductCategoryViewModel { get; private set; }
        public static ProductConditionViewModel ProductConditionViewModel { get; private set; }
        public static ProductTagViewModel ProductTagViewModel { get; private set; }
        public static SortAndFilterViewModel AuctionProductSortAndFilterViewModel { get; private set; }
        public static SortAndFilterViewModel BorrowProductSortAndFilterViewModel { get; private set; }
        public static SortAndFilterViewModel BuyProductSortAndFilterViewModel { get; private set; }
        public static ReviewCreateViewModel ReviewCreateViewModel { get; private set; }
        public static SeeBuyerReviewsViewModel SeeBuyerReviewsViewModel { get; private set; }
        public static SeeSellerReviewsViewModel SeeSellerReviewsViewModel { get; private set; }
        public static BasketViewModel BasketViewModel { get; private set; }
        public static CompareProductsViewModel CompareProductsViewModel { get; private set; }

        public static User CurrentUser { get; set; }
        public static User TestingUser { get; set; }

        private const int BUYER = 1;
        private const int SELLER = 2;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeConfiguration();
            this.InitializeComponent();
        }

        private IConfiguration InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            return Configuration;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            mainWindow = new UiLayer.MainWindow();
            mainWindow.Activate();
            // Create test users that match the database
            TestingUser = new User(1, "alice123", "alice@example.com");
            TestingUser.UserType = BUYER; // Matches database value
            CurrentUser = new User(2, "bob321", "bob@example.com");
            CurrentUser.UserType = SELLER; // Matches database value

            // Instantiate database connection with configuration
            DatabaseConnection = new DataBaseConnection(Configuration);
            // Instantiate repositories
            ProductCategoryRepository = new ProductCategoryRepository(DatabaseConnection);
            ProductConditionRepository = new ProductConditionRepository(DatabaseConnection);
            ProductTagRepository = new ProductTagRepository(DatabaseConnection);
            AuctionProductsRepository = new AuctionProductsRepository(DatabaseConnection);
            BorrowProductsRepository = new BorrowProductsRepository(DatabaseConnection);
            BuyProductsRepository = new BuyProductsRepository(DatabaseConnection);
            ReviewRepository = new ReviewRepository(DatabaseConnection);
            BasketRepository = new BasketRepository(DatabaseConnection);

            // Instantiate services
            ProductService = new ProductService(BorrowProductsRepository);
            BuyProductsService = new BuyProductsService(BuyProductsRepository);
            BorrowProductsService = new BorrowProductsService(BorrowProductsRepository);
            AuctionProductsService = new AuctionProductsService(AuctionProductsRepository);
            CategoryService = new ProductCategoryService(ProductCategoryRepository);
            TagService = new ProductTagService(ProductTagRepository);
            ConditionService = new ProductConditionService(ProductConditionRepository);
            ReviewsService = new ReviewsService(ReviewRepository);
            BasketService = new BasketService(BasketRepository);

            // Instantiate view models
            BuyProductsViewModel = new BuyProductsViewModel(BuyProductsService);
            AuctionProductsViewModel = new AuctionProductsViewModel(AuctionProductsService);
            ProductCategoryViewModel = new ProductCategoryViewModel(CategoryService);
            ProductTagViewModel = new ProductTagViewModel(TagService);
            ProductConditionViewModel = new ProductConditionViewModel(ConditionService);
            BorrowProductsViewModel = new BorrowProductsViewModel(BorrowProductsService);
            AuctionProductSortAndFilterViewModel = new SortAndFilterViewModel(AuctionProductsService);
            BorrowProductSortAndFilterViewModel = new SortAndFilterViewModel(BorrowProductsService);
            BuyProductSortAndFilterViewModel = new SortAndFilterViewModel(BuyProductsService);
            ReviewCreateViewModel = new ReviewCreateViewModel(ReviewsService, CurrentUser, TestingUser);
            SeeSellerReviewsViewModel = new SeeSellerReviewsViewModel(ReviewsService, TestingUser, TestingUser);
            SeeBuyerReviewsViewModel = new SeeBuyerReviewsViewModel(ReviewsService, CurrentUser);
            BasketViewModel = new BasketViewModel(CurrentUser, BasketService);
            CompareProductsViewModel = new CompareProductsViewModel();
        }

        private Window mainWindow;
    }
}
