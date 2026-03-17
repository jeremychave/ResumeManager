using ResumeManagerWebApp.DTOs;
using ResumeManagerWebApp.Models;

namespace ResumeManagerWebApp.Services
{
    public class DocumentApiService
    {
        private readonly HttpClient _httpClient;

        public DocumentApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Document>> GetAllDocumentsAsync()
        {
            var response = await _httpClient.GetAsync("api/documents");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Document>>();
        }

        public async Task<UploadDocumentResponseDto> UploadDocumentAsync(IFormFile file)
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

                var response = await _httpClient.PostAsync("api/documents/upload", content);

                return await response.Content.ReadFromJsonAsync<UploadDocumentResponseDto>();
            }
        }
    }
}
