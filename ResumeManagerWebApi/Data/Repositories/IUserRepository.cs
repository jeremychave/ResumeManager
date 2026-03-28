using ResumeManagerWebApi.Data.Entities;

namespace ResumeManagerWebApi.Data.Repositories
{
    public interface IUserRepository
    {
        User Create(string email);

        void Delete(string email);
    }
}
