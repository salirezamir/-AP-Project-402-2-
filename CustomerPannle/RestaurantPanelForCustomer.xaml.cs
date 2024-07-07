using Restaurant_Manager.Models;
using System.Windows;
using Restaurant_Manager.DAL;
using System;


namespace Restaurant_Manager.CustomerPannle
{
    public partial class RestaurantPanelForCustomer : Window
    {
        private Restaurant _restaurant;

        public RestaurantPanelForCustomer(Restaurant restaurant)
        {
            InitializeComponent();
            _restaurant = restaurant;
            LoadMenu();
        }

        private void LoadMenu()
        {
            MenuListView.ItemsSource = _restaurant.Menu;
        }

        private void MenuListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MenuListView.SelectedItem is Stuff selectedStuff)
            {
                LoadComments(selectedStuff);
            }
        }

        private void LoadComments(Stuff stuff)
        {
            CommentListView.ItemsSource = stuff.Comments.ToList();
        }
    }
}