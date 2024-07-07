using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_Manager.Models
{
    public class Order_Stuffs
    {
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public long Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public virtual Stuff stuff { get; set; }
        [Required]
        public virtual Order order { get; set; }
    }
}
