using ResumeManagerWebApi.Models;
using ResumeManagerWebApi.Repositories;
using ResumeManagerWebApi.Services.Documents.Responses;

namespace ResumeManagerWebApi.Services.Documents
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IDocumentsRepository _documentRepository;
        private readonly IDocumentsValidationService _documentsValidationService;

        public DocumentsService(
            IDocumentsRepository documentRepository,
            IDocumentsValidationService documentsValidationService)
        {
            _documentRepository = documentRepository;
            _documentsValidationService = documentsValidationService;
        }

        public async Task<IEnumerable<Document>> GetAllDocuments()
        {
            return await _documentRepository.GetAllDocuments();
        }

        public async Task<UploadDocumentResponse> Upload(IFormFile file)
        {
            var documentsValidationResponse = _documentsValidationService.Validate(file);

            if (!documentsValidationResponse.IsValid)
            {
                return new UploadDocumentResponse
                {
                    BlobName = null,
                    Success = false,
                    Errors = documentsValidationResponse.Errors
                };
            }

            var blobName = await _documentRepository.Upload(file);

            return new UploadDocumentResponse
            {
                BlobName = blobName,
                Success = true,
                Errors = null
            };
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
