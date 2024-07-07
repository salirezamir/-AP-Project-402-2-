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
        public double Rate { get; set; }
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
        public string For { get; set; }
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
        int st = -1;
        string rt = "";
        User _user;
        List<CompDGVM> CompList;
        bool reapeat = false;
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
                AvgRate = 0,
                Reservation = false
            };
            _context.Restaurants.Add(restaurant);
            Random random = new Random();
            int pass = random.Next(10000000, 99999999);
            User user = new User
            {
                Username = UsernameRsTx.Text,
                Name = NameRsTx.Text,
                Password = MainWindow.CreateMD5(pass.ToString()),
                Email = EmailRsTx.Text,
                Phone = PhoneRsTx.Text,
                Type = User.Types.Restaurant,
                Restaurant = restaurant,
                Address = AddrRsTx.Text,
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
                        .GroupJoin(_context.RestaurantComplaints, r => r, c => c.Restaurant, (r, c)
                        => new RestDGVM
                        {
                            City = r.City,
                            Rate = r.AvgRate,
                            Name = r.Name,
                            Complaint = c.Where(x => x.Status == RestaurantComplaint.CStatus.Pending).Any()
                        })
                        .ToList();
                    
                    st = -1;
                    
                }
                else if (Tb.SelectedIndex == 2)
                {
                    FilterCpTx.Clear();
                    FilterCpCb.SelectedIndex = -1;
                    //ComplaintDg.ItemsSource
                    CompList = _context.RestaurantComplaints.Select(x => new CompDGVM
                    {
                        Id = x.Id,
                        Name = x.User.Name,
                        Username = x.User.Username,
                        Title = x.Title,
                        RName = x.Restaurant.Name,
                        Status = x.Status.ToString(),
                        For = "Restaurant"
                    })
                    .ToList();
                    CompList = _context.StuffComplaints.Select(x => new CompDGVM
                    {
                        Id = x.Id,
                        Name = x.User.Name,
                        Username = x.User.Username,
                        Title = x.Title,
                        RName = x.Stuff.Name,
                        Status = x.Status.ToString(),
                        For = "Stuffs"
                    }).ToList().Concat(CompList).ToList();
                    CompList = _context.OrderComplaints.Select(x => new CompDGVM
                    {
                        Id = x.Id,
                        Name = x.Order.User.Name,
                        Username = x.Order.User.Username,
                        Title = x.Title,
                        RName = x.Order.Id.ToString(),
                        Status = x.Status.ToString(),
                        For = "Order"
                    }).ToList().Concat(CompList).ToList();
                    st = -1;
                    ComplaintDg.ItemsSource = CompList;
                    UserNameCpAnTx.Clear();
                }
                else if (Tb.SelectedIndex == 3)
                {
                    if (st == -1)
                    {
                        ComAnHint.Visibility = Visibility.Visible;
                        UserNameCpAnTx.Clear();
                        NameCpAnTx.Clear();
                        ResNameCpAnTx.Clear();
                        DetailAnTx.Clear();
                        FilterCpAnCb.SelectedIndex = -1;
                        AnswerAnTx.Clear();
                    }
                    else if (string.IsNullOrEmpty(UserNameCpAnTx.Text))
                    {
                        ComAnHint.Visibility = Visibility.Hidden;
                        if (rt == "Restaurant")
                        {
                            ComAnHint.Visibility = Visibility.Hidden;
                            var complaint = _context.RestaurantComplaints.Where(x => x.Id == st).Select(x => new
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
                        }
                        else if (rt == "Stuffs")
                        {
                            ComAnHint.Visibility = Visibility.Hidden;
                            var complaint = _context.StuffComplaints.Where(x => x.Id == st).Select(x => new
                            {
                                x.User.Username,
                                x.User.Name,
                                SName = x.Stuff.Name,
                                RName = x.Stuff.Resturant.Name,
                                x.Detail,
                                x.Answer,
                                x.Status
                            }).FirstOrDefault();
                            UserNameCpAnTx.Text = complaint.Username;
                            NameCpAnTx.Text = complaint.Name;
                            ResNameCpAnTx.Text = complaint.RName;
                            DetailAnTx.Text = $"Stuff : {complaint.SName}\n" + complaint.Detail;
                            AnswerAnTx.Text = complaint.Answer;
                            FilterCpAnCb.SelectedIndex = (int)complaint.Status;
                        }
                        else if (rt == "Order")
                        {
                            ComAnHint.Visibility = Visibility.Hidden;
                            var complaint = _context.OrderComplaints.Where(x => x.Id == st).Select(x => new
                            {
                                x.Order.User.Username,
                                x.Order.User.Name,
                                x.Order.Id,
                                x.Order.OrderDate,
                                x.Detail,
                                x.Answer,
                                x.Status
                            }).FirstOrDefault();
                            var stuffs = _context.Order_Stuffs.Where(Order_Stuffs => Order_Stuffs.order.Id == complaint.Id)
                                .Select(Order_Stuffs => new
                            {
                                Order_Stuffs.stuff.Name,
                                Order_Stuffs.Quantity
                            }).ToList();
                            UserNameCpAnTx.Text = complaint.Username;
                            NameCpAnTx.Text = complaint.Name;
                            ResNameCpAnTx.Text = complaint.OrderDate.ToString();
                            //Change Label
                            DetailAnTx.Text = "StufList:Name,Quantity";
                            foreach (var stuff in stuffs)
                                DetailAnTx.Text += $"{stuff.Name},{stuff.Quantity}\n";
                            DetailAnTx.Text += "Details:\n";
                            DetailAnTx.Text += complaint.Detail;
                            AnswerAnTx.Text = complaint.Answer;
                            FilterCpAnCb.SelectedIndex = (int)complaint.Status;
                        }
                        reapeat = true;
                    }
                }
                else if (Tb.SelectedIndex == 0 || Tb.SelectedIndex == 4)
                {
                    st = -1;
                }
            }
        }

        private void ComplaintDg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ComplaintDg.SelectedIndex == -1 || ComplaintDg.SelectedIndex == ComplaintDg.Items.Count)
                return;
            st = (ComplaintDg.SelectedItem as CompDGVM).Id;
            rt = (ComplaintDg.SelectedItem as CompDGVM).For;
            
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
                    .GroupJoin(_context.RestaurantComplaints, r => r, c => c.Restaurant, (r, c)
                    => new RestDGVM
                    {
                        City = r.City,
                        Rate = r.AvgRate,
                        Name = r.Name,
                        Complaint = c.Where(x => x.Status == RestaurantComplaint.CStatus.Pending).Any()
                    })
                    .ToList();
            }
            else if (FilterRsCb.SelectedIndex == 1)
            {
                ResturantDg.ItemsSource = _context.Restaurants
                    .Where(x => x.City.Contains(FilterRsTx.Text))
                    .GroupJoin(_context.RestaurantComplaints, r => r, c => c.Restaurant, (r, c)
                    => new RestDGVM
                    {
                        City = r.City,
                        Rate = r.AvgRate,
                        Name = r.Name,
                        Complaint = c.Where(x => x.Status == RestaurantComplaint.CStatus.Pending).Any()
                    })
                    .ToList();
            }
            else if (FilterRsCb.SelectedIndex == 2)
            {
                ResturantDg.ItemsSource = _context.Restaurants
                    .Where(x => x.AvgRate.ToString().Contains(FilterRsTx.Text))
                    .GroupJoin(_context.RestaurantComplaints, r => r, c => c.Restaurant, (r, c)
                    => new RestDGVM
                    {
                        City = r.City,
                        Rate = r.AvgRate,
                        Name = r.Name,
                        Complaint = c.Where(x => x.Status == RestaurantComplaint.CStatus.Pending).Any()
                    })
                    .ToList();
            }
            else if (FilterRsCb.SelectedIndex == 3)
            {
                RestaurantComplaint.CStatus status;
                if (FilterRsTx.Text.ToLower() == "answered")
                    status = RestaurantComplaint.CStatus.Answered;
                else if (FilterRsTx.Text.ToLower() == "pending")
                    status = RestaurantComplaint.CStatus.Pending;
                else
                {
                    MessageBox.Show("Invalid Status");
                    return;
                }
                ResturantDg.ItemsSource = _context.Restaurants
                    .GroupJoin(_context.RestaurantComplaints, r => r, c => c.Restaurant, (r, c)
                    => new RestDGVM
                    {
                        City = r.City,
                        Rate = r.AvgRate,
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
                ComplaintDg.ItemsSource = CompList
                    .Where(x => x.Username.Contains(FilterCpTx.Text))
                    .ToList();

            }
            else if (FilterCpCb.SelectedIndex == 1)
            {
                ComplaintDg.ItemsSource = CompList
                    .Where(x => x.Name.Contains(FilterCpTx.Text))
                    .ToList();
            }
            else if (FilterCpCb.SelectedIndex == 2)
            {
                ComplaintDg.ItemsSource = CompList
                    .Where(x => x.Title.Contains(FilterCpTx.Text))
                    .ToList();
            }
            else if (FilterCpCb.SelectedIndex == 3)
            {
                ComplaintDg.ItemsSource = CompList
                    .Where(x => x.RName.Contains(FilterCpTx.Text))
                    .ToList();
            }
            else if (FilterCpCb.SelectedIndex == 4)
            {
                RestaurantComplaint.CStatus status;
                if (FilterCpTx.Text.ToLower() == "answered")
                    status = RestaurantComplaint.CStatus.Answered;
                else if (FilterCpTx.Text.ToLower() == "pending")
                    status = RestaurantComplaint.CStatus.Pending;
                else
                {
                    MessageBox.Show("Invalid Status");
                    return;
                }
                ComplaintDg.ItemsSource = CompList
                    .Where(x => x.Status.Contains(status.ToString()))
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
            if (rt == "Restaurant")
            {
                var complaint = _context.RestaurantComplaints.Where(x => x.Id == st).FirstOrDefault();
                complaint.Status = (RestaurantComplaint.CStatus)FilterCpAnCb.SelectedIndex;
                complaint.Answer = AnswerAnTx.Text;
            }
            else if (rt == "Stuffs")
            {
                var complaint = _context.StuffComplaints.Where(x => x.Id == st).FirstOrDefault();
                complaint.Status = (StuffComplaint.CStatus)FilterCpAnCb.SelectedIndex;
                complaint.Answer = AnswerAnTx.Text;
            }
            else if (rt == "Order")
            {
                var complaint = _context.OrderComplaints.Where(x => x.Id == st).FirstOrDefault();
                complaint.Status = (OrderComplaint.CStatus)FilterCpAnCb.SelectedIndex;
                complaint.Answer = AnswerAnTx.Text;
            }
            _context.SaveChanges();
            UserNameCpAnTx.Clear();
            NameCpAnTx.Clear();
            ResNameCpAnTx.Clear();
            DetailAnTx.Clear();
            FilterCpAnCb.SelectedIndex = -1;
            AnswerAnTx.Clear();
            st = -1;
            rt = "";
            
            MessageBox.Show("Complaint Answered");
        }
    }
}
