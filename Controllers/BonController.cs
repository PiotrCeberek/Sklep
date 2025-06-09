using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Projekt.Data;
using Projekt.Models;
using System.Globalization;
using System.Security.Claims;

namespace Projekt.Controllers
{
    public class BonController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public BonController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        public IActionResult GenerujBon()
        {
            var userId = _userManager.GetUserId(User);
            var random = new Random();

            string GenerateCode(int length = 3)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Range(0, length)
                    .Select(_ => chars[random.Next(chars.Length)]).ToArray());
            }

            DateTime expirationDate = DateTime.Now.AddDays(random.Next(1, 31));
            int remainingUses = random.Next(1, 6);
            int discountPercent = random.Next(1, 100);

            var bon = new Bon
            {
                Kod = GenerateCode(),
                DataWaznosci = expirationDate,
                PozostalaIloscUzywan = remainingUses,
                ProcentZnizki = discountPercent,
                UserId = userId
            };

            _context.Bony.Add(bon);
            _context.SaveChanges();

            return RedirectToAction("PokazBony");
        }
        [Authorize]
        [HttpGet]
        public IActionResult PokazBony()
        {
            var userId = _userManager.GetUserId(User);

            var bony_u = _context.Bony.Where(b => b.PozostalaIloscUzywan == 0).ToList();
            if (bony_u.Any())
            {
                _context.Bony.RemoveRange(bony_u);
                _context.SaveChanges();
            }

            var bony = _context.Bony
                .Where(b => b.UserId == userId)
                .ToList();

            ViewBag.UserId = userId;
            ViewBag.TimerSeconds = 300;

            return View(bony);
        }

        public IActionResult UzyjBon(string BonKod)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bonyDoUsuniecia = _context.Bony.Where(b => b.PozostalaIloscUzywan == 0).ToList();
            if (bonyDoUsuniecia.Any())
            {
                _context.Bony.RemoveRange(bonyDoUsuniecia);
                _context.SaveChanges();
            }

            var bon = _context.Bony.FirstOrDefault(b => b.Kod == BonKod);

            if (bon == null)
            {
                TempData["Message"] = "Nie ma takiego kodu!";
                HttpContext.Session.SetString("Znizka", "0");
                return RedirectToAction("Cart", "Shop");
            }

            if (bon.UserId != userId)
            {
                TempData["Message"] = "Ten bon nie należy do Ciebie!";
                HttpContext.Session.SetString("Znizka", "0");
                return RedirectToAction("Cart", "Shop");
            }

            if (bon.PozostalaIloscUzywan <= 0)
            {
                TempData["Message"] = "Ten bon został już wykorzystany!";
                HttpContext.Session.SetString("Znizka", "0");
                return RedirectToAction("Cart", "Shop");
            }

            bon.PozostalaIloscUzywan--;
            _context.SaveChanges();

            HttpContext.Session.SetString("Znizka", bon.ProcentZnizki.ToString());
            HttpContext.Session.SetString("BonKod", BonKod);

            TempData["Info"] = $"Bon {BonKod} zastosowany";
            return RedirectToAction("Cart", "Shop");
        }
        public IActionResult AnulujBon()
        {
            var bonKod = HttpContext.Session.GetString("BonKod");

            if (!string.IsNullOrEmpty(bonKod))
            {
                var bon = _context.Bony.FirstOrDefault(b => b.Kod == bonKod);

                if (bon != null)
                {
                    bon.PozostalaIloscUzywan++;
                    _context.SaveChanges();
                }

                HttpContext.Session.Remove("Znizka");
                HttpContext.Session.Remove("BonKod");

                TempData["Message"] = "Bon został anulowany.";
            }

            return RedirectToAction("Cart", "Shop");
        }


    }
}
