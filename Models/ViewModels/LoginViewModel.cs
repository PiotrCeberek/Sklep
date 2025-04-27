using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Emial is requied")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is requied")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }
}
