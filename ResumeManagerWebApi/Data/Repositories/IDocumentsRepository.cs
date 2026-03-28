using ResumeManagerWebApi.Dtos;

namespace ResumeManagerWebApi.Data.Repositories
{
    public interface IDocumentsRepository
    {
        public Task<IEnumerable<Document>> GetAllDocuments();

        public Task<string> Upload(IFormFile file);

        public Task<Stream> Download(string blobName);

        public Task<bool> Delete(string blobName);
    }
}
