using Restaurant_Manager.Models;
using Restaurant_Manager.DAL;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_Manager
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
            lstRestaurants.ItemsSource = restaurants;
        }

        private void LoadOrders()
        {
            var orders = _context.Orders.Where(o => o.User.Id == _currentUser.Id).ToList();
            lstOrders.ItemsSource = orders;
        }

        private void LoadComplaints()
        {
            var restaurantComplaints = _context.RestaurantComplaints.Where(c => c.User.Id == _currentUser.Id).ToList();
            var staffComplaints = _context.StuffComplaints.Where(c => c.User.Id == _currentUser.Id).ToList();
            var orderComplaints = _context.OrderComplaints.Where(c => c.User.Id == _currentUser.Id).ToList();

            lstRestaurantComplaints.ItemsSource = restaurantComplaints;
            lstStuffComplaints.ItemsSource = staffComplaints;
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
            string city = txtSearchCity.Text;
            string name = txtSearchName.Text;
            string receptionType = (cmbReceptionType.SelectedItem as ComboBoxItem)?.Content.ToString();
            double.TryParse(txtMinScore.Text, out double minScore);

            var filteredRestaurants = _context.Restaurants.AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
            {
                filteredRestaurants = filteredRestaurants.Where(r => r.City.Contains(city));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                filteredRestaurants = filteredRestaurants.Where(r => r.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(receptionType))
            {
                if (receptionType == "Delivery")
                {
                    filteredRestaurants = filteredRestaurants.Where(r => r.Delivery);
                }
                else if (receptionType == "Dine-In")
                {
                    filteredRestaurants = filteredRestaurants.Where(r => r.DineIn);
                }
                else if (receptionType == "Both")
                {
                    filteredRestaurants = filteredRestaurants.Where(r => r.Delivery || r.DineIn);
                }
            }

            if (minScore > 0)
            {
                filteredRestaurants = filteredRestaurants.Where(r => r.AvgRate >= minScore);
            }

            lstRestaurants.ItemsSource = filteredRestaurants.ToList();
        }

        private void LstRestaurants_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstRestaurants.SelectedItem is Restaurant selectedRestaurant)
            {
                var restaurantPage = new ResturantWindow(selectedRestaurant, _currentUser);
                restaurantPage.Show();
            }
        }

        private void LstOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstOrders.SelectedItem is Order selectedOrder)
            {
                // Optionally, you can display more details of the selected order
            }
        }

        private void SubmitOrderComment_Click(object sender, RoutedEventArgs e)
        {
            if (lstOrders.SelectedItem is Order selectedOrder)
            {
                if (int.TryParse(txtOrderRating.Text, out int rating) && rating >= 1 && rating <= 5)
                {
                    string comment = txtOrderComment.Text;

                    var orderStuffs = _context.Order_Stuffs.Where(os => os.order.Id == selectedOrder.Id).ToList();
                    if (orderStuffs.Any())
                    {
                        var restaurant = orderStuffs.First().stuff.Resturant;

                        var newOrderComplaint = new OrderComplaint
                        {
                            User = _currentUser,
                            Detail = comment,
                            Title = $"Order {selectedOrder.Id} - Rating",
                            //Status = RestaurantComplaint.CStatus.Pending
                        };

                        _context.OrderComplaints.Add(newOrderComplaint);

                        selectedOrder.Rate = rating;
                        _context.Orders.Update(selectedOrder);
                        _context.SaveChanges();

                        MessageBox.Show("Comment and Rating submitted successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No items found for the selected order.");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid rating between 1 and 5.");
                }
            }
            else
            {
                MessageBox.Show("Please select an order.");
            }
        }
    }
}
