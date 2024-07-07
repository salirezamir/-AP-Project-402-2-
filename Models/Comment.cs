using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_Manager.Models
{
    public class Comment
    {
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public virtual User Users { get; set; }
        [Required]
        public string Details { get; set; }
        [Required]
        public virtual Stuff Stuff { get; set; }
        public string Answer { get; set; }
        public bool IsEdited { get; set; }
        public DateTime Date { get; set; }
    }
}
