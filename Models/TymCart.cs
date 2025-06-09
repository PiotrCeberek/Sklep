using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class TymCart
    {
        [Key]
        public int TymCartId { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Ilość musi być co najmniej 1.")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być większa od 0.")]
        public decimal Price { get; set; }

        public bool Available { get; set; }

        public Order? Order { get; set; }
    }
}
