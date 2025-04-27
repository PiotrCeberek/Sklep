using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }
        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        public string? Name { get; set; }
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100 percent.")]
        public decimal Discount { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public Product? Product { get; set; }




    }
}
