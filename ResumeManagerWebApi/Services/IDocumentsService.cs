using ResumeManagerWebApi.Models;

namespace ResumeManagerWebApi.Services
{
    public interface IDocumentsService
    {
        public Task<IEnumerable<Document>> GetAllDocuments();

        public Task<string> Upload(IFormFile file);

        public Task<Stream> Download(string blobName);

        public Task Delete(string blobName);
    }
}
