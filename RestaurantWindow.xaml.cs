using Microsoft.Identity.Client.NativeInterop;
using Microsoft.Win32;
using Restaurant_Manager.DAL;
using Restaurant_Manager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
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

namespace Restaurant_Manager
{
    /// <summary>
    /// Interaction logic for RestaurantWindow.xaml
    /// </summary>
    /// public static class ExportData


    public class MenuView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string Type { get; set; }
        public string Rate { get; set; }
        public string Materials { get; set; }
    }
    public class HistView
    {
        public int Id { get; set; }
        public string UName { get; set; }
        public string UserName { get; set; }
        public string OrderDate { get; set; }
        public string TotalAmount { get; set; }
        public string Type { get; set; }
    }
    public class CommentView
    {
        public string Id { get; set; }
        public string Answer { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
    }
    // Stuff View Same As DB
    public class RestVM : INotifyPropertyChanged
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
    public partial class RestaurantWindow : Window
    {
        List<HistView> _histViews;
        User _user;
        Restaurant _restaurant;
        bool repeat = false;
        int id = -1;
        private readonly RestaurantContext _context = new RestaurantContext();

        public static void ExportCsv<T>(List<T> genericList, string finalPath)
        {
            var sb = new StringBuilder();
            var header = "";
            var info = typeof(T).GetProperties();
            if (!File.Exists(finalPath))
            {
                var file = File.Create(finalPath);
                file.Close();
                foreach (var prop in typeof(T).GetProperties())
                {
                    header += prop.Name + "; ";
                }
                header = header.Substring(0, header.Length - 2);
                sb.AppendLine(header);
                TextWriter sw = new StreamWriter(finalPath, true);
                sw.Write(sb.ToString());
                sw.Close();
            }
            foreach (var obj in genericList)
            {
                sb = new StringBuilder();
                var line = "";
                foreach (var prop in info)
                {
                    line += prop.GetValue(obj, null) + "; ";
                }
                line = line.Substring(0, line.Length - 2);
                sb.AppendLine(line);
                TextWriter sw = new StreamWriter(finalPath, true);
                sw.Write(sb.ToString());
                sw.Close();
            }
        }

        public RestaurantWindow(Restaurant restaurant, User user)
        {
            InitializeComponent();
            _restaurant = restaurant;
            _user = user;
            _histViews = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
                .Where(x => x.u.stuff.Resturant == _restaurant)
                .Select(x => new HistView
                {
                    Id = x.o.Id,
                    UserName = x.o.User.Username,
                    OrderDate = x.o.OrderDate.ToString(),
                    TotalAmount = x.o.TotalAmount.ToString(),
                    Type = x.o.OrderType.ToString()
                })
                .GroupBy(x => x.Id)
                .Select(y => y.First())
                .ToList();
            CommentDg.Visibility = Visibility.Hidden;
            DataContext = new RestVM();
            if (_restaurant.AvgRate >= 4.5)
            {
                AcTxb.Visibility = Visibility.Visible;
                ActivateBtn.Visibility = Visibility.Visible;
                StTxb.Visibility = Visibility.Visible;
                StTxb.Text = "Status :";
                if (_restaurant.Reservation)
                {
                    StTxb.Text += "Activated";
                    ActivateBtn.Content = "Deactivate";
                }
                else
                {
                    StTxb.Text += "Deactivated";
                    ActivateBtn.Content = "Activate";
                }
            }
            else
            {
                AcTxb.Visibility = Visibility.Hidden;
                ActivateBtn.Visibility = Visibility.Hidden;
                StTxb.Visibility = Visibility.Hidden;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void Tb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (repeat)
                {
                    repeat = false;
                    Tb.SelectedIndex = 2;
                    return;
                }
                if (Tb.SelectedIndex == 2)
                {
                    if (id == -1)
                    {
                        NameRsTx.Clear();
                        MaterialTx.Clear();
                        QuantityTx.Clear();
                        PriceTx.Clear();
                        RemoveBtn.Visibility = Visibility.Hidden;
                        TypeCb.SelectedIndex = -1;
                        AddOrEditBtn.Content = "Add";
                        AnswerBtn.Visibility = Visibility.Hidden;
                        AnswerTx.Visibility = Visibility.Hidden;
                        CommentDg.Visibility = Visibility.Hidden;
                    }
                    else if (CommentDg.Visibility == Visibility.Hidden)
                    {
                        Stuff stuff = _context.Stuffs.Where(x => x.Id == id).First();
                        NameRsTx.Text = stuff.Name;
                        MaterialTx.Text = stuff.Materials;
                        QuantityTx.Text = stuff.Quantity.ToString();
                        PriceTx.Text = stuff.Price.ToString();
                        TypeCb.SelectedIndex = (int)stuff.fType;
                        RemoveBtn.Visibility = Visibility.Visible;
                        AddOrEditBtn.Content = "Edit";
                        repeat = true;
                        AnswerBtn.Visibility = Visibility.Visible;
                        AnswerBtn.IsEnabled = false;
                        AnswerTx.Visibility = Visibility.Visible;
                        CommentDg.Visibility = Visibility.Visible;
                        CommentDg.ItemsSource = _context.Comments
                            .Where(x => x.Stuff == stuff)
                            .Select(x => new CommentView
                            {
                                Id = x.Id.ToString(),
                                Answer = x.Answer,
                                Name = x.Users.Name,
                                Details = x.Details
                            })
                            .ToList();
                    }
                    return;
                }
                if (Tb.SelectedIndex == 0)
                {
                    if (_restaurant.AvgRate >= 4.5)
                    {
                        AcTxb.Visibility = Visibility.Visible;
                        ActivateBtn.Visibility = Visibility.Visible;
                        StTxb.Visibility = Visibility.Visible;
                        StTxb.Text = "Status :";
                        if (_restaurant.Reservation)
                        {
                            StTxb.Text += "Activated";
                            ActivateBtn.Content = "Deactivate";
                        }
                        else
                        {
                            StTxb.Text += "Deactivated";
                            ActivateBtn.Content = "Activate";
                        }
                    }
                    else
                    {
                        AcTxb.Visibility = Visibility.Hidden;
                        ActivateBtn.Visibility = Visibility.Hidden;
                        StTxb.Visibility = Visibility.Hidden;
                    }
                    passwordNewTx.Clear();
                    passwordNewRTx.Clear();
                    passwordOldTx.Clear();
                }
                else if (Tb.SelectedIndex == 1)
                {
                    MenuDg.ItemsSource = _context.Stuffs.Where(x => x.Resturant == _restaurant && x.Available)
                        .Select(x => new MenuView
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Price = x.Price.ToString(),
                            Quantity = x.Quantity.ToString(),
                            Type = x.fType.ToString(),
                            Rate = x.Rate.ToString(),
                            Materials = x.Materials
                        }).ToList();
                }
                else if (Tb.SelectedIndex == 3)
                {
                    _histViews = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
                        .Where(x => x.u.stuff.Resturant == _restaurant)
                        .Select(x => new HistView
                        {
                            Id = x.o.Id,
                            UserName = x.o.User.Username,
                            OrderDate = x.o.OrderDate.ToString(),
                            TotalAmount = x.o.TotalAmount.ToString(),
                            Type = x.o.OrderType.ToString(),
                            UName = x.o.User.Name
                        })
                        .GroupBy(x => x.Id)
                        .Select(y => y.First())
                        .ToList();
                    HistoryDg.ItemsSource = _histViews;
                    StuffsDg.ItemsSource = null;
                    FilterHiCb.SelectedIndex = -1;
                    FilterHiTx.Clear();
                }
                id = -1;
            }
        }

        private void MenuDg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MenuDg.SelectedIndex == -1 || MenuDg.SelectedIndex == MenuDg.Items.Count)
                return;
            id = (MenuDg.SelectedItem as MenuView).Id;
            Tb.SelectedIndex = 2;
        }

        private void AddOrEditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NameRsTx.Text == "" || MaterialTx.Text == "" || QuantityTx.Text == "" || PriceTx.Text == "" || TypeCb.SelectedIndex == -1)
            {
                MessageBox.Show("Please Fill All Fields");
                return;
            }
            Stuff stuff;
            if (id != -1)
            {
                stuff = _context.Stuffs.Where(x => x.Id == id).First();
                stuff.Name = NameRsTx.Text;
                stuff.Materials = MaterialTx.Text;
                stuff.Quantity = int.Parse(QuantityTx.Text);
                stuff.Price = long.Parse(PriceTx.Text);
                stuff.fType = (Stuff.SType)TypeCb.SelectedIndex;
                _context.SaveChanges();
            }
            else
            {
                stuff = new Stuff();
                stuff.Name = NameRsTx.Text;
                stuff.Materials = MaterialTx.Text;
                stuff.Quantity = int.Parse(QuantityTx.Text);
                stuff.Price = long.Parse(PriceTx.Text);
                stuff.fType = (Stuff.SType)TypeCb.SelectedIndex;
                stuff.Available = true;
                stuff.Resturant = _context.Restaurants.Where(x => x.Id == _restaurant.Id).First();
                stuff.PicFileId = 0;
                stuff.Rate = 0;
                _context.Stuffs.Add(stuff);
                _context.SaveChanges();
            }
            NameRsTx.Clear();
            MaterialTx.Clear();
            QuantityTx.Clear();
            PriceTx.Clear();
            AnswerBtn.Visibility = Visibility.Hidden;
            AnswerTx.Visibility = Visibility.Hidden;
            CommentDg.Visibility = Visibility.Hidden;
            RemoveBtn.Visibility = Visibility.Hidden;
            TypeCb.SelectedIndex = -1;
            AddOrEditBtn.Content = "Add";
            id = -1;
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            Stuff stuff = _context.Stuffs.Where(x => x.Id == id).First();
            stuff.Available = false;
            _context.SaveChanges();
            NameRsTx.Clear();
            MaterialTx.Clear();
            QuantityTx.Clear();
            PriceTx.Clear();
            RemoveBtn.Visibility = Visibility.Hidden;
            TypeCb.SelectedIndex = -1;
            AddOrEditBtn.Content = "Add";
            AnswerBtn.Visibility = Visibility.Hidden;
            AnswerTx.Visibility = Visibility.Hidden;
            CommentDg.Visibility = Visibility.Hidden;
            id = -1;
        }

        private void SaveFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV File|*.csv";
            saveFileDialog1.Title = "Save an CSV File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                try
                {
                    ExportCsv(_histViews, saveFileDialog1.FileName);
                }
                catch (Exception el)
                {
                    MessageBox.Show(el.Message);
                }
            }
        }

        private void HistoryDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == HistoryDg)
            {
                if (HistoryDg.SelectedIndex == -1 || HistoryDg.SelectedIndex == HistoryDg.Items.Count)
                    return;
                StuffsDg.ItemsSource = _context.Order_Stuffs
                    .Where(x => x.order.Id == (HistoryDg.SelectedItem as HistView).Id)
                    .Select(x => new
                    {
                        x.Id,
                        x.Price,
                        x.stuff.Name,
                        x.Quantity,
                        Type = x.stuff.fType.ToString()
                    })
                    .ToList();
            }
        }

        private void FilterRsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FilterHiCb.SelectedIndex == -1 || FilterHiTx.Text == "")
                return;
            StuffsDg.ItemsSource = null;
            if (FilterHiCb.SelectedIndex == 0)
            {
                _histViews = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
                    .Where(x => x.u.stuff.Resturant == _restaurant)
                    .Where(x => x.o.User.Username.Contains(FilterHiTx.Text))
                    .Select(x => new HistView
                    {
                        Id = x.o.Id,
                        UserName = x.o.User.Username,
                        OrderDate = x.o.OrderDate.ToString(),
                        TotalAmount = x.o.TotalAmount.ToString(),
                        Type = x.o.OrderType.ToString(),
                        UName = x.o.User.Name
                    })
                    .GroupBy(x => x.Id)
                    .Select(y => y.First())
                    .ToList();
                HistoryDg.ItemsSource = _histViews;
            }
            else if (FilterHiCb.SelectedIndex == 1)
            {
                _histViews = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
                    .Where(x => x.u.stuff.Resturant == _restaurant)
                    .Where(x => x.o.User.Phone.Contains(FilterHiTx.Text))
                    .Select(x => new HistView
                    {
                        Id = x.o.Id,
                        UserName = x.o.User.Username,
                        OrderDate = x.o.OrderDate.ToString(),
                        TotalAmount = x.o.TotalAmount.ToString(),
                        Type = x.o.OrderType.ToString(),
                        UName = x.o.User.Name
                    })
                    .GroupBy(x => x.Id)
                    .Select(y => y.First())
                    .ToList();
                HistoryDg.ItemsSource = _histViews;
            }
            else if (FilterHiCb.SelectedIndex == 2)
            {
                _histViews = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
                    .Where(x => x.u.stuff.Resturant == _restaurant && x.u.stuff.Name.Contains(FilterHiTx.Text))
                    .Select(x => new HistView
                    {
                        Id = x.o.Id,
                        UserName = x.o.User.Username,
                        OrderDate = x.o.OrderDate.ToString(),
                        TotalAmount = x.o.TotalAmount.ToString(),
                        Type = x.o.OrderType.ToString(),
                        UName = x.o.User.Name
                    })
                    .GroupBy(x => x.Id)
                    .Select(y => y.First())
                    .ToList();
                HistoryDg.ItemsSource = _histViews;
            }
            else if (FilterHiCb.SelectedIndex == 3)
            {
                if (long.TryParse(FilterHiTx.Text, out long amount))
                {
                    _histViews = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
                        .Where(x => x.u.stuff.Resturant == _restaurant)
                        .Where(x => x.o.TotalAmount >= amount)
                        .Select(x => new HistView
                        {
                            Id = x.o.Id,
                            UserName = x.o.User.Username,
                            OrderDate = x.o.OrderDate.ToString(),
                            TotalAmount = x.o.TotalAmount.ToString(),
                            Type = x.o.OrderType.ToString(),
                            UName = x.o.User.Name
                        })
                        .GroupBy(x => x.Id)
                        .Select(y => y.First())
                        .ToList();
                    HistoryDg.ItemsSource = _histViews;
                }
            }
            else if (FilterHiCb.SelectedIndex == 4)
            {
                if (long.TryParse(FilterHiTx.Text, out long amount))
                {
                    _histViews = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
                        .Where(x => x.u.stuff.Resturant == _restaurant)
                        .Where(x => x.o.TotalAmount <= amount)
                        .Select(x => new HistView
                        {
                            Id = x.o.Id,
                            UserName = x.o.User.Username,
                            OrderDate = x.o.OrderDate.ToString(),
                            TotalAmount = x.o.TotalAmount.ToString(),
                            Type = x.o.OrderType.ToString(),
                            UName = x.o.User.Name
                        })
                        .GroupBy(x => x.Id)
                        .Select(y => y.First())
                        .ToList();
                    HistoryDg.ItemsSource = _histViews;
                }
            }
            else if (FilterHiCb.SelectedIndex == 5)
            {
                _histViews = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
                    .Where(x => x.u.stuff.Resturant == _restaurant)
                    .Select(x => new HistView
                    {
                        Id = x.o.Id,
                        UserName = x.o.User.Username,
                        OrderDate = x.o.OrderDate.ToString(),
                        TotalAmount = x.o.TotalAmount.ToString(),
                        Type = x.o.OrderType.ToString(),
                        UName = x.o.User.Name
                    })
                    .GroupBy(x => x.Id)
                    .Select(y => y.First())
                    .Where(x => x.Type.Contains(FilterHiTx.Text))
                    .ToList();
                HistoryDg.ItemsSource = _histViews;
            }
            else if (FilterHiCb.SelectedIndex == 6)
            {
                try
                {
                    var s = FilterHiTx.Text.Split("|");
                    DateTime d1 = DateTime.Parse(s[0]);
                    DateTime d2 = DateTime.Parse(s[1]);
                    DateTime d;
                    if (d1 < d2)
                    {
                        d = d2;
                        d2 = d1;
                        d1 = d;
                    }

                    _histViews = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
                    .Where(x => x.u.stuff.Resturant == _restaurant)
                    .Where(x => x.o.OrderDate <= d1 && x.o.OrderDate >= d2)
                    .Select(x => new HistView
                    {
                        Id = x.o.Id,
                        UserName = x.o.User.Username,
                        OrderDate = x.o.OrderDate.ToString(),
                        TotalAmount = x.o.TotalAmount.ToString(),
                        Type = x.o.OrderType.ToString(),
                        UName = x.o.User.Name
                    })
                    .GroupBy(x => x.Id)
                    .Select(y => y.First())
                    .ToList();
                    HistoryDg.ItemsSource = _histViews;
                }
                catch
                {
                    MessageBox.Show("Invalid Date Format");
                }
            }

        }

        private void CommentDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == CommentDg)
            {
                if (CommentDg.SelectedIndex == -1 || CommentDg.SelectedIndex == CommentDg.Items.Count)
                    return;
                AnswerTx.Text = (CommentDg.SelectedItem as CommentView).Answer;
                AnswerBtn.IsEnabled = true;
            }
        }

        private void AnswerBtn_Click(object sender, RoutedEventArgs e)
        {
            Comment comment = _context.Comments.Where(x => x.Id.ToString() == (CommentDg.SelectedItem as CommentView).Id).First();
            comment.Answer = AnswerTx.Text;
            _context.SaveChanges();
            AnswerTx.Clear();
            CommentDg.ItemsSource = _context.Comments
                            .Where(x => x.Stuff.Id == id)
                            .Select(x => new CommentView
                            {
                                Id = x.Id.ToString(),
                                Answer = x.Answer,
                                Name = x.Users.Name,
                                Details = x.Details
                            })
                            .ToList();
            CommentDg.SelectedIndex = -1;
            AnswerBtn.IsEnabled = false;
        }

        private void ActivateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_restaurant.AvgRate >= 4.5)
            {
                Restaurant A = _context.Restaurants.Where(x => x.Id == _restaurant.Id).First();
                StTxb.Text = "Status :";
                if (_restaurant.Reservation)
                {
                    A.Reservation = false;
                    AcTxb.Text = "Deactivated";
                    ActivateBtn.Content = "Activate";
                }
                else
                {
                    A.Reservation = true;
                    AcTxb.Text = "Activated";
                    ActivateBtn.Content = "Deactivate";
                }
                _context.SaveChanges();
                _restaurant = A;
            }
            else
            {
                AcTxb.Visibility = Visibility.Hidden;
                ActivateBtn.Visibility = Visibility.Hidden;
                StTxb.Visibility = Visibility.Hidden;
            }
        }

        private void PassEnaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (passwordNewTx.Password == "" || passwordNewRTx.Password == "" || passwordOldTx.Password == "")
            {
                MessageBox.Show("Please Fill All Fields");
                return;
            }
            if (MainWindow.CreateMD5(passwordOldTx.Password) != _user.Password)
            {
                MessageBox.Show("Old Password is Incorrect");
                return;
            }
            if (passwordNewTx.Password != passwordNewRTx.Password)
            {
                MessageBox.Show("New Passwords are not the same");
                return;
            }
            _user = _context.Users.Where(x => x.Id == _user.Id).First();
            _user.Password = MainWindow.CreateMD5(passwordNewRTx.Password);
            _context.SaveChanges();
            passwordNewTx.Clear();
            passwordNewRTx.Clear();
            passwordOldTx.Clear();

        }
    }
}
