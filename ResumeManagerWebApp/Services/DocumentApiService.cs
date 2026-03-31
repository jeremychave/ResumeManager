using ResumeManagerWebApp.Common;
using ResumeManagerWebApp.DTOs.Document;
using ResumeManagerWebApp.Models;

namespace ResumeManagerWebApp.Services
{
    public class DocumentApiService : IDocumentApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DocumentApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<Document>> GetAllDocumentsAsync(string userEmail)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/documents/{userEmail}");

            this.AddHttpHeader(request, userEmail);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseDto = await response.Content.ReadFromJsonAsync<GetAllDocumentsResponseDto>();

            if (responseDto.Documents != null) 
            {
                return responseDto.Documents.Select(dto => new Document
                {
                    BlobName = dto.BlobName,
                    FileName = dto.FileName,
                    Size = dto.Size
                }).ToList();
            }
            else
            {
                return new List<Document>();
            }
        }

        public async Task<UploadDocumentResponseDto> UploadDocumentAsync(IFormFile file, string userEmail)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/documents/upload");

            this.AddHttpHeader(request, userEmail);

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);
                content.Add(new StringContent(userEmail), "userEmail");
                request.Content = content;
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadFromJsonAsync<UploadDocumentResponseDto>();
            }
        }

        public async Task<DeleteDocumentResponseDto> DeleteDocumentAsync(string blobName)
        {
            var response = await _httpClient.DeleteAsync($"api/documents/{blobName}");

            return await response.Content.ReadFromJsonAsync<DeleteDocumentResponseDto>();
        }

        private void AddHttpHeader(HttpRequestMessage request, string userEmail)
        {
            var apiKey = _configuration["ApiSettings:ResumeManagerApiKey"];
            var secret = _configuration["ApiSettings:SignatureSecret"];

            var signature = HmacHelper.GenerateSignature(userEmail, secret);

            request.Headers.Add(AppConstants.HeaderHttpUserEmail, userEmail);
            request.Headers.Add(AppConstants.HeaderHttpApiKey, apiKey);
            request.Headers.Add(AppConstants.HeaderHttpUserSignature, signature);
        }
    }
}
