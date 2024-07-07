using Restaurant_Manager.Models;
using System.Windows;
using System.Linq;
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
            using (var context = new RestaurantContext())
            {
                MenuListView.ItemsSource = context.Stuffs
                    .Where(s => s.Resturant.Id == _restaurant.Id)
                    .ToList();
            }
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
            using (var context = new RestaurantContext())
            {
                CommentListView.ItemsSource = context.Comments
                    .Where(c => c.Stuff.Id == stuff.Id)
                    .ToList();
            }
        }
    }
}
