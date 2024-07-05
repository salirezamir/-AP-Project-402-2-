using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_Manager.Models
{
    public class Restaurant
    {
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public bool Delivery { get; set; }
        [Required]
        public bool DineIn { get; set; }
        [Required]
        public double AvgRate { get; set; }
        [Required]
        public bool Reservation { get; set; }
    }
}

