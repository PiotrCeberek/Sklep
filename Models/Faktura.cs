using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class Faktura
    {
        [Key]
        public int FakturaId { get; set; }

        [Display(Name = "Data zakupu")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Data zakupu jest wymagana.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Użytkownik jest wymagany.")]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public Users User { get; set; }

        public List<ItemOrder> Items { get; set; }
    }
}
