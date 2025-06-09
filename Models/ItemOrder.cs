using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class ItemOrder
    {
        [Key]
        public int ItemOrderId { get; set; }

        [Required(ErrorMessage = "Pole Zamówienie jest wymagane.")]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Pole Produkt jest wymagane.")]
        [ForeignKey("Product")]
        public int? ProductId { get; set; }

        [Required(ErrorMessage = "Ilość jest wymagana.")]
        [Range(1, int.MaxValue, ErrorMessage = "Ilość musi być większa lub równa 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być większa niż 0.")]
        public decimal Price { get; set; }

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
