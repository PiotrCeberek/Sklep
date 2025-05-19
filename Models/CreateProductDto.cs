using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    [Keyless]
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? ImagePath { get; set; }
        public string? Description { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
