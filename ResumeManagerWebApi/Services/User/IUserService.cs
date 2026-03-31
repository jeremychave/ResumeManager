namespace ResumeManagerWebApi.Services.User
{
    public interface IUserService
    {
        Task SyncUser(string email);
    }
}
