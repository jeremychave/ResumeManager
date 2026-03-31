using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ResumeManagerWebApi.Data.Entities;

namespace ResumeManagerWebApi.Data.Repositories
{
    public class DocumentsRepository : IDocumentsRepository
    {
        private readonly BlobContainerClient _container;
        private readonly ResumeManagerDbContext _context;

        public DocumentsRepository(IConfiguration config, ResumeManagerDbContext context)
        {
            var storageUrl = config["AzureStorage:BlobServiceUrl"];
            var containerName = config["AzureStorage:ContainerName"];

            var credential = new DefaultAzureCredential();
            var serviceClient = new BlobServiceClient(new Uri(storageUrl), credential);

            _container = serviceClient.GetBlobContainerClient(containerName);
            _context = context;
        }

        public async Task<IEnumerable<UserDocument>> GetUserDocuments(Guid userId)
        {
            return _context.UserDocument
                .Where(ud => ud.UserId == userId)
                .ToList();
        }

        public async Task<UserDocument> AddUserDocument(Guid userId, string blobName, string fileName)
        {
            var newDocument = new UserDocument
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BlobName = blobName,
                FileName = fileName
            };

            _context.UserDocument.Add(newDocument);
            _context.SaveChanges();

            return newDocument;
        }

        public async Task<BlobProperties> GetBlobProperties(string blobName)
        {
            return await _container.GetBlobClient(blobName).GetPropertiesAsync();
        }

        public async Task<string> UploadBlob(IFormFile file)
        {
            var blob = _container.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));

            using (var stream = file.OpenReadStream()) 
            {
                await blob.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = file.ContentType });
            };

            return blob.Name;
        }

        public async Task<Stream> Download(string blobName)
        {
            var blob = _container.GetBlobClient(blobName);
            var response = await blob.DownloadAsync();
            return response.Value.Content;
        }

        public async Task<bool> Delete(string blobName)
        {
            var blob = _container.GetBlobClient(blobName);
            return await blob.DeleteIfExistsAsync();
        }
    }
}
