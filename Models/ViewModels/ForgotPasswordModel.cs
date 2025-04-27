using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
