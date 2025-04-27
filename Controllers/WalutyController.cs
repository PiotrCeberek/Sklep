using Microsoft.AspNetCore.Mvc;
using Projekt.Data;
using Projekt.Data.Projekt.Services;

namespace Projekt.Controllers
{
    public class WalutyController : Controller
    {
        private readonly ApiService _apiService;
        private readonly AppDbContext _context;

        public WalutyController(ApiService apiService, AppDbContext context)
        {
            _apiService = apiService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string apiUrl = "https://api.nbp.pl/api/exchangerates/rates/a/usd";
            var waluty = await _apiService.GetWalutyAsync(apiUrl);

            _context.Waluty.Add(waluty);
            await _context.SaveChangesAsync();

            return View(waluty);
        }
    }
}
