using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApi.Repositories;

namespace ResumeManagerWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsRepository _documentRepository;

        public DocumentsController(IDocumentsRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var blobName = await _documentRepository.Upload(file);
            return Ok(new { blobName });
        }

        [HttpGet("download/{blobName}")]
        public async Task<IActionResult> Download(string blobName)
        {
            var stream = await _documentRepository.Download(blobName);
            return File(stream, "application/octet-stream", blobName);
        }

        [HttpDelete("{blobName}")]
        public async Task<IActionResult> Delete(string blobName)
        {
            await _documentRepository.Delete(blobName);
            return NoContent();
        }
    }
}
