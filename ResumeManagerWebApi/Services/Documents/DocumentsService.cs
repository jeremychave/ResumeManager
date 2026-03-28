using ResumeManagerWebApi.Data.Repositories;
using ResumeManagerWebApi.Dtos;
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
                    FileName = null,
                    Success = false,
                    Errors = documentsValidationResponse.Errors
                };
            }

            var blobName = await _documentRepository.Upload(file);

            return new UploadDocumentResponse
            {
                FileName = file.FileName,
                Success = true,
                Errors = null
            };
        }

        public async Task<Stream> Download(string blobName)
        {
            return await _documentRepository.Download(blobName);
        }

        public async Task<DeleteDocumentResponse> Delete(string blobName)
        {
            var success = await _documentRepository.Delete(blobName);

            return new DeleteDocumentResponse
            {
                Success = success,
                BlobName = blobName,
                Errors = success ? null : new List<string> { $"Failed to delete document" }
            };
        }
    }
}
