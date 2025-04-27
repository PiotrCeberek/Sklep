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
        [StringLength(40, MinimumLength = 6, ErrorMessage = "The {0} must be at {2} and at max {1} character")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Hasła nie są takie same")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}
