using Restaurant_Manager.CustomerPannle;
using System.Data.Entity;

namespace CustomerPanelApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public AppDbContext() : base("Server")
        {
        }
    }
}
