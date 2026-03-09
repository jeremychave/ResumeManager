using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApp.Services;

namespace ResumeManagerWebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductApiService _apiService;

        public ProductsController(ProductApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _apiService.GetProductsAsync();
            return View(products);
        }
    }
}
