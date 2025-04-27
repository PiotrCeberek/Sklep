using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Models
{
    public class ArticleComment
    {
        [Key]
        public int ArticleCommentId { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public string Comment { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime Date { get; set; }

        
        public Users User { get; set; }
        public Product Product { get; set; }

    }
}
