using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class Faktura
    {
        [Key]
        public int FakturaId { get; set; }

        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PurchaseDate { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public Users User { get; set; }
        public List<ItemOrder> Items { get; set; }
    }
}
