using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class VerifyCodeModel
    {
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Kod jest wymagany.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Kod musi mieć 6 cyfr.")]
        public string Code { get; set; }
    }
}
