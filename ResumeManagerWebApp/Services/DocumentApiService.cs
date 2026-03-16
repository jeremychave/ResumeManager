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
    }
}
