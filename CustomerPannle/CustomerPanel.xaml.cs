using Restaurant_Manager.Models;
using Restaurant_Manager.DAL;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using Restaurant_Manager.CustomerPanel;
using System.Xml.Linq;

namespace Restaurant_Manager.CustomerPannle
{
    public partial class CustomerPanel : Window
    {
        private readonly RestaurantContext _context = new RestaurantContext();
        private User _currentUser;

        public CustomerPanel(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadUserProfile();
            LoadRestaurants();
            LoadOrders();
            LoadComplaints();
        }

        private void LoadUserProfile()
        {
            txtUsername.Text = _currentUser.Username;
            txtName.Text = _currentUser.Name;
            txtEmail.Text = _currentUser.Email;
            txtPhone.Text = _currentUser.Phone;
            txtZipcode.Text = _currentUser.Zipcode?.ToString() ?? string.Empty;

            if (_currentUser.Gender.HasValue)
            {
                cmbGender.SelectedItem = cmbGender.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == _currentUser.Gender.ToString());
                cmbGender.IsEnabled = false;
            }
        }

        private void LoadRestaurants()
        {
            var restaurants = _context.Restaurants.ToList();
            RestaurantListView.ItemsSource = restaurants;
        }

        private void LoadOrders()
        {
            var orders = _context.Orders.Where(o => o.User.Id == _currentUser.Id).ToList();
            lstOrders.ItemsSource = orders;
        }

        private void LoadComplaints()
        {
            var restaurantComplaints = _context.RestaurantComplaints.Where(c => c.User.Id == _currentUser.Id).ToList();
            var stuffComplaints = _context.StuffComplaints.Where(c => c.User.Id == _currentUser.Id).ToList();
            var orderComplaints = _context.OrderComplaints.Where(c => c.Order.User.Id == _currentUser.Id).ToList();

            lstRestaurantComplaints.ItemsSource = restaurantComplaints;
            lstStuffComplaints.ItemsSource = stuffComplaints;
            lstOrderComplaints.ItemsSource = orderComplaints;
        }

        private void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            if (new EmailAddressAttribute().IsValid(txtEmail.Text))
            {
                var emailValidationResult = ((TextBox)FindName("txtEmail")).GetBindingExpression(TextBox.TextProperty).ValidateWithoutUpdate();
                var zipValidationResult = ((TextBox)FindName("txtZipcode")).GetBindingExpression(TextBox.TextProperty).ValidateWithoutUpdate();

                if (!emailValidationResult || !zipValidationResult)
                {
                    MessageBox.Show("Please correct the validation errors.");
                    return;
                }

                _currentUser.Email = txtEmail.Text;

                if (long.TryParse(txtZipcode.Text, out long zipcode))
                {
                    _currentUser.Zipcode = zipcode;
                }
                else
                {
                    _currentUser.Zipcode = null;
                }

                if (cmbGender.IsEnabled && cmbGender.SelectedItem is ComboBoxItem selectedItem)
                {
                    _currentUser.Gender = Enum.TryParse(typeof(User.Genders), selectedItem.Content.ToString(), out var gender) ? (User.Genders?)gender : null;
                    cmbGender.IsEnabled = false;
                }

                try
                {
                    _context.Users.Update(_currentUser);
                    _context.SaveChanges();
                    MessageBox.Show("Profile updated successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating profile: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid email address.");
            }
        }

        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGender.SelectedItem != null)
            {
                cmbGender.IsEnabled = false;
            }
        }

        private void SearchRestaurants_Click(object sender, RoutedEventArgs e)
        {
            var city = txtSearchCity.Text;
            var name = txtSearchName.Text;
            var receptionType = (cmbReceptionType.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool isValidScore = double.TryParse(txtMinScore.Text, out double minScore);

            var filteredRestaurants = _context.Restaurants.AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                filteredRestaurants = filteredRestaurants.Where(r => r.City.Contains(city));
            }
            if (!string.IsNullOrEmpty(name))
            {
                filteredRestaurants = filteredRestaurants.Where(r => r.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(receptionType))
            {
                if (receptionType == "Both")
                {
                    filteredRestaurants = filteredRestaurants.Where(r => r.Delivery && r.DineIn);
                }
                else if (receptionType == "Delivery")
                {
                    filteredRestaurants = filteredRestaurants.Where(r => r.Delivery);
                }
                else if (receptionType == "Dine-In")
                {
                    filteredRestaurants = filteredRestaurants.Where(r => r.DineIn);
                }
            }
            if (isValidScore)
            {
                filteredRestaurants = filteredRestaurants.Where(r => r.AvgRate >= minScore);
            }

            RestaurantListView.ItemsSource = filteredRestaurants.ToList();
        }

        private void RestaurantListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (RestaurantListView.SelectedItem is Restaurant selectedRestaurant)
            {
                var restaurantDetail = new RestaurantPanelForCustomer(selectedRestaurant);
                restaurantDetail.Show();
                this.Close();
            }
        }

        private void LstOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstOrders.SelectedItem is Order selectedOrder)
            {
                txtOrderRating.Text = selectedOrder.Rate.ToString() ?? string.Empty;
            }
        }

        private void SubmitOrderRating_Click(object sender, RoutedEventArgs e)
        {
            if (lstOrders.SelectedItem is Order selectedOrder)
            {
                if (int.TryParse(txtOrderRating.Text, out int rating) && rating >= 1 && rating <= 5)
                {
                    selectedOrder.Rate = rating;

                    try
                    {
                        _context.Orders.Update(selectedOrder);
                        _context.SaveChanges();
                        MessageBox.Show("Rating submitted successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error submitting rating: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid rating between 1 and 5.");
                }
            }
        }
    }
}
