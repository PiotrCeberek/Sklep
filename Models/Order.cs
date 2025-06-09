using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Id użytkownika jest wymagane.")]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Całkowita kwota musi być większa lub równa zero.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Data zamówienia jest wymagana.")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastUpdated { get; set; }

        public virtual Users User { get; set; }

        public virtual ICollection<ItemOrder>? ItemOrders { get; set; }
        public virtual ICollection<History>? Histories { get; set; }
    }
}
