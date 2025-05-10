using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;
using System.Globalization;

namespace Projekt.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(AppDbContext context, IWebHostEnvironment environment, ILogger<ProductController> logger) : base(context)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public IActionResult Products()
        {
            var products = _context.Products.Include(p => p.Category).ToList();

            string waluta = HttpContext.Session.GetString("WybranaWaluta");
            decimal mnoznik = decimal.Parse(HttpContext.Session.GetString("Mnoznik"));

            ViewBag.Mnoznik = mnoznik;
            ViewBag.Waluta = waluta;

            return View(products);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            ViewBag.CategoryList = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images/products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.ImagePath = "/images/products/" + uniqueFileName;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Produkt dodano pomyślnie.";
                return RedirectToAction("Products");
            }

            ViewBag.CategoryList = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
            return View(product);
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products.FindAsync(product.ProductId);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingProduct.ImagePath))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, existingProduct.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images/products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var extension = Path.GetExtension(imageFile.FileName);
                    var uniqueFileName = $"{Guid.NewGuid().ToString()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    existingProduct.ImagePath = "/images/products/" + uniqueFileName;
                }

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Description = product.Description;

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Produkt zaktualizowano pomyślnie.";
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteProduct")]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.Promotions)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            if (product.Promotions != null && product.Promotions.Any())
            {
                TempData["ErrorMessage"] = "Nie można usunąć produktu, ponieważ jest używany w aktywnych promocjach. Najpierw usuń promocje.";
                return RedirectToAction(nameof(Products));
            }

            try
            {
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, product.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Produkt usunięto pomyślnie.";
                return RedirectToAction(nameof(Products));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania produktu o ID {ProductId}.", id);
                TempData["ErrorMessage"] = "Wystąpił błąd podczas usuwania produktu. Spróbuj ponownie.";
                return RedirectToAction(nameof(Products));
            }
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _context.Products.AnyAsync(p => p.ProductId == id);
        }
    }
}