using ResumeManagerWebApp.DTOs.Document;
using ResumeManagerWebApp.Models;

namespace ResumeManagerWebApp.Services
{
    public interface IDocumentApiService
    {
        Task<List<Document>> GetAllDocumentsAsync(string userEmail);

        Task<UploadDocumentResponseDto> UploadDocumentAsync(IFormFile file);

        Task<DeleteDocumentResponseDto> DeleteDocumentAsync(string blobName);
    }
}
