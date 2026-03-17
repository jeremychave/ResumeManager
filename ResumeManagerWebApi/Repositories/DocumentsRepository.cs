using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ResumeManagerWebApi.Models;

namespace ResumeManagerWebApi.Repositories
{
    public class DocumentsRepository : IDocumentsRepository
    {
        private readonly BlobContainerClient _container;

        public DocumentsRepository(IConfiguration config)
        {
            var storageUrl = config["AzureStorage:BlobServiceUrl"];
            var containerName = config["AzureStorage:ContainerName"];

            var credential = new DefaultAzureCredential();
            var serviceClient = new BlobServiceClient(new Uri(storageUrl), credential);

            _container = serviceClient.GetBlobContainerClient(containerName);
        }

        public async Task<IEnumerable<Document>> GetAllDocuments()
        {
            var documents = new List<Document>();
            await foreach (var blobItem in _container.GetBlobsAsync())
            {
                var document = new Document
                {
                    BlobName = blobItem.Name,
                    FileName = blobItem.Metadata.TryGetValue("FileName", out var name) ? name : blobItem.Name,
                    Size = blobItem.Properties.ContentLength ?? 0
                };

                documents.Add(document);
            }

            return documents;
        }

        public async Task<string> Upload(IFormFile file)
        {
            var blob = _container.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));

            var metadata = new Dictionary<string, string>
            {
                { "FileName", file.FileName }
            };

            using (var stream = file.OpenReadStream()) 
            {
                await blob.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = file.ContentType },
                metadata: metadata);
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
