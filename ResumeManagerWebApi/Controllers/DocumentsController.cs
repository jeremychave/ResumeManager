using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApi.Common;
using ResumeManagerWebApi.Controllers.Dtos.Document;
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

        [HttpGet("{userEmail}")]
        public async Task<IActionResult> GetAllDocuments(string userEmail)
        {
            var validationError = ValidateRequestHeader(Request.Headers);

            if (!string.IsNullOrEmpty(validationError))
            {
                return Unauthorized(validationError);
            }

            var documentBo = await _documentService.GetAllDocuments(userEmail);

            var responseDto = new GetAllDocumentsResponseDto
            {
                Documents = documentBo.Select(bo => new DocumentDto
                {
                    BlobName = bo.BlobName,
                    FileName = bo.FileName,
                    Size = bo.Size
                })
            };

            return Ok(responseDto);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentRequestDto request)
        {
            var validationError = ValidateRequestHeader(Request.Headers);

            if (!string.IsNullOrEmpty(validationError))
            {
                return Unauthorized(validationError);
            }

            var uploadResponse = await _documentService.Upload(request.File, request.UserEmail);

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

        [HttpDelete("{blobName}/{userEmail}")]
        public async Task<IActionResult> Delete(string blobName, string userEmail)
        {
            var deleteResponse = await _documentService.Delete(blobName, userEmail);

            if (deleteResponse.Success)
            {
                return Ok(deleteResponse);
            }
            else
            {
                return BadRequest(deleteResponse);
            }
        }

        private string ValidateRequestHeader(IHeaderDictionary header)
        {
            if (!header.TryGetValue(AppConstants.HeaderHttpApiKey, out var apiKey) ||
                apiKey != _configuration["ApiSettings:ResumeManagerApiKey"])
            {
                return "Invalid API Key";
            }

            var signature = Request.Headers[AppConstants.HeaderHttpUserSignature].ToString();
            var userEmail = Request.Headers[AppConstants.HeaderHttpUserEmail].ToString();
            var secret = _configuration["ApiSettings:SignatureSecret"];
            var expectedSignature = HmacHelper.GenerateSignature(userEmail, secret);

            if (signature != expectedSignature)
            {
                return "Invalid signature";
            }

            if (string.IsNullOrEmpty(userEmail))
            {
                return "Missing user Email";
            }

            return null;
        }
    }
}
