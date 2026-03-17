using ResumeManagerWebApi.Services.Documents.Responses;

namespace ResumeManagerWebApi.Services.Documents
{
    public interface IDocumentsValidationService
    {
        public DocumentsValidationResponse Validate(IFormFile file);
    }
}
