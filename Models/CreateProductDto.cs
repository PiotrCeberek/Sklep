using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    [Keyless]
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Nazwa produktu jest wymagana.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być większa niż 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Kategoria jest wymagana.")]
        public int CategoryId { get; set; }

        public string? ImagePath { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Ilość jest wymagana.")]
        [Range(1, int.MaxValue, ErrorMessage = "Ilość musi być większa bądź równa 1.")]
        public int Quantity { get; set; }
    }
}
