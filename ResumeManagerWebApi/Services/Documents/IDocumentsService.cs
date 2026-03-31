using ResumeManagerWebApi.Services.Documents.Bo;
using ResumeManagerWebApi.Services.Documents.Responses;

namespace ResumeManagerWebApi.Services.Documents
{
    public interface IDocumentsService
    {
        public Task<IEnumerable<DocumentBo>> GetAllDocuments(string userEmail);

        public Task<UploadDocumentResponse> Upload(IFormFile file);

        public Task<Stream> Download(string blobName);

        public Task<DeleteDocumentResponse> Delete(string blobName);
    }
}
