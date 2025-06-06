using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;
using System.Security.Claims;
using System.Text.Json;

namespace Projekt.Controllers
{
    public class ShopController : BaseController
    {
        private readonly AppDbContext _context;
        

        public ShopController(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IActionResult Index(int? categoryId)
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Promotions)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            var currentDate = DateTime.Now;
            var discountedProducts = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Promotions)
                .Where(p => p.Promotions.Any(prom => prom.StartDateTime <= currentDate && prom.EndDateTime >= currentDate))
                .ToList();

            var categories = _context.Categories.ToList();
            ViewData["Categories"] = categories;
            ViewData["DiscountedProducts"] = discountedProducts;
            ViewData["SelectedCategoryId"] = categoryId;
            UpdateCartCount();
            UpdateFavoritesCount();

            string waluta = HttpContext.Session.GetString("WybranaWaluta");
            decimal mnoznik = decimal.Parse(HttpContext.Session.GetString("Mnoznik"));

            ViewBag.Mnoznik = mnoznik;
            ViewBag.Waluta = waluta;

            return View(products.ToList());
        }

        public IActionResult CategoryProducts(int categoryId)
        {
            string waluta = HttpContext.Session.GetString("WybranaWaluta");
            decimal mnoznik = decimal.Parse(HttpContext.Session.GetString("Mnoznik"));

            ViewBag.Mnoznik = mnoznik;
            ViewBag.Waluta = waluta;

            var category = _context.Categories
                .FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return NotFound();
            }

            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Promotions)
                .Where(p => p.CategoryId == categoryId)
                .ToList();

            ViewData["CategoryName"] = category.Name;
            UpdateCartCount();
            UpdateFavoritesCount();
            return View(products);
        }
        [Authorize]
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var product = _context.Products
                .Include(p => p.Promotions)
                .FirstOrDefault(p => p.ProductId == productId);
            if (product == null) return NotFound();

            if (product.Quantity < quantity)
            {
                TempData["Error"] = $"Niewystarczająca ilość produktu {product.Name} w magazynie. Dostępne: {product.Quantity}.";
                return RedirectToAction(nameof(Index));
            }

            var currentDate = DateTime.Now;
            var activePromotion = product.Promotions?
                .FirstOrDefault(p => p.StartDateTime <= currentDate && p.EndDateTime >= currentDate);
            var priceToUse = activePromotion != null
                ? product.Price * (1 - activePromotion.Discount / 100)
                : product.Price;

            var cartItem = _context.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                if (product.Quantity < cartItem.Quantity)
                {
                    TempData["Error"] = $"Niewystarczająca ilość produktu {product.Name} w magazynie. Dostępne: {product.Quantity}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = priceToUse
                };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            UpdateCartCount();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Cart()
        {
            string znizkaString = HttpContext.Session.GetString("Znizka");
            decimal znizka = 0m;

            if (!string.IsNullOrEmpty(znizkaString) && decimal.TryParse(znizkaString, out decimal wynik))
            {
                znizka = wynik / 100m; // np. 10% -> 0.10
            }

            string waluta = HttpContext.Session.GetString("WybranaWaluta");
            decimal mnoznik = decimal.Parse(HttpContext.Session.GetString("Mnoznik"));

            ViewBag.Mnoznik = mnoznik;
            ViewBag.Waluta = waluta;
            ViewBag.Znizka = znizka;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var cartWithDetails = _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                    .ThenInclude(p => p.Promotions)
                .ToList();

            var currentDate = DateTime.Now;
            foreach (var item in cartWithDetails)
            {
                if (item.Product == null)
                {
                    _context.CartItems.Remove(item);
                    continue;
                }

                var activePromotion = item.Product.Promotions?
                    .FirstOrDefault(p => p.StartDateTime <= currentDate && p.EndDateTime >= currentDate);
                item.Price = activePromotion != null
                    ? item.Product.Price * (1 - activePromotion.Discount / 100)
                    : item.Product.Price;
            }

            _context.SaveChanges();
            UpdateCartCount();
            return View(cartWithDetails);
        }

        public IActionResult AllProducts()
        {
            string waluta = HttpContext.Session.GetString("WybranaWaluta");
            decimal mnoznik = decimal.Parse(HttpContext.Session.GetString("Mnoznik"));

            ViewBag.Mnoznik = mnoznik;
            ViewBag.Waluta = waluta;

            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Promotions)
                .ToList();

            UpdateCartCount();
            UpdateFavoritesCount();
            return View(products);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToFavorites(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null) return NotFound();

            var existingFavorite = _context.Favorites.FirstOrDefault(f => f.UserId == userId && f.ProductId == productId);
            if (existingFavorite == null)
            {
                var favorite = new Favorite
                {
                    UserId = userId,
                    ProductId = productId
                };
                _context.Favorites.Add(favorite);
                _context.SaveChanges();
            }

            UpdateCartCount();
            UpdateFavoritesCount();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult Favorites()
        {
            string waluta = HttpContext.Session.GetString("WybranaWaluta");
            decimal mnoznik = decimal.Parse(HttpContext.Session.GetString("Mnoznik"));

            ViewBag.Mnoznik = mnoznik;
            ViewBag.Waluta = waluta;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var favoriteProducts = _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Product)
                    .ThenInclude(p => p.Category)
                .Include(f => f.Product)
                    .ThenInclude(p => p.Promotions)
                .Select(f => f.Product)
                .ToList();

            UpdateCartCount();
            UpdateFavoritesCount();
            return View(favoriteProducts);
        }

        public IActionResult Details(int id)
        {
            string waluta = HttpContext.Session.GetString("WybranaWaluta");
            decimal mnoznik = decimal.Parse(HttpContext.Session.GetString("Mnoznik"));

            ViewBag.Mnoznik = mnoznik;
            ViewBag.Waluta = waluta;

            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Promotions)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            UpdateCartCount();
            UpdateFavoritesCount();
            return View(product);
        }

        [HttpGet]
        public IActionResult RemoveFromFavorites(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var favorite = _context.Favorites
                .FirstOrDefault(f => f.UserId == userId && f.ProductId == id);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult RemoveFromCart(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var cartItem = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.CartItemId == id); // Szukaj po CartItemId!

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            UpdateCartCount();
            return RedirectToAction(nameof(Cart));
        }




    }


}
    

       

       

