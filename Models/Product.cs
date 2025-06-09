using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Nazwa produktu jest wymagana.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Cena produktu jest wymagana.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być większa od 0.01")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Kategoria produktu jest wymagana.")]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public string? ImagePath { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Ilość produktu jest wymagana.")]
        [Range(0, int.MaxValue, ErrorMessage = "Ilość nie może być ujemna.")]
        public int Quantity { get; set; } = 0;

        public Category? Category { get; set; }
        public ICollection<ItemOrder>? ItemOrders { get; set; }
        public ICollection<ArticleComment>? ArticleComments { get; set; }
        public ICollection<Promotion>? Promotions { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
    }
}
