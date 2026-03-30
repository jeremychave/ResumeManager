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

        public void SyncUser(string email)
        {
            var user = _userRepository.Get(email);

            if (user == null)
            {
                _userRepository.Create(email);
            }
        }
    }
}
