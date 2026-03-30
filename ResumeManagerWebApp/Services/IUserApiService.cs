using System.Security.Claims;

namespace ResumeManagerWebApp.Services
{
    public interface IUserApiService
    {
        Task SyncUserAsync(ClaimsPrincipal principal);
    }
}
