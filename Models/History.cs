using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class History
    {
        [Key]
        public int HistoryId { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public DateTime Date { get; set; }

        public Users? User { get; set; }
        public Order? Order { get; set; }
    }
}
