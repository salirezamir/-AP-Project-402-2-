using System.ComponentModel.DataAnnotations;

namespace Restaurant_Manager.Models
{
    public class Order
    {
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public virtual Restaurant Restaurant { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
    }
}
