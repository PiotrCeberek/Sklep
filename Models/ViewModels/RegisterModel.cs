using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Pole jest wymagane")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane")]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "{0} musi mieć co najmniej {2} znaków i maksymalnie {1} znaków.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Hasła nie są takie same")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        public string ConfirmPassword { get; set; }
    }
}
