using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApi.Models;

namespace ResumeManagerWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 999.99m },
                new Product { Id = 2, Name = "Mouse", Price = 19.99m },
                new Product { Id = 3, Name = "Keyboard", Price = 49.99m }
            };

            return Ok(products);
        }
    }
}
