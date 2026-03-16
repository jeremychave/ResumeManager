using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApp.Services;

namespace ResumeManagerWebApp.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly DocumentApiService _apiService;

        public DocumentsController(DocumentApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var documents = await _apiService.GetAllDocumentsAsync();
            return View(documents);
        }
    }
}
