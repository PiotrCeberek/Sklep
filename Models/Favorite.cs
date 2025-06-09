using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }

        [Required(ErrorMessage = "Pole Użytkownik jest wymagane.")]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Pole Produkt jest wymagane.")]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Users? User { get; set; }
        public Product? Product { get; set; }
    }
}
