using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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
            _container.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<string> Upload(IFormFile file)
        {
            var blob = _container.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));

            using var stream = file.OpenReadStream();
            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            return blob.Name;
        }

        public async Task<Stream> Download(string blobName)
        {
            var blob = _container.GetBlobClient(blobName);
            var response = await blob.DownloadAsync();
            return response.Value.Content;
        }

        public async Task Delete(string blobName)
        {
            var blob = _container.GetBlobClient(blobName);
            await blob.DeleteIfExistsAsync();
        }
    }
}
