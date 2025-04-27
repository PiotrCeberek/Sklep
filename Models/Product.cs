using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        
        public string? ImagePath { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Quantity { get; set; } = 0;

        public Category? Category { get; set; }
        public ICollection<ItemOrder>? ItemOrders { get; set; }
        public ICollection<ArticleComment>? ArticleComments { get; set; }
        public ICollection<Promotion>? Promotions { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        

    }
}
