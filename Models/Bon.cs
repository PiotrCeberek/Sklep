using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Projekt.Models
{
    public class Bon
    {
        [Key]
        public int Id { get; set; }
        public string Kod { get; set; }
        public DateTime DataWaznosci { get; set; }
        public int PozostalaIloscUzywan { get; set; }
        public double ProcentZnizki { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public Users? User { get; set; }


    }
}
