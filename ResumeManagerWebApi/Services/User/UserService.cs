using ResumeManagerWebApi.Data.Repositories;

namespace ResumeManagerWebApi.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task SyncUser(string email)
        {
            var user = await _userRepository.Get(email);

            if (user == null)
            {
                await _userRepository.Create(email);
            }
        }
    }
}
