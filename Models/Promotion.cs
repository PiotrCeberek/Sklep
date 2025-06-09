using System;
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

        [Required(ErrorMessage = "Nazwa promocji jest wymagana.")]
        public string Name { get; set; }

        [Range(0, 99, ErrorMessage = "Zniżka musi mieć od 0 do 99 procent.")]
        public decimal Discount { get; set; }

        [Required(ErrorMessage = "Data rozpoczęcia promocji jest wymagana.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "Data zakończenia promocji jest wymagana.")]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }

        public Product? Product { get; set; }
    }

   
    
}
