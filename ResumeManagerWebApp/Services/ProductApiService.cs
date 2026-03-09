using ResumeManagerWebApp.Models;

namespace ResumeManagerWebApp.Services
{
    public class ProductApiService
    {
        private readonly HttpClient _httpClient;

        public ProductApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync("api/products");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Product>>();
        }
    }
}
