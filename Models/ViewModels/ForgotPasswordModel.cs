using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "E-mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "Wprowadź poprawny adres e-mail.")]
        public string Email { get; set; }
    }
}