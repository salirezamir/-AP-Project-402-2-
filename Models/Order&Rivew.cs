using System.ComponentModel.DataAnnotations;

namespace Restaurant_Manager.Models
{
    public class Order
    {
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Required]
        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
    }

    public class Review
    {
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Required]
        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        [Required]
        public string ReviewText { get; set; }
        [Required]
        public int Rating { get; set; }
    }
}
