using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;
using Projekt.Models.Email;

namespace Projekt.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailService _emailService;

        public OrderController(AppDbContext context, UserManager<Users> userManager, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Użytkownik nie jest zalogowany.");
            }

            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Twój koszyk jest pusty.";
                return RedirectToAction("Index", "Cart");
            }

            foreach (var cartItem in cartItems)
            {
                if (cartItem.Product == null)
                {
                    TempData["Error"] = "Jeden z produktów w koszyku nie istnieje. Usuń go z koszyka i spróbuj ponownie.";
                    return RedirectToAction("Index", "Cart");
                }

                if (cartItem.Product.Quantity < cartItem.Quantity)
                {
                    TempData["Error"] = $"Niewystarczająca ilość w magazynie dla {cartItem.Product.Name}. Dostępne: {cartItem.Product.Quantity}, Żądane: {cartItem.Quantity}";
                    return RedirectToAction("Index", "Cart");
                }
            }

            var order = new Order
            {
                UserId = userId,
                Status = "Oczekujące",
                Total = cartItems.Sum(ci => ci.Price * ci.Quantity),
                OrderDate = DateTime.Now,
                ItemOrders = new List<ItemOrder>()
            };

            foreach (var cartItem in cartItems)
            {
                var itemOrder = new ItemOrder
                {
                    Order = order,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price
                };
                order.ItemOrders.Add(itemOrder);

                cartItem.Product.Quantity -= cartItem.Quantity;
                _context.Update(cartItem.Product);
            }

            var historyEntry = new History
            {
                UserId = userId,
                Order = order,
                Date = DateTime.Now
            };

            _context.Orders.Add(order);
            _context.Histories.Add(historyEntry);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            var employees = await _context.Users
                .Where(u => u.Role == "Employee")
                .ToListAsync();

            foreach (var employee in employees)
            {
                var notification = new Notification
                {
                    UserId = employee.Id,
                    Message = $"Nowe zamówienie #{order.OrderId} do realizacji.",
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                    Order = order
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                TempData["Error"] = "Nie udało się pobrać danych użytkownika lub adresu email.";
            }
            else
            {
                await _context.Entry(order)
                    .Collection(o => o.ItemOrders)
                    .Query()
                    .Include(io => io.Product)
                    .LoadAsync();

                string itemsList = string.Join("", order.ItemOrders.Select(item => $"<li>{(item.Product?.Name ?? "Nieznany produkt")} - {item.Quantity} szt. - {item.Price * item.Quantity:C}</li>"));
                string emailBody = $@"
            <h2>Potwierdzenie zamówienia #{order.OrderId}</h2>
            <p>Dziękujemy za złożenie zamówienia w naszym sklepie!</p>
            <p><strong>Status:</strong> {order.Status}</p>
            <p><strong>Data:</strong> {order.OrderDate.ToString("g")}</p>
            <p><strong>Produkty:</strong></p>
            <ul>{itemsList}</ul>
            <p><strong>Kwota całkowita:</strong> {order.Total.ToString("C")}</p>
            <p>Szczegóły zamówienia możesz zobaczyć w swojej historii zamówień.</p>
            <p>Pozdrawiamy,<br>Sklep Spożywczy</p>";

                try
                {
                    await _emailService.SendEmailAsync(user.Email, $"Zamówienie #{order.OrderId} - Potwierdzenie", emailBody);
                    TempData["Message"] = "Zamówienie złożone pomyślnie. Potwierdzenie wysłane na email.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Zamówienie złożone, ale nie udało się wysłać emaila: {ex.Message}";
                }
            }

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
        }

        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.ItemOrders)
                    .ThenInclude(io => io.Product)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> OrderHistory()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Użytkownik nie jest zalogowany.");
            }

            var history = await _context.Histories
                .Where(h => h.UserId == userId)
                .Include(h => h.Order)
                    .ThenInclude(o => o.ItemOrders)
                        .ThenInclude(io => io.Product)
                .OrderByDescending(h => h.Date)
                .ToListAsync();

            return View(history);
        }
        public async Task<IActionResult> OrderSummary()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Użytkownik nie jest zalogowany.");
            }

            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product)
              
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Twój koszyk jest pusty.";
                return RedirectToAction("Index", "Cart");
            }

            return View(cartItems);
        }
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound("Zamówienie nie znalezione.");
            }

            order.Status = status;
            order.LastUpdated = DateTime.Now;     
            _context.Update(order);
            await _context.SaveChangesAsync();

            var userEmail = order.User?.Email;
            if (!string.IsNullOrEmpty(userEmail))
            {
                string message = $"Twoje zamówienie #{orderId} ma teraz status: {status}";
                await _emailService.SendEmailAsync(userEmail, $"Aktualizacja statusu zamówienia: #{orderId}", message);
            }

            return Ok("Status zamówienia zaktualizowany i email wysłany.");
        }
    }
}
