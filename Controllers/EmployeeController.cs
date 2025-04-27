using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;
using Projekt.Models.Email;
using System.Threading.Tasks;

namespace Projekt.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public EmployeeController(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context, EmailService emailService) : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _emailService = emailService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            return View(employees);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    FullName = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    NormalizedEmail = model.Email.ToUpper(),
                    NormalizedUserName = model.Email.ToUpper(),
                    Role = "Employee"
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Employee"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Employee"));
                    }
                    await _userManager.AddToRoleAsync(user, "Employee");
                    TempData["Message"] = "Pracownik dodany pomyślnie.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Message"] = "Pracownik usunięty pomyślnie.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(user);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Notifications()
        {
            var notifications = await _context.Notifications
                .Include(n => n.Order)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ProcessOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.ItemOrders)
                    .ThenInclude(io => io.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.OrderId == orderId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Update(notification);
                await _context.SaveChangesAsync();
            }

            return View(order);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.ItemOrders)
                    .ThenInclude(io => io.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            foreach (var item in order.ItemOrders)
            {
                if (item.Product == null)
                {
                    TempData["Error"] = $"Produkt w zamówieniu nie istnieje: {item.ProductId}.";
                    return RedirectToAction(nameof(ProcessOrder), new { orderId });
                }

                if (item.Product.Quantity < item.Quantity)
                {
                    TempData["Error"] = $"Niewystarczająca ilość produktu {item.Product.Name} w magazynie. Dostępne: {item.Product.Quantity}.";
                    return RedirectToAction(nameof(ProcessOrder), new { orderId });
                }
            }

            order.Status = "Gotowe do odbioru";
            order.LastUpdated = DateTime.Now;
            _context.Update(order);

            if (!string.IsNullOrEmpty(order.User?.Email))
            {
                string itemsList = string.Join("", order.ItemOrders.Select(item => $"<li>{item.Product.Name} - {item.Quantity} szt. - {item.Price * item.Quantity:C}</li>"));
                string emailBody = $@"
                    <h2>Zamówienie #{order.OrderId} - Gotowe do odbioru</h2>
                    <p>Twoje zamówienie zostało skompletowane i jest gotowe do odbioru.</p>
                    <p><strong>Status:</strong> {order.Status}</p>
                    <p><strong>Data:</strong> {order.OrderDate.ToString("g")}</p>
                    <p><strong>Produkty:</strong></p>
                    <ul>{itemsList}</ul>
                    <p><strong>Kwota całkowita:</strong> {order.Total.ToString("C")}</p>
                    <p>Pozdrawiamy,<br>Sklep Spożywczy</p>";

                await _emailService.SendEmailAsync(order.User.Email, $"Zamówienie #{order.OrderId} - Gotowe do odbioru", emailBody);
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Zamówienie zostało zrealizowane i jest gotowe do odbioru.";
            return RedirectToAction(nameof(Notifications));
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ManageStock()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<IActionResult> UpdateStock(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return NotFound();
            }

            var model = new UpdateStock
            {
                ProductId = product.ProductId,
                ProductName = product.Name,
                CurrentQuantity = product.Quantity
            };

            return View(model);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(UpdateStock model)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

                if (product == null)
                {
                    return NotFound();
                }

                var newQuantity = product.Quantity + model.QuantityToAdd;

                if (newQuantity < 0)
                {
                    ModelState.AddModelError("QuantityToAdd", "Nie można ustawić ujemnego stanu magazynowego.");
                    model.ProductName = product.Name;
                    model.CurrentQuantity = product.Quantity;
                    return View(model);
                }

                product.Quantity = newQuantity;
                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Stan magazynowy produktu {product.Name} zaktualizowany pomyślnie.";
                return RedirectToAction(nameof(ManageStock));
            }

            return View(model);
        }
        
    }
}