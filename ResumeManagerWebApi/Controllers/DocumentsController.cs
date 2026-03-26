using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApi.Common;
using ResumeManagerWebApi.Services.Documents;

namespace ResumeManagerWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsService _documentService;
        private readonly IConfiguration _configuration;

        public DocumentsController(IDocumentsService documentService, IConfiguration configuration)
        {
            _documentService = documentService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDocuments()
        {
            if (!Request.Headers.TryGetValue(AppConstants.HeaderHttpApiKey, out var apiKey) ||
                apiKey != _configuration["ApiSettings:ResumeManagerApiKey"])
            {
                return Unauthorized("Invalid API Key");
            }

            var userEmail = Request.Headers[AppConstants.HeaderHttpUserEmail].ToString();
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Missing user Email");
            }

            var documentNames = await _documentService.GetAllDocuments();
            return Ok(documentNames);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var uploadResponse = await _documentService.Upload(file);

            if (uploadResponse.Success)
            {
                return Ok(uploadResponse);
            }
            else
            {
                return BadRequest(uploadResponse);
            }
        }

        [HttpGet("download/{blobName}")]
        public async Task<IActionResult> Download(string blobName)
        {
            var stream = await _documentService.Download(blobName);
            return File(stream, "application/octet-stream", blobName);
        }

        [HttpDelete("{blobName}")]
        public async Task<IActionResult> Delete(string blobName)
        {
            var deleteResponse = await _documentService.Delete(blobName);

            if (deleteResponse.Success)
            {
                return Ok(deleteResponse);
            }
            else
            {
                return BadRequest(deleteResponse);
            }
        }
    }
}
