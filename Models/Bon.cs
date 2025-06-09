using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class Bon
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kod bonu jest wymagany.")]
        public string Kod { get; set; }

        [Required(ErrorMessage = "Data ważności jest wymagana.")]
        [DataType(DataType.Date)]
        public DateTime DataWaznosci { get; set; }

        [Required(ErrorMessage = "Pozostała ilość użyć jest wymagana.")]
        [Range(1, int.MaxValue, ErrorMessage = "Pozostała ilość użyć musi być liczbą nieujemną.")]
        public int PozostalaIloscUzywan { get; set; }

        [Required(ErrorMessage = "Procent zniżki jest wymagany.")]
        [Range(1, 100, ErrorMessage = "Procent zniżki musi być w zakresie od 0 do 100.")]
        public double ProcentZnizki { get; set; }

        [Required(ErrorMessage = "Identyfikator użytkownika jest wymagany.")]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public Users? User { get; set; }
    }
}
