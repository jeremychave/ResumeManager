using ResumeManagerWebApp.Models;

namespace ResumeManagerWebApp.Services
{
    public class ResumeApiService
    {
        private readonly HttpClient _httpClient;

        public ResumeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Resume>> GetAllResumeAsync()
        {
            var response = await _httpClient.GetAsync("api/documents");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Resume>>();
        }
    }
}
