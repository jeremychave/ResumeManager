using ResumeManagerWebApi.Data.Repositories;
using ResumeManagerWebApi.Services.Documents.Bo;
using ResumeManagerWebApi.Services.Documents.Responses;

namespace ResumeManagerWebApi.Services.Documents
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IDocumentsRepository _documentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDocumentsValidationService _documentsValidationService;

        public DocumentsService(
            IDocumentsRepository documentRepository,
            IUserRepository userRepository,
            IDocumentsValidationService documentsValidationService)
        {
            _documentRepository = documentRepository;
            _userRepository = userRepository;
            _documentsValidationService = documentsValidationService;
        }

        public async Task<IEnumerable<DocumentBo>> GetAllDocuments(string userEmail)
        {
            var documentBos = new List<DocumentBo>();

            var user = await _userRepository.Get(userEmail);

            if(user != null)
            {
                var userDocuments = await _documentRepository.GetUserDocuments(user.Id);

                foreach (var userDocument in userDocuments)
                {
                    var blobSize = (await _documentRepository.GetBlobProperties(userDocument.BlobName)).ContentLength;

                    documentBos.Add(new DocumentBo
                    {
                        Id = userDocument.Id,
                        UserId = userDocument.UserId,
                        BlobName = userDocument.BlobName,
                        FileName = userDocument.FileName,
                        Size = blobSize
                    });
                }
            }

            return documentBos;
        }

        public async Task<UploadDocumentResponse> Upload(IFormFile file, string userEmail)
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

            var blobName = await _documentRepository.UploadBlob(file);
            var user = await _userRepository.Get(userEmail);
            await _documentRepository.AddUserDocument(user.Id, blobName, file.FileName);

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

        public async Task<DeleteDocumentResponse> Delete(string blobName, string userEmail)
        {
            var user = await _userRepository.Get(userEmail);

            await _documentRepository.DeleteUserDocument(user.Id, blobName);
            var success = await _documentRepository.DeleteBlob(blobName);

            return new DeleteDocumentResponse
            {
                Success = success,
                BlobName = blobName,
                Errors = success ? null : new List<string> { $"Failed to delete document" }
            };
        }
    }
}
