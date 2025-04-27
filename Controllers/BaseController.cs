using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Projekt.Data;

namespace Projekt.Controllers
{
    public class BaseController : Controller
    {
        protected readonly AppDbContext _context;

        public BaseController(AppDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            UpdateCartCount();
            UpdateFavoritesCount();
            base.OnActionExecuting(context);
        }

        protected void UpdateCartCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                int totalItems = _context.CartItems
                    .Where(c => c.UserId == userId)
                    .Sum(c => c.Quantity);
                ViewData["CartCount"] = totalItems.ToString();
            }
            else
            {
                ViewData["CartCount"] = "0";
            }
        }

        protected void UpdateFavoritesCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                int favoriteCount = _context.Favorites
                    .Where(f => f.UserId == userId)
                    .Count();
                ViewData["FavoritesCount"] = favoriteCount.ToString();
            }
            else
            {
                ViewData["FavoritesCount"] = "0";
            }
        }
    }
}
