using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_Manager.Models
{
    public class Complaint
    {
        public enum CStatus
        {
            Pending,
            Answered
        }
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public string Details { get; set; }
        public string? Answer { get; set; }
        [Required]
        public virtual User Users { get; set; }
        [Required]
        public virtual Restaurant? Restaurant { get; set; }
        [Required]
        public CStatus Status { get; set; }
    }
}
