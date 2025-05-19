using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var model = _context.Orders.OrderByDescending(p => p.OrderId).FirstOrDefault();
            var document = new GenerateFileLook(model);

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", "produkt.pdf");
        }
    }
}
