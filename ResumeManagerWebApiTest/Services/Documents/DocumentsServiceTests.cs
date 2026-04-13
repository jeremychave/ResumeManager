using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using ResumeManagerWebApi.Data.Entities;
using ResumeManagerWebApi.Data.Repositories;
using ResumeManagerWebApi.Services.Documents;
using ResumeManagerWebApi.Services.Documents.Responses;
using UserEntity = ResumeManagerWebApi.Data.Entities.User;

namespace ResumeManagerWebApiTest.Services.Documents
{
    public class DocumentsServiceTests
    {
        private DocumentsService documentsService;
        private Mock<IDocumentsRepository> documentsRepositoryMock;
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IDocumentsValidationService> documentsValidationServiceMock;

        [SetUp]
        public void Setup()
        {
            documentsRepositoryMock = new Mock<IDocumentsRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            documentsValidationServiceMock = new Mock<IDocumentsValidationService>();

            documentsService = new DocumentsService(
                documentsRepositoryMock.Object,
                userRepositoryMock.Object,
                documentsValidationServiceMock.Object);
        }

        [Test]
        public async Task GetAllDocuments_WhenUserIsNotFound_ShouldReturnEmptyList()
        {
            // Arrange
            var email = "test@example.com";
            this.userRepositoryMock.Setup(m => m.Get(email)).ReturnsAsync((UserEntity?)null);

            // Act
            var actualDocuments = await documentsService.GetAllDocuments(email);

            // Assert
            Assert.That(actualDocuments, Is.Empty);
            this.documentsRepositoryMock.Verify(m => m.GetUserDocuments(It.IsAny<Guid>()), Times.Never());
            this.documentsRepositoryMock.Verify(m => m.GetBlobProperties(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task GetAllDocuments_WhenUserIsFoundAndHasNoDocuments_ShouldReturnEmptyList()
        {
            // Arrange
            var email = "test@example.com";
            var userDocuments = new List<UserDocument>();
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = email,
                Documents = userDocuments
            };
            this.userRepositoryMock.Setup(m => m.Get(email)).ReturnsAsync(user);
            this.documentsRepositoryMock.Setup(m => m.GetUserDocuments(user.Id)).ReturnsAsync(userDocuments);

            // Act
            var actualDocuments = await documentsService.GetAllDocuments(email);

            // Assert
            Assert.That(actualDocuments, Is.Empty);
            this.documentsRepositoryMock.Verify(m => m.GetUserDocuments(user.Id), Times.Once());
            this.documentsRepositoryMock.Verify(m => m.GetBlobProperties(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task GetAllDocuments_WhenUserIsFoundAndHasDocuments_ShouldReturnDocumentBos()
        {
            // Arrange
            var email = "test@example.com";
            var userId = Guid.NewGuid();
            var userDocument1 = new UserDocument
            {
                Id = Guid.NewGuid(),
                BlobName = "blob1",
                FileName = "file1.txt",
                UserId = userId
            };
            var userDocument2 = new UserDocument
            {
                Id = Guid.NewGuid(),
                BlobName = "blob2",
                FileName = "file2.txt",
                UserId = userId
            };
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = email,
                Documents = new List<UserDocument> { userDocument1, userDocument2 }
            };
            this.userRepositoryMock.Setup(m => m.Get(email)).ReturnsAsync(user);
            this.documentsRepositoryMock
                .Setup(m => m.GetUserDocuments(user.Id))
                .ReturnsAsync(new List<UserDocument> { userDocument1, userDocument2 });

            var blobProperties1 = BlobsModelFactory.BlobProperties(contentLength: 100);
            var blobProperties2 = BlobsModelFactory.BlobProperties(contentLength: 200);

            this.documentsRepositoryMock
                .Setup(m => m.GetBlobProperties(userDocument1.BlobName))
                .ReturnsAsync(blobProperties1);
            this.documentsRepositoryMock
                .Setup(m => m.GetBlobProperties(userDocument2.BlobName))
                .ReturnsAsync(blobProperties2);

            // Act
            var actualDocuments = await documentsService.GetAllDocuments(email);

            // Assert
            Assert.That(actualDocuments.Count, Is.EqualTo(2));

            Assert.That(actualDocuments.ElementAt(0).Id, Is.EqualTo(userDocument1.Id));
            Assert.That(actualDocuments.ElementAt(0).UserId, Is.EqualTo(userId));
            Assert.That(actualDocuments.ElementAt(0).BlobName, Is.EqualTo(userDocument1.BlobName));
            Assert.That(actualDocuments.ElementAt(0).FileName, Is.EqualTo(userDocument1.FileName));
            Assert.That(actualDocuments.ElementAt(0).Size, Is.EqualTo(100));

            Assert.That(actualDocuments.ElementAt(1).Id, Is.EqualTo(userDocument2.Id));
            Assert.That(actualDocuments.ElementAt(1).UserId, Is.EqualTo(userId));
            Assert.That(actualDocuments.ElementAt(1).BlobName, Is.EqualTo(userDocument2.BlobName));
            Assert.That(actualDocuments.ElementAt(1).FileName, Is.EqualTo(userDocument2.FileName));
            Assert.That(actualDocuments.ElementAt(1).Size, Is.EqualTo(200));
        }

        [Test]
        public async Task Upload_WhenValidationFailed_ShouldReturnResponseWithErrors()
        {
            using (var stream = new MemoryStream())
            {
                // Arrange
                var file = new FormFile(stream, 0, 1, "file", "file.pdf");
                var email = "test@example.com";
                var error1 = "An error occured";
                var error2 = "An other error occured";
                var documentsValidationResponse = new DocumentsValidationResponse
                {
                    Errors = new List<string> { error1, error2 }
                };
                this.documentsValidationServiceMock.Setup(m => m.Validate(file)).Returns(documentsValidationResponse);

                // Act
                var actualResponse = await documentsService.Upload(file, email);

                // Assert
                Assert.That(actualResponse.Success, Is.False);
                Assert.That(actualResponse.FileName, Is.Null);
                Assert.That(actualResponse.Errors, Is.Not.Null);
                Assert.That(actualResponse.Errors.Count, Is.EqualTo(2));
                Assert.That(actualResponse.Errors[0], Is.EqualTo(error1));
                Assert.That(actualResponse.Errors[1], Is.EqualTo(error2));
            }
        }

        [Test]
        public async Task Upload_WhenNoValidationErrorsAndDocumentNotFound_ShouldUploadBlobAndAddUserDocument()
        {
            using (var stream = new MemoryStream())
            {
                // Arrange
                var file = new FormFile(stream, 0, 1, "file2", "file2.pdf");
                var email = "test@example.com";
                var documentsValidationResponse = new DocumentsValidationResponse
                {
                    Errors = new List<string>()
                };

                this.documentsValidationServiceMock.Setup(m => m.Validate(file)).Returns(documentsValidationResponse);

                var userId = Guid.NewGuid();
                var userDocument = new UserDocument()
                {
                    Id = Guid.NewGuid(),
                    BlobName = "blob1",
                    FileName = "file1.txt",
                    UserId = userId
                };
                var user = new UserEntity
                {
                    Id = userId,
                    Email = email,
                    Documents = new List<UserDocument> { userDocument }
                };

                this.userRepositoryMock.Setup(m => m.Get(email)).ReturnsAsync(user);
                this.documentsRepositoryMock
                    .Setup(m => m.GetUserDocuments(userId))
                    .ReturnsAsync(new List<UserDocument>() { userDocument });

                var expectedBlobName = "BlobNameFile2.pdf";
                this.documentsRepositoryMock.Setup(m => m.UploadBlob(file, null)).ReturnsAsync(expectedBlobName);

                // Act
                var actualResponse = await documentsService.Upload(file, email);

                // Assert
                Assert.That(actualResponse.Success, Is.True);
                Assert.That(actualResponse.FileName, Is.EqualTo(file.FileName));
                Assert.That(actualResponse.Errors, Is.Null);
                this.documentsRepositoryMock.Verify(m => m.UploadBlob(file, null), Times.Once());
                this.documentsRepositoryMock.Verify(m => m.AddUserDocument(userId, expectedBlobName, file.FileName), Times.Once());
            }
        }

        [Test]
        public async Task Upload_WhenNoValidationErrorsAndDocumentFound_ShouldUploadBlob()
        {
            using (var stream = new MemoryStream())
            {
                // Arrange
                var file = new FormFile(stream, 0, 1, "file1", "file1.pdf");
                var email = "test@example.com";
                var documentsValidationResponse = new DocumentsValidationResponse
                {
                    Errors = new List<string>()
                };

                this.documentsValidationServiceMock.Setup(m => m.Validate(file)).Returns(documentsValidationResponse);

                var userId = Guid.NewGuid();
                var userDocument = new UserDocument()
                {
                    Id = Guid.NewGuid(),
                    BlobName = "blob1",
                    FileName = file.FileName,
                    UserId = userId
                };
                var user = new UserEntity
                {
                    Id = userId,
                    Email = email,
                    Documents = new List<UserDocument> { userDocument }
                };

                this.userRepositoryMock.Setup(m => m.Get(email)).ReturnsAsync(user);
                this.documentsRepositoryMock
                    .Setup(m => m.GetUserDocuments(userId))
                    .ReturnsAsync(new List<UserDocument>() { userDocument });

                // Act
                var actualResponse = await documentsService.Upload(file, email);

                // Assert
                Assert.That(actualResponse.Success, Is.True);
                Assert.That(actualResponse.FileName, Is.EqualTo(file.FileName));
                Assert.That(actualResponse.Errors, Is.Null);
                this.documentsRepositoryMock.Verify(m => m.UploadBlob(file, userDocument.BlobName), Times.Once());
                this.documentsRepositoryMock.Verify(m => m.AddUserDocument(
                    It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            }
        }

        [Test]
        public async Task DownloadDocument_WhenDocumentNotFound_ShouldReturnResponseWithError()
        {
            // Arrange
            var fileName = "file1.pdf";
            var email = "test@example.com";
            this.documentsRepositoryMock.Setup(m => m.GetUserDocument(email, fileName)).ReturnsAsync((UserDocument?)null);

            // Act
            var actualResponse = await documentsService.DownloadDocument(fileName, email);

            // Assert
            Assert.That(actualResponse.Success, Is.False);
            Assert.That(actualResponse.Content, Is.Null);
            Assert.That(actualResponse.Error, Is.EqualTo("Document not found"));
        }

        [Test]
        public async Task DownloadDocument_WhenDocumentFound_ShouldReturnResponseWithContent()
        {
            using (var stream = new MemoryStream())
            {
                // Arrange
                var fileName = "file1.pdf";
                var email = "test@example.com";
                var userDocument = new UserDocument { BlobName = "blob1" };
                this.documentsRepositoryMock.Setup(m => m.GetUserDocument(email, fileName)).ReturnsAsync(userDocument);
                this.documentsRepositoryMock.Setup(m => m.Download(userDocument.BlobName)).ReturnsAsync(stream);

                // Act
                var actualResponse = await documentsService.DownloadDocument(fileName, email);

                // Assert
                Assert.That(actualResponse.Success, Is.True);
                Assert.That(actualResponse.Content, Is.EqualTo(stream));
                Assert.That(actualResponse.Error, Is.Null);
            }
        }

        [Test]
        public async Task Delete_WhenDeleteBlobReturnError_ShouldReturnResponseWithError()
        {
            // Arrange
            var blobName = "file1.pdf";
            var email = "test@example.com";
            var userId = Guid.NewGuid();
            var userDocument = new UserDocument()
            {
                Id = Guid.NewGuid(),
                BlobName = blobName,
                FileName = "file1.pdf",
                UserId = userId
            };
            var user = new UserEntity
            {
                Id = userId,
                Email = email,
                Documents = new List<UserDocument> { userDocument }
            };

            this.userRepositoryMock.Setup(m => m.Get(email)).ReturnsAsync(user);
            this.documentsRepositoryMock.Setup(m => m.DeleteBlob(blobName)).ReturnsAsync(false);

            // Act
            var actualResponse = await documentsService.Delete(blobName, email);

            // Assert
            Assert.That(actualResponse.Success, Is.False);
            Assert.That(actualResponse.BlobName, Is.EqualTo(blobName));
            Assert.That(actualResponse.Errors, Is.Not.Null);
            Assert.That(actualResponse.Errors.Count, Is.EqualTo(1));
            Assert.That(actualResponse.Errors[0], Is.EqualTo($"Failed to delete document"));
        }

        [Test]
        public async Task Delete_WhenDeleteBlobReturnNoError_ShouldReturnResponseWithNoError()
        {
            // Arrange
            var blobName = "file1.pdf";
            var email = "test@example.com";
            var userId = Guid.NewGuid();
            var userDocument = new UserDocument()
            {
                Id = Guid.NewGuid(),
                BlobName = blobName,
                FileName = "file1.pdf",
                UserId = userId
            };
            var user = new UserEntity
            {
                Id = userId,
                Email = email,
                Documents = new List<UserDocument> { userDocument }
            };

            this.userRepositoryMock.Setup(m => m.Get(email)).ReturnsAsync(user);
            this.documentsRepositoryMock.Setup(m => m.DeleteBlob(blobName)).ReturnsAsync(true);

            // Act
            var actualResponse = await documentsService.Delete(blobName, email);

            // Assert
            Assert.That(actualResponse.Success, Is.True);
            Assert.That(actualResponse.BlobName, Is.EqualTo(blobName));
            Assert.That(actualResponse.Errors, Is.Null);
            this.documentsRepositoryMock.Verify(m => m.DeleteUserDocument(userId, blobName), Times.Once());
            this.documentsRepositoryMock.Verify(m => m.DeleteBlob(blobName), Times.Once());
        }
    }
}
