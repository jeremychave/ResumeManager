using Azure.Storage.Blobs.Models;
using ResumeManagerWebApi.Data.Entities;

namespace ResumeManagerWebApi.Data.Repositories
{
    public interface IDocumentsRepository
    {
        public Task<IEnumerable<UserDocument>> GetUserDocuments(Guid userId);
        public Task<UserDocument?> GetUserDocument(string userEmail, string fileName);
        public Task<UserDocument> AddUserDocument(Guid userId, string blobName, string fileName);
        public Task DeleteUserDocument(Guid userId, string blobName);

        public Task<BlobProperties> GetBlobProperties(string blobName);
        public Task<string> UploadBlob(IFormFile file, string? existingBlobName = null);
        public Task<Stream> Download(string blobName);
        public Task<bool> DeleteBlob(string blobName);
    }
}
