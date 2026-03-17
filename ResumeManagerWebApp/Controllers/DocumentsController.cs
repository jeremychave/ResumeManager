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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var documents = await _apiService.GetAllDocumentsAsync();
            return View(documents);
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var uploadDocumentResponse = await _apiService.UploadDocumentAsync(file);

            if (uploadDocumentResponse.Success) 
            {
                return RedirectToAction(nameof(Index));
            }

            if (uploadDocumentResponse.Errors != null)
            {
                foreach (var error in uploadDocumentResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Upload failed");
            }

            return View();
        }
    }
}
