using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public string? Status { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? LastUpdated { get; set; }

        public Users User { get; set; }
        public ICollection<ItemOrder>? ItemOrders { get; set; }
        public ICollection<History>? Histories { get; set; }
        

    }
}
