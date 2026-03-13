namespace ResumeManagerWebApi.Repositories
{
    public interface IDocumentsRepository
    {
        public Task<string> Upload(IFormFile file);

        public Task<Stream> Download(string blobName);

        public Task Delete(string blobName);
    }
}
