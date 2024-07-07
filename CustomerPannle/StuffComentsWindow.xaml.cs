using Restaurant_Manager.Models;
using System.Windows;

namespace Restaurant_Manager.CustomerPannle
{
    public partial class StuffComentsWindow : Window
    {
        private Stuff _stuff;
        public StuffComentsWindow(Stuff stuff)
        {
            InitializeComponent();
            _stuff = stuff;
            DataContext = _stuff;
            // Load comments
        }
    }
}
