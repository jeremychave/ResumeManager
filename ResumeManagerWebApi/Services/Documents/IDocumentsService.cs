using ResumeManagerWebApi.Services.Documents.Bo;
using ResumeManagerWebApi.Services.Documents.Responses;

namespace ResumeManagerWebApi.Services.Documents
{
    public interface IDocumentsService
    {
        public Task<IEnumerable<DocumentBo>> GetAllDocuments(string userEmail);

        public Task<UploadDocumentResponse> Upload(IFormFile file, string userEmail);

        public Task<DownloadDocumentResponse> DownloadDocument(string fileName, string userEmail);

        public Task<DeleteDocumentResponse> Delete(string blobName, string userEmail);
    }
}
