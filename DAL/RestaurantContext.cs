using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Models;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_Manager.DAL
{
    internal class RestaurantContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Stuff> Stuffs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_Stuffs> Order_Stuffs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Server"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }

    }
}
