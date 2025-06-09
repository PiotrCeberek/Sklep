using System;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class PasswordResetCodeModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Identyfikator użytkownika jest wymagany.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Kod resetowania hasła jest wymagany.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Data utworzenia jest wymagana.")]
        public DateTime CreatedAt { get; set; }

        public DateTime EndTime { get; set; }
        public DateTime? UseAt { get; set; }

        public virtual Users User { get; set; }
    }
}
