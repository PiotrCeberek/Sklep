using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Wprowadź poprawny adres e-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj mnie?")]
        public bool RememberMe { get; set; }
    }
}
