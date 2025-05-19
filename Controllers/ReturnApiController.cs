using Microsoft.AspNetCore.Mvc;
using Projekt.Data;
using Projekt.Models;

namespace Projekt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReturnApiController : ControllerBase
    {
        private readonly AppDbContext context;
        public ReturnApiController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetItems()
        {
            var items = context.Products.OrderByDescending(e => e.ProductId).ToList();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var item = context.Products.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);

        }

        [HttpPost]
        public IActionResult CreateItem(CreateProductDto nowy)
        {
            var product = new Product
            {
                Name = nowy.Name,
                Price = nowy.Price,
                CategoryId = nowy.CategoryId,
                ImagePath = nowy.ImagePath,
                Description = nowy.Description,
                Quantity = nowy.Quantity
            };
            context.Products.Add(product);
            context.SaveChanges();
            return Ok(product);
        }
        [HttpPut("{id}")]
        public IActionResult EditItem(int id, CreateProductDto nowy)
        {
            var item = context.Products.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            item.Name = nowy.Name;
            item.Price = nowy.Price;
            item.CategoryId = nowy.CategoryId;
            item.ImagePath = nowy.ImagePath;
            nowy.Description = nowy.Description;
            nowy.Quantity = nowy.Quantity;

            context.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id)
        {
            var item = context.Products.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            context.Products.Remove(item);
            context.SaveChanges();

            return Ok();
        }



    }
}
