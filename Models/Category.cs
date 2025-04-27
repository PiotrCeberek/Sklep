using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
