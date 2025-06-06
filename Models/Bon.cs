using System.ComponentModel.DataAnnotations;
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

    }
}
