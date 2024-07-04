using Restaurant_Manager.Models;
using Restaurant_Manager.DAL;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            var complaints = _context.Complaints.Where(c => c.User.Id == _currentUser.Id).ToList();
            lstReviews.ItemsSource = complaints;
        }

        private void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            _currentUser.Email = txtEmail.Text;
            if (long.TryParse(txtZipcode.Text, out long zipcode))
            {
                _currentUser.Zipcode = zipcode;
            }
            else
            {
                _currentUser.Zipcode = null; // or handle as per your logic
            }

            if (cmbGender.IsEnabled && cmbGender.SelectedItem is ComboBoxItem selectedItem)
            {
                _currentUser.Gender = Enum.TryParse(typeof(User.Genders), selectedItem.Content.ToString(), out var gender) ? (User.Genders?)gender : null;
                cmbGender.IsEnabled = false;
            }
            _context.Users.Update(_currentUser);
            _context.SaveChanges();
            MessageBox.Show("Profile updated successfully!");
        }

        private void Gender_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Gender ComboBox should become read-only after an item is selected
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
            }

            if (minScore > 0)
            {
                filteredRestaurants = filteredRestaurants.Where(r => r.AvgRate >= minScore);
            }

            lstRestaurants.ItemsSource = filteredRestaurants.ToList();
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
                int.TryParse(txtOrderRating.Text, out int rating);
                string comment = txtOrderComment.Text;

                var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == selectedOrder.User.Restaurant.Id);

                var newComplaint = new Complaint
                {
                    User = _currentUser,
                    Detail = comment,
                    Title = $"Order {selectedOrder.Id} - Rating",
                    Restaurant = restaurant, // Correctly linked to the restaurant
                    Status = Complaint.CStatus.Pending
                };

                _context.Complaints.Add(newComplaint);

                selectedOrder.Rate = rating;
                _context.Orders.Update(selectedOrder);
                _context.SaveChanges();

                MessageBox.Show("Comment and Rating submitted successfully!");
            }
            else
            {
                MessageBox.Show("Please select an order.");
            }
        }

    }
}
