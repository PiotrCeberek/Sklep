using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class UpdateStock
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int CurrentQuantity { get; set; }

        [Required]
        [Range(-1000, 1000, ErrorMessage = "Ilość do dodania musi być między -1000 a 1000.")]
        public int QuantityToAdd { get; set; }
    }
}
