using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_Manager.Models
{
    internal class Rating
    {
        [Key,Required]
        public int Id { get; set; }
        [Required]
        public virtual User Users { get; set; }
        [Required]
        public int Rate { get; set; }
        [Required]
        public virtual Stuff? Stuff { get; set; }
        //public virtual Resturants? Restaurant { get; set; }
    }
}
