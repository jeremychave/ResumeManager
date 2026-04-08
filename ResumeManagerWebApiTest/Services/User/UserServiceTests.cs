using Moq;
using ResumeManagerWebApi.Data.Repositories;
using ResumeManagerWebApi.Services.User;

using UserEntity = ResumeManagerWebApi.Data.Entities.User;

namespace ResumeManagerWebApiTest.Services.User
{
    public class UserServiceTests
    {
        private UserService _userService;

        private Mock<IUserRepository> userRepositoryMock;

        [SetUp]
        public void Setup()
        {
            userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(userRepositoryMock.Object);
        }

        [Test]
        public async Task SyncUser_WhenCalled_ShouldGetUser()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            await _userService.SyncUser(email);

            // Assert
            this.userRepositoryMock.Verify(m => m.Get(email), Times.Once);
        }

        [Test]
        public async Task SyncUser_WhenUserExists_ShouldNotCreateUser()
        {
            // Arrange
            var email = "test@example.com";
            var existingUser = new UserEntity { Email = email };
            this.userRepositoryMock.Setup(m => m.Get(email)).ReturnsAsync(existingUser);

            // Act
            await _userService.SyncUser(email);

            // Assert
            this.userRepositoryMock.Verify(m => m.Create(email), Times.Never);
        }

        [Test]
        public async Task SyncUser_WhenUserDoesNotExists_ShouldCreateUser()
        {
            // Arrange
            var email = "test@example.com";
            UserEntity? existingUser = null;
            this.userRepositoryMock.Setup(m => m.Get(email)).ReturnsAsync(existingUser);

            // Act
            await _userService.SyncUser(email);

            // Assert
            this.userRepositoryMock.Verify(m => m.Create(email), Times.Once);
        }
    }
}