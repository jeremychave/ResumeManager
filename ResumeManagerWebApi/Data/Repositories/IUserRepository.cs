using ResumeManagerWebApi.Data.Entities;

namespace ResumeManagerWebApi.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User?> Get(string email);

        Task<User> Create(string email);

        Task Delete(string email);
    }
}
