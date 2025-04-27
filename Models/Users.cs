using Microsoft.AspNetCore.Identity;

namespace Projekt.Models
{
    public class Users : IdentityUser
    {
        
        public string FullName { get; set; }
        public string Role { get; set; }

        public ICollection<ArticleComment> ArticleComments { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<History> Histories { get; set; }

    }
}
