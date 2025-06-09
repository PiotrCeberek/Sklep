using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class CreateEmployeeModel
    {
        [Required(ErrorMessage = "Imię i nazwisko jest wymagane.")]
        [Display(Name = "Imię i nazwisko")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasła nie są zgodne.")]
        public string ConfirmPassword { get; set; }
    }
}
