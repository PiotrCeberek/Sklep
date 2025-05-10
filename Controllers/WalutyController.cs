using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projekt.Data;
using Projekt.Data.Projekt.Services;
using Projekt.Models;
using System.Globalization;
using System.Text.Json;

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

        [HttpPost]
        public async Task<IActionResult> Index(string waluta)
        {
            HttpContext.Session.SetString("WybranaWaluta", waluta);
            if (waluta != "PLN")
            {
                string apiUrl = $"https://api.nbp.pl/api/exchangerates/rates/a/{waluta}";
                var waluty = await _apiService.GetWalutyAsync(apiUrl);

                _context.Waluty.Add(waluty);
                await _context.SaveChangesAsync();


                var kurs = _context.Waluty.OrderByDescending(p => p.Id).FirstOrDefault();

                if (kurs != null)
                {
                    var cena = kurs.Cena.ToString();
                    HttpContext.Session.SetString("Mnoznik", cena);
                    Console.WriteLine("****" + cena + "****");
                }
            }
            else
            {
                HttpContext.Session.SetString("Mnoznik", "1");
            }

            var products = _context.Products.ToList();
            foreach (var product in products)
            {
                product.Price = product.Price * decimal.Parse(HttpContext.Session.GetString("Mnoznik"));
            }
            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }

    }
}
