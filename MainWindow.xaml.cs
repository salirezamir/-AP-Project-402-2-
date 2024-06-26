using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.DAL;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Restaurant_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly RestaurantContext _context = new RestaurantContext();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
                _context.Database.EnsureCreated();
            /*
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */
            MessageBox.Show("Database created");
        }
    }
}