using System.Linq;
using System.Windows;
using Restaurant_Manager.Models;
using Restaurant_Manager.DAL;

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
            LoadReviews();
        }

        private void LoadUserProfile()
        {
            txtUsername.Text = _currentUser.Username;
            txtName.Text = _currentUser.Name;
            txtEmail.Text = _currentUser.Email;
            txtPhone.Text = _currentUser.Phone;
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

        private void LoadReviews()
        {
            var reviews = _context.Complaints.Where(r => r.User.Id == _currentUser.Id).ToList();
            lstReviews.ItemsSource = reviews;
        }

        private void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            _currentUser.Name = txtName.Text;
            _currentUser.Email = txtEmail.Text;
            _currentUser.Phone = txtPhone.Text;
            _context.Users.Update(_currentUser);
            _context.SaveChanges();
            MessageBox.Show("Profile updated successfully!");
        }
    }
}
