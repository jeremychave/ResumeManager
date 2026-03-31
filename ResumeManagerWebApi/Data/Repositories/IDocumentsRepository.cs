using Azure.Storage.Blobs.Models;
using ResumeManagerWebApi.Data.Entities;

namespace ResumeManagerWebApi.Data.Repositories
{
    public interface IDocumentsRepository
    {
        public Task<IEnumerable<UserDocument>> GetUserDocuments(Guid userId);
        public Task<UserDocument> AddUserDocument(Guid userId, string blobName, string fileName);

        public Task<BlobProperties> GetBlobProperties(string blobName);
        public Task<string> UploadBlob(IFormFile file);
        public Task<Stream> Download(string blobName);
        public Task<bool> Delete(string blobName);
    }
}
