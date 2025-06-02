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

        [HttpGet]
        public IActionResult StworzBon()
        {
            var bony = _context.Bony.ToList();
            return View(bony);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StworzBon(Bon bon)
        {
            if (ModelState.IsValid)
            {
                _context.Bony.Add(bon);
                _context.SaveChanges();
                return RedirectToAction("StworzBon");
            }

            ViewBag.Bony = _context.Bony.ToList();
            return View(bon);
        }

    }
}
