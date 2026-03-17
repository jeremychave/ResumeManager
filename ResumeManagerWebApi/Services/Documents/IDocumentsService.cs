using ResumeManagerWebApi.Models;
using ResumeManagerWebApi.Services.Documents.Responses;

namespace ResumeManagerWebApi.Services.Documents
{
    public interface IDocumentsService
    {
        public Task<IEnumerable<Document>> GetAllDocuments();

        public Task<UploadDocumentResponse> Upload(IFormFile file);

        public Task<Stream> Download(string blobName);

        public Task<DeleteDocumentResponse> Delete(string blobName);
    }
}
