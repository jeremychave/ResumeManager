using ResumeManagerWebApi.Services.Documents.Responses;

namespace ResumeManagerWebApi.Services.Documents
{
    public class DocumentsValidationService : IDocumentsValidationService
    {
        public DocumentsValidationResponse Validate(IFormFile file)
        {
            var response = new DocumentsValidationResponse
            {
                Errors = new List<string>()
            };

            if (file == null || file.Length == 0)
            {
                response.Errors.Add("Please select a file.");
                return response;
            }

            const long maxBytes = 5L * 1024 * 1024; // 5 MB
            if (file.Length > maxBytes)
            {
                response.Errors.Add("File size must not exceed 5 MB.");
            }

            var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                response.Errors.Add($"{ext} is not a valid format file");
            }

            return response;
        }
    }
}
