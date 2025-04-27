using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;

namespace Projekt.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PromotionController : BaseController
    {
        private readonly AppDbContext _context;

        public PromotionController(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IActionResult ManagePromotions()
        {
            var currentDate = DateTime.Now;
            var promotions = _context.Promotions
                .Include(p => p.Product)
                .Where(p => p.StartDateTime <= currentDate && p.EndDateTime >= currentDate)
                .ToList();
            return View(promotions);
        }

        public IActionResult AddPromotion()
        {
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPromotion(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Promotions.Add(promotion);
                _context.SaveChanges();
                return RedirectToAction(nameof(ManagePromotions));
            }

            ViewBag.Products = _context.Products.ToList();
            return View(promotion);
        }

        public IActionResult EditPromotion(int id)
        {
            var promotion = _context.Promotions
                .Include(p => p.Product)
                .FirstOrDefault(p => p.PromotionId == id);
            if (promotion == null)
            {
                return NotFound();
            }

            ViewBag.Products = _context.Products.ToList();
            return View(promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPromotion(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Update(promotion);
                _context.SaveChanges();
                return RedirectToAction(nameof(ManagePromotions));
            }

            ViewBag.Products = _context.Products.ToList();
            return View(promotion);
        }

        public IActionResult DeletePromotion(int id)
        {
            var promotion = _context.Promotions
                .Include(p => p.Product)
                .FirstOrDefault(p => p.PromotionId == id);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        [HttpPost, ActionName("DeletePromotion")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePromotionConfirmed(int id)
        {
            var promotion = _context.Promotions.Find(id);
            if (promotion != null)
            {
                _context.Promotions.Remove(promotion);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(ManagePromotions));
        }
    }
}
