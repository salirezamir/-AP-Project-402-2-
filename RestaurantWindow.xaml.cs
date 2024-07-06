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

namespace Restaurant_Manager
{
    /// <summary>
    /// Interaction logic for RestaurantWindow.xaml
    /// </summary>
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
        public string UserName { get; set; }
        public string OrderDate { get; set; }
        public string TotalAmount { get; set; }
        public string Type { get; set; }
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
        List<HistView> _histViews ;
        User _user;
        Restaurant _restaurant;
        bool repeat = false;
        int id = -1;
        private readonly RestaurantContext _context = new RestaurantContext();

        public RestaurantWindow(Restaurant restaurant,User user)
        {
            InitializeComponent();
            _restaurant = restaurant;
            _user = user;
            //_histViews = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
            //    .Where(x => x.u.stuff.Resturant == _restaurant)
            //    .Select(x => new HistView { 
            //        Id = x.o.Id.ToString(),
            //        UserName = x.o.User.Name,
            //        OrderDate = x.o.OrderDate.ToString(),
            //        TotalAmount = x.o.TotalAmount.ToString(),
            //        Type = x.u.stuff.fType.ToString() 
            //    }).ToList();
            _histViews = _context.Order_Stuffs.Where(x => x.stuff.Resturant == _restaurant)
                .GroupBy(x => x.order)
                .ToList()
                .Select(x => new HistView
                {
                    Id = x.Key.Id,
                    UserName = x.Key.User.Name,
                    OrderDate = x.Key.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    TotalAmount = x.Key.TotalAmount.ToString(),
                    Type = x.Key.OrderType.ToString()
                }).ToList();
            DataContext = new RestVM();
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
                    }
                    else
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
                    }
                    return;
                }
                if (Tb.SelectedIndex == 0)
                {
                    
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
                _context.Stuffs.Add(stuff);
            }
            _context.SaveChanges();
            NameRsTx.Clear();
            MaterialTx.Clear();
            QuantityTx.Clear();
            PriceTx.Clear();
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
        }

        private void SaveFilterBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HistoryDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == HistoryDg)
            {
                if (HistoryDg.SelectedIndex == -1 || HistoryDg.SelectedIndex == HistoryDg.Items.Count)
                    return;
                StuffsDg.ItemsSource = _context.Order_Stuffs
                    .Where(x => x.order.Id == (HistoryDg.SelectedItem as HistView).Id)
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
                HistoryDg.ItemsSource = _histViews.Where(x => x.UserName == FilterHiTx.Text);
            }
            else if (FilterHiCb.SelectedIndex == 1)
            {
               HistoryDg.ItemsSource = _context.Orders.Join(_context.Order_Stuffs, o => o, u => u.order, (o, u) => new { o, u })
                    .Where(x => x.o.User.Phone == FilterHiTx.Text && x.u.stuff.Resturant == _restaurant)
                    .Select(x => new HistView
                    {
                        Id = x.o.Id,
                        UserName = x.o.User.Name,
                        OrderDate = x.o.OrderDate.ToString(),
                        TotalAmount = x.o.TotalAmount.ToString(),
                        Type = x.u.stuff.fType.ToString()
                    }).ToList();
            }
            else if (FilterHiCb.SelectedIndex == 2)
            {
                HistoryDg.ItemsSource = _context.Order_Stuffs.Where(x => x.stuff.Name == FilterHiTx.Text && x.stuff.Resturant == _restaurant)
                    .GroupBy(x => x.order)
                    .ToList()
                    .Select(x => new HistView
                    {
                        Id = x.Key.Id,
                        UserName = x.Key.User.Name,
                        OrderDate = x.Key.OrderDate.ToString(),
                        TotalAmount = x.Key.TotalAmount.ToString(),
                        Type = x.Key.OrderType.ToString()
                    }).ToList();
            }
            else if (FilterHiCb.SelectedIndex == 3)
            {
                if (long.TryParse(FilterHiTx.Text, out long amount))
                {
                    HistoryDg.ItemsSource = _histViews.Where(x => long.Parse(x.TotalAmount) >= amount);
                }
            }
            else if (FilterHiCb.SelectedIndex == 4)
            {
                if (long.TryParse(FilterHiTx.Text, out long amount))
                {
                    HistoryDg.ItemsSource = _histViews.Where(x => long.Parse(x.TotalAmount) <= amount);
                }
            }
            else if (FilterHiCb.SelectedIndex == 5)
            {
                HistoryDg.ItemsSource = _histViews.Where(x => x.Type == FilterHiTx.Text);
            }
            else if (FilterHiCb.SelectedIndex == 6)
            {
                try
                {
                    var s = FilterHiTx.Text.Split("/");
                    DateTime d1 = DateTime.Parse(s[0]);
                    DateTime d2 = DateTime.Parse(s[1]);
                    TimeSpan df = d1 - d2;

                    HistoryDg.ItemsSource = _context.Order_Stuffs.Where(x => x.order.OrderDate - d1 <= df && x.order.OrderDate - d2 >= df)
                        .GroupBy(x => x.order)
                        .ToList()
                        .Select(x => new HistView
                        {
                            Id = x.Key.Id,
                            UserName = x.Key.User.Name,
                            OrderDate = x.Key.OrderDate.ToString(),
                            TotalAmount = x.Key.TotalAmount.ToString(),
                            Type = x.Key.OrderType.ToString()
                        }).ToList();
                }
                catch
                {
                    MessageBox.Show("Invalid Date Format");
                }
            }

        }
    }
}
