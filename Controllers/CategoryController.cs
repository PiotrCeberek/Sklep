using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;

namespace Projekt.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(AppDbContext context, IWebHostEnvironment environment, ILogger<CategoryController> logger) : base(context)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    if (imageFile.FileName.Length > 100)
                    {
                        ModelState.AddModelError("imageFile", "Za długa nazwa zdjęcia (max 100 znaków)");
                        return View(category);
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images/categories");
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
                    category.ImagePath = "/images/categories/" + uniqueFileName;
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas tworzenia kategorii.");
                ModelState.AddModelError("", "Błąd podczas tworzenia kategorii. Spróbuj ponownie.");
                return View(category);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, Category category, IFormFile imageFile)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                var existingCategory = await _context.Categories.FindAsync(id);
                if (existingCategory == null)
                {
                    return NotFound();
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (imageFile.FileName.Length > 100)
                    {
                        ModelState.AddModelError("imageFile", "Za długa nazwa zdjęcia (max 100 znaków)");
                        return View(category);
                    }

                    if (!string.IsNullOrEmpty(existingCategory.ImagePath))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, existingCategory.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images/categories");
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
                    existingCategory.ImagePath = "/images/categories/" + uniqueFileName;
                }

                existingCategory.Name = category.Name;
                _context.Update(existingCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd przy zmienianiu ID kategorii.", id);
                ModelState.AddModelError("", "Błąd podczas tworzenia kategorii. Spróbuj ponownie.");
                return View(category);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            if (category.Products != null && category.Products.Any())
            {
                TempData["ErrorMessage"] = "Nie można usunąć tej kategorii, ponieważ posiada powiązane produkty. Najpierw usuń produkty.";
                return RedirectToAction(nameof(Categories));
            }

            try
            {
                if (!string.IsNullOrEmpty(category.ImagePath))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, category.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Pomyślnie usunięto kategorię.";
                return RedirectToAction(nameof(Categories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd przy zmienianiu ID kategorii.", id);
                TempData["ErrorMessage"] = "Błąd podczas tworzenia kategorii. Spróbuj ponownie.";
                return RedirectToAction(nameof(Categories));
            }
        }

        private async Task<bool> CategoryExists(int id)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == id);
        }
    }
}
