using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class ArticleComment
    {
        [Key]
        public int ArticleCommentId { get; set; }

        [Required(ErrorMessage = "Identyfikator użytkownika jest wymagany.")]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Identyfikator produktu jest wymagany.")]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Komentarz jest wymagany.")]
        public string Comment { get; set; }

        [Range(1, 5, ErrorMessage = "Ocena musi być liczbą od 1 do 5.")]
        public int Rating { get; set; }

        public DateTime Date { get; set; }

        public Users User { get; set; }
        public Product Product { get; set; }
    }
}
