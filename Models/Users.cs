using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_Manager.Models
{
    internal class Users
    {
        public enum Types
        {
            Admin,
            Customer,
            Restaurant
        }
        public enum Tiers
        {
            Gold,
            Silver,
            Bronze
        }
        public enum Genders
        {
            Male,
            Female
        }
        [Key,Required]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public Types Type { get; set; }
        [Required]
        public Tiers Tier { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Name { get; set; }
        public long Zipcode { get; set; }
        public Genders Gender { get; set; }

    }
}
