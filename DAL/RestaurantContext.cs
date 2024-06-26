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
        public DbSet<Users> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Server"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }

    }
}
