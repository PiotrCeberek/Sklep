using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Pole jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Stare hasło")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "Hasło musi mieć od {2} do {1} znaków")]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Hasła nie są takie same")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź nowe hasło")]
        public string ConfirmNewPassword { get; set; }
    }
}
