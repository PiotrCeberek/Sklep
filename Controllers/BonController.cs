using Microsoft.AspNetCore.Mvc;
using Projekt.Data;
using Projekt.Models;

namespace Projekt.Controllers
{
    public class BonController : Controller
    {
        private readonly AppDbContext _context;

        public BonController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult GenerujBon()
        {
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
                ProcentZnizki = discountPercent
            };

            _context.Bony.Add(bon);
            _context.SaveChanges();

            return RedirectToAction("PokazBony");
        }

        [HttpGet]
        public IActionResult PokazBony()
        {
            var bony = _context.Bony.ToList();
            return View(bony);
        }

        public IActionResult UzyjBon(string BonKod)
        {
            var bon = _context.Bony.FirstOrDefault(b => b.Kod == BonKod);

            if (bon == null)
            {
                TempData["Message"] = "Nie ma takiego kodu!";
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

            var znizka = bon.ProcentZnizki;
            HttpContext.Session.SetString("Znizka", bon.ProcentZnizki.ToString());

            TempData["Message"] = $"Bon {BonKod} został użyty! Pozostała liczba użyć: {bon.PozostalaIloscUzywan}";
            return RedirectToAction("Cart", "Shop");
        }




    }
}
