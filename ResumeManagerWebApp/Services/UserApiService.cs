using ResumeManagerWebApp.DTOs.User;
using System.Security.Claims;

namespace ResumeManagerWebApp.Services
{
    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SyncUserAsync(ClaimsPrincipal principal)
        {
            var userEmail = principal.FindFirst("preferred_username")?.Value;

            if (userEmail == null)
            {
                return;
            }

            var payload = new SyncUserRequestDto
            {
                Email = userEmail
            };

            var response = await _httpClient.PostAsJsonAsync("api/user/sync", payload);
        }
    }
}
