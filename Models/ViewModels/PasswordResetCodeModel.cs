using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.ViewModels
{
    public class PasswordResetCodeModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? UseAt { get; set; }

        public virtual Users User { get; set; }

    }
}
