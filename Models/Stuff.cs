using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_Manager.Models
{
    internal class Stuff
    {
        public enum SType
        {
            Food,
            Drink,
            Dessert
        }
        [Key,Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public virtual Restaurant Resturant { get; set; }
        [Required]
        public string Materials { get; set; }
        [Required]
        public long Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public SType fType { get; set; }
        [Required]
        public int Rate { get; set; }
        public int PicFileId { get; set; }
    }
}
