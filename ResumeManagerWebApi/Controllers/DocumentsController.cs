using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApi.Services.Documents;

namespace ResumeManagerWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsService _documentService;

        public DocumentsController(IDocumentsService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDocuments()
        {
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
            await _documentService.Delete(blobName);
            return NoContent();
        }
    }
}
