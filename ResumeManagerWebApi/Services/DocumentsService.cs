using ResumeManagerWebApi.Models;
using ResumeManagerWebApi.Repositories;

namespace ResumeManagerWebApi.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IDocumentsRepository _documentRepository;

        public DocumentsService(IDocumentsRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<IEnumerable<Document>> GetAllDocuments()
        {
            return await _documentRepository.GetAllDocuments();
        }

        public async Task<string> Upload(IFormFile file)
        {
            return await _documentRepository.Upload(file);
        }

        public async Task<Stream> Download(string blobName)
        {
            return await _documentRepository.Download(blobName);
        }

        public async Task Delete(string blobName)
        {
            await _documentRepository.Delete(blobName);
        }
    }
}
