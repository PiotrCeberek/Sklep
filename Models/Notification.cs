using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        [ForeignKey("Order")]
        public int? OrderId { get; set; }

        public virtual Order Order { get; set; }
    }
}
