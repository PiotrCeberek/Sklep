using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace Projekt.Controllers
{
    public class GenerateFileController : Controller
    {
        private readonly AppDbContext _context;
        public GenerateFileController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int nrZamowienia)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            //var model = _context.Orders.OrderByDescending(p => p.OrderId).FirstOrDefault();

            var model = _context.Orders
            .Include(o => o.User)
            .Include(o => o.ItemOrders)
            .ThenInclude(io => io.Product)
            .FirstOrDefault(p => p.OrderId == nrZamowienia);

            string waluta = HttpContext.Session.GetString("WybranaWaluta");
            decimal mnoznik = decimal.Parse(HttpContext.Session.GetString("Mnoznik"));

            var document = new GenerateFileLook(model, waluta, mnoznik);



            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", "produkt.pdf");
        }
    }
}
