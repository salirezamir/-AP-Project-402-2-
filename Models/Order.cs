using System;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_Manager.Models
{
    public class Order
    {
        public enum Type
        {
            Delivery,
            Reservation
        }
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public Type OrderType { get; set; }
        [Required]
        public int Rate { get; set; } = 0;
    }
}
