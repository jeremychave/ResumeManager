using ResumeManagerWebApi.Data.Entities;

namespace ResumeManagerWebApi.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ResumeManagerDbContext _context;

        public UserRepository(ResumeManagerDbContext context)
        {
            _context = context;
        }

        public User Create(string email)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email
            };

            _context.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Delete(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null) 
            {
                throw new KeyNotFoundException($"User with email {email} does not exist !");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
