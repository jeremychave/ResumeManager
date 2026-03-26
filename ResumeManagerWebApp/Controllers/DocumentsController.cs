using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApp.Services;

namespace ResumeManagerWebApp.Controllers
{
    [Authorize]
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
            var userEmail = User.FindFirst("preferred_username")?.Value;
            var documents = await _apiService.GetAllDocumentsAsync(userEmail);
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
                TempData["Message"] = $"Document {uploadDocumentResponse.FileName} uploaded successfully.";
            }
            else
            {
                TempData["Error"] = uploadDocumentResponse.Errors != null 
                    ? string.Join("<br>", uploadDocumentResponse.Errors) 
                    : "Upload failed";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string blobName)
        {
            var response = await _apiService.DeleteDocumentAsync(blobName);

            if (response.Success)
            {
                TempData["Message"] = $"Document {response.FileName} deleted successfully.";
            }
            else
            {
                TempData["Error"] = response.Errors != null
                    ? string.Join("<br>", response.Errors)
                    : $"Failed to delete the document : {blobName}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
