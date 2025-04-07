using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MarketMinds;
using MarketMinds.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace UiLayer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const int BUYER = 3;
        private const int SELLER = 2;

        public MainWindow()
        {
            this.InitializeComponent();
        }
        private void HandleAuctionProductListViewButton_Click(object sender, RoutedEventArgs e)
        {
            auctionProductListViewWindow = new AuctionProductListView();
            auctionProductListViewWindow.Activate();
        }
        private void HandleBorrowProductListViewButton_Click(object sender, RoutedEventArgs e)
        {
            borrowProductListViewWindow = new BorrowProductListView();
            borrowProductListViewWindow.Activate();
        }
        private void HandleBuyProductListViewButton_Click(object sender, RoutedEventArgs e)
        {
            buyProductListViewWindow = new BuyProductListView();
            buyProductListViewWindow.Activate();
        }
        private void HandleAdminViewButton_Click(object sender, RoutedEventArgs e)
        {
            adminViewWindow = new AdminView();
            adminViewWindow.Activate();
        }
        private void HandleBasketViewButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a window that hosts the BasketView page
            basketViewWindow = new Window();
            basketViewWindow.Content = new BasketView();
            basketViewWindow.Activate();
        }

        private void HandleLeaveReviewButton_Click(object sender, RoutedEventArgs e)
        {
            leaveReviewViewWindow = new CreateReviewView(App.ReviewCreateViewModel);
            leaveReviewViewWindow.Activate();
        }

        private void HandleCreateListingButton_Click(object sender, RoutedEventArgs e)
        {
            CreateListingViewWindow = new Window();
            CreateListingViewWindow.Content = new CreateListingView(this);
            CreateListingViewWindow.Activate();
        }

        private void HandleSeeReviewViewButton_Click(object sender, RoutedEventArgs e)
        {
            // buyer
            if (App.CurrentUser.UserType == BUYER)
            {
                seeReviewsWindow = new SeeBuyerReviewsView(App.SeeBuyerReviewsViewModel);
                seeReviewsWindow.Activate();
            }
            else if (App.CurrentUser.UserType == SELLER)
            {
                // Create a window that hosts the SeeSellerReviewsView page
                seeReviewsWindow = new Window();
                seeReviewsWindow.Content = new SeeSellerReviewsView(App.SeeSellerReviewsViewModel);
                seeReviewsWindow.Activate();
            }
        }

        private Window basketViewWindow;
        private Window auctionProductListViewWindow;
        private Window borrowProductListViewWindow;
        private Window buyProductListViewWindow;
        private Window adminViewWindow;
        private Window leaveReviewViewWindow;
        public Window CreateListingViewWindow { get; set; }
        private Window seeReviewsWindow;
    }
}
