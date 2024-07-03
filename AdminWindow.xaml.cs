using Restaurant_Manager.DAL;
using Restaurant_Manager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Restaurant_Manager
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    class RestDGVM
    {
        public string Name { get; set; }
        public string City { get; set; }
        public int Rate { get; set; }
        public bool Complaint { get; set; }
    }
    class CompDGVM
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string RName { get; set; }
        public string Status { get; set; }
    }
    public class AdminWindowViewModel : INotifyPropertyChanged
    {
        private string _router;
        public string Router
        {
            get => _router;
            set
            {
                _router = value;
                OnPropertyChange(nameof(_router));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class AdminWindow : Window
    {
        User _user;
        private readonly RestaurantContext _context = new RestaurantContext();
        public AdminWindow(User user)
        {
            _user = user;
            DataContext = new AdminWindowViewModel();
            InitializeComponent();
            well.Text = "Welcome : " + user.Name;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            
        }
        bool reapeat = false;
        private void SinRsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameRsTx.Text) || string.IsNullOrWhiteSpace(NameRsTx.Text)
                || string.IsNullOrWhiteSpace(CityRsTx.Text) || string.IsNullOrWhiteSpace(EmailRsTx.Text)
                || string.IsNullOrWhiteSpace(PhoneRsTx.Text) || string.IsNullOrWhiteSpace(AddrRsTx.Text)
                || DineRsCb.SelectedIndex < 0 || DeliveryRsCb.SelectedIndex < 0)
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }
            if (Validation.GetErrors(UsernameRsTx).Count > 0 || Validation.GetErrors(NameRsTx).Count > 0
                || Validation.GetErrors(CityRsTx).Count > 0 || Validation.GetErrors(EmailRsTx).Count > 0
                || Validation.GetErrors(PhoneRsTx).Count > 0)
            {
                MessageBox.Show("Please fix all fields");
                return;
            }
            Restaurant restaurant = new Restaurant
            {
                Name = NameRsTx.Text,
                City = CityRsTx.Text,
                Address = AddrRsTx.Text,
                DineIn = DineRsCb.SelectedIndex == 0 ? true : false,
                Delivery = DeliveryRsCb.SelectedIndex == 0 ? true : false,
                rate = 0,
                Reservation = false
            };
            _context.Restaurants.Add(restaurant);
            Random random = new Random();
            int pass = random.Next(10000000, 99999999);
            User user = new User
            {
                Username = UsernameRsTx.Text,
                Name = "Resturant " + NameRsTx.Text,
                Password = MainWindow.CreateMD5(pass.ToString()),
                Email = EmailRsTx.Text,
                Phone = PhoneRsTx.Text,
                Type = User.Types.Restaurant,
                Restaurant = restaurant
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            MessageBox.Show($"Restaurant Created\nUsername: {UsernameRsTx.Text}\nPassword: {pass}");

        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (reapeat)
                {
                    reapeat = false;
                    Tb.SelectedIndex = 3;
                    return;
                }
                if (Tb.SelectedIndex == 1)
                {
                    FilterRsTx.Clear();
                    FilterRsCb.SelectedIndex = -1;
                    ResturantDg.ItemsSource = _context.Restaurants
                        .GroupJoin(_context.Complaints, r => r, c => c.Restaurant, (r, c)
                        => new RestDGVM
                        {
                            City = r.City,
                            Rate = r.rate,
                            Name = r.Name,
                            Complaint = c.Where(x => x.Status == Complaint.CStatus.Pending).Any()
                        })
                        .ToList();
                    compAnId.Text = "-1";
                }
                else if (Tb.SelectedIndex == 2)
                {
                    FilterCpTx.Clear();
                    FilterCpCb.SelectedIndex = -1;
                    ComplaintDg.ItemsSource = _context.Complaints.Select(x => new CompDGVM
                    {
                        Id = x.Id,
                        Name = x.User.Name,
                        Username = x.User.Username,
                        Title = x.Title,
                        RName = x.Restaurant.Name,
                        Status = x.Status.ToString()
                    })
                    .ToList();
                    compAnId.Text = "-1";
                    UserNameCpAnTx.Clear();
                }
                else if (Tb.SelectedIndex == 3)
                {
                    if (compAnId.Text == "-1")
                    {
                        ComAnHint.Visibility = Visibility.Visible;
                        UserNameCpAnTx.Clear();
                        NameCpAnTx.Clear();
                        ResNameCpAnTx.Clear();
                        DetailAnTx.Clear();
                        FilterCpAnCb.SelectedIndex = -1;
                        AnswerAnTx.Clear();
                    }
                    else if (int.TryParse(compAnId.Text, out int Id) && string.IsNullOrEmpty(UserNameCpAnTx.Text))
                    {
                        ComAnHint.Visibility = Visibility.Hidden;
                        var complaint = _context.Complaints.Where(x => x.Id == Id).Select(x => new
                        {
                            x.User.Username,
                            x.User.Name,
                            RName = x.Restaurant.Name,
                            x.Detail,
                            x.Answer,
                            x.Status
                        }).FirstOrDefault();
                        UserNameCpAnTx.Text = complaint.Username;
                        NameCpAnTx.Text = complaint.Name;
                        ResNameCpAnTx.Text = complaint.RName;
                        DetailAnTx.Text = complaint.Detail;
                        AnswerAnTx.Text = complaint.Answer;
                        FilterCpAnCb.SelectedIndex = (int)complaint.Status;
                        reapeat = true;
                    }
                }
                else if (Tb.SelectedIndex == 0 || Tb.SelectedIndex == 4)
                {
                    compAnId.Text = "-1";
                }
            }
        }

        private void ComplaintDg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ComplaintDg.SelectedIndex == -1 || ComplaintDg.SelectedIndex == ComplaintDg.Items.Count)
                return;
            compAnId.Text = (ComplaintDg.SelectedItem as CompDGVM).Id.ToString();
            Tb.SelectedIndex = 3;
        }

        private void FilterRsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FilterRsTx.Text) || FilterRsCb.SelectedIndex < 0)
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }
            else if (FilterRsCb.SelectedIndex == 0)
            {
                ResturantDg.ItemsSource = _context.Restaurants
                    .Where(x => x.Name.Contains(FilterRsTx.Text))
                    .GroupJoin(_context.Complaints, r => r, c => c.Restaurant, (r, c)
                    => new RestDGVM
                    {
                        City = r.City,
                        Rate = r.rate,
                        Name = r.Name,
                        Complaint = c.Where(x => x.Status == Complaint.CStatus.Pending).Any()
                    })
                    .ToList();
            }
            else if (FilterRsCb.SelectedIndex == 1)
            {
                ResturantDg.ItemsSource = _context.Restaurants
                    .Where(x => x.City.Contains(FilterRsTx.Text))
                    .GroupJoin(_context.Complaints, r => r, c => c.Restaurant, (r, c)
                    => new RestDGVM
                    {
                        City = r.City,
                        Rate = r.rate,
                        Name = r.Name,
                        Complaint = c.Where(x => x.Status == Complaint.CStatus.Pending).Any()
                    })
                    .ToList();
            }
            else if (FilterRsCb.SelectedIndex == 2)
            {
                ResturantDg.ItemsSource = _context.Restaurants
                    .Where(x => x.rate.ToString().Contains(FilterRsTx.Text))
                    .GroupJoin(_context.Complaints, r => r, c => c.Restaurant, (r, c)
                    => new RestDGVM
                    {
                        City = r.City,
                        Rate = r.rate,
                        Name = r.Name,
                        Complaint = c.Where(x => x.Status == Complaint.CStatus.Pending).Any()
                    })
                    .ToList();
            }
            else if (FilterRsCb.SelectedIndex == 3)
            {
                Complaint.CStatus status;
                if (FilterRsTx.Text.ToLower() == "answered")
                    status = Complaint.CStatus.Answered;
                else if (FilterRsTx.Text.ToLower() == "pending")
                    status = Complaint.CStatus.Pending;
                else
                {
                    MessageBox.Show("Invalid Status");
                    return;
                }
                ResturantDg.ItemsSource = _context.Restaurants
                    .GroupJoin(_context.Complaints, r => r, c => c.Restaurant, (r, c)
                    => new RestDGVM
                    {
                        City = r.City,
                        Rate = r.rate,
                        Name = r.Name,
                        Complaint = c.Where(x => x.Status == status).Any()
                    })
                    .Where(x => x.Complaint)
                    .ToList();
            }
        }

        private void FilterCpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FilterCpTx.Text) || FilterCpCb.SelectedIndex < 0)
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }
            else if (FilterCpCb.SelectedIndex == 0)
            {
                
                ComplaintDg.ItemsSource = _context.Complaints
                    .Where(x => x.User.Username.Contains(FilterCpTx.Text))
                    .Select(x => new CompDGVM
                    {
                        Id = x.Id,
                        Name = x.User.Name,
                        Username = x.User.Username,
                        Title = x.Title,
                        RName = x.Restaurant.Name,
                        Status = x.Status.ToString()
                    })
                    .ToList();
            }
            else if (FilterCpCb.SelectedIndex == 1)
            {
                ComplaintDg.ItemsSource = _context.Complaints
                    .Where(x => x.User.Name.Contains(FilterCpTx.Text))
                    .Select(x => new CompDGVM
                    {
                        Id = x.Id,
                        Name = x.User.Name,
                        Username = x.User.Username,
                        Title = x.Title,
                        RName = x.Restaurant.Name,
                        Status = x.Status.ToString()
                    })
                    .ToList();
            }
            else if (FilterCpCb.SelectedIndex == 2)
            {
                ComplaintDg.ItemsSource = _context.Complaints
                    .Where(x => x.Title.Contains(FilterCpTx.Text))
                    .Select(x => new CompDGVM
                    {
                        Id = x.Id,
                        Name = x.User.Name,
                        Username = x.User.Username,
                        Title = x.Title,
                        RName = x.Restaurant.Name,
                        Status = x.Status.ToString()
                    })
                    .ToList();
            }
            else if (FilterCpCb.SelectedIndex == 3)
            {
                ComplaintDg.ItemsSource = _context.Complaints
                    .Where(x => x.Restaurant.Name.Contains(FilterCpTx.Text))
                    .Select(x => new CompDGVM
                    {
                        Id = x.Id,
                        Name = x.User.Name,
                        Username = x.User.Username,
                        Title = x.Title,
                        RName = x.Restaurant.Name,
                        Status = x.Status.ToString()
                    })
                    .ToList();
            }
            else if (FilterCpCb.SelectedIndex == 4)
            {
                Complaint.CStatus status;
                if (FilterCpTx.Text.ToLower() == "answered")
                    status = Complaint.CStatus.Answered;
                else if (FilterCpTx.Text.ToLower() == "pending")
                    status = Complaint.CStatus.Pending;
                else
                {
                    MessageBox.Show("Invalid Status");
                    return;
                }
                ComplaintDg.ItemsSource = _context.Complaints
                    .Where(x => x.Status == status)
                    .Select(x => new CompDGVM
                    {
                        Id = x.Id,
                        Name = x.User.Name,
                        Username = x.User.Username,
                        Title = x.Title,
                        RName = x.Restaurant.Name,
                        Status = x.Status.ToString()
                    })
                    .ToList();
            }

        }

        private void AnswerCpAnBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FilterCpAnCb.SelectedIndex == -1 || string.IsNullOrEmpty(AnswerAnTx.Text))
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }
            if (int.TryParse(compAnId.Text, out int Id))
            {
                var complaint = _context.Complaints.Where(x => x.Id == Id).FirstOrDefault();
                complaint.Answer = AnswerAnTx.Text;
                complaint.Status = (Complaint.CStatus)FilterCpAnCb.SelectedIndex;
                _context.SaveChanges();
            }
            UserNameCpAnTx.Clear();
            NameCpAnTx.Clear();
            ResNameCpAnTx.Clear();
            DetailAnTx.Clear();
            FilterCpAnCb.SelectedIndex = -1;
            AnswerAnTx.Clear();
            compAnId.Text = "-1";
            MessageBox.Show("Complaint Answered");
        }
    }
}
