using Microsoft.AspNetCore.Http;
using ResumeManagerWebApi.Services.Documents;

namespace ResumeManagerWebApiTest.Services.Documents
{
    public class DocumentsValidationServiceTests
    {
        private DocumentsValidationService _service;

        [SetUp]
        public void Setup()
        {
            _service = new DocumentsValidationService();
        }

        [Test]
        public void Validate_WhenFileIsNull_ShouldReturnErrors()
        {
            // Arrange
            FormFile? file = null;

            // Act
            var actualResponse = _service.Validate(file);

            // Assert
            Assert.That(actualResponse.IsValid, Is.False);
            Assert.That(actualResponse.Errors.Count, Is.EqualTo(1));
            Assert.That(actualResponse.Errors[0], Is.EqualTo("Please select a file."));
        }

        [Test]
        public void Validate_WhenFileLengthIsZero_ShouldReturnErrors()
        {
            using (var stream = new MemoryStream())
            {
                // Arrange
                var lenght = 0;
                var file = new FormFile(stream, 0, lenght, "file", "file.pdf");

                // Act
                var actualResponse = _service.Validate(file);

                // Assert
                Assert.That(actualResponse.IsValid, Is.False);
                Assert.That(actualResponse.Errors.Count, Is.EqualTo(1));
                Assert.That(actualResponse.Errors[0], Is.EqualTo("Please select a file."));
            }
        }

        [Test]
        public void Validate_WhenFileLengthIsGreaterThanMaxBytes_ShouldReturnErrors()
        {
            using (var stream = new MemoryStream())
            {
                // Arrange
                var maxBytes = 5L * 1024 * 1024;
                var lenght = maxBytes + 1;
                var file = new FormFile(stream, 0, lenght, "file", "file.pdf");

                // Act
                var actualResponse = _service.Validate(file);

                // Assert
                Assert.That(actualResponse.IsValid, Is.False);
                Assert.That(actualResponse.Errors.Count, Is.EqualTo(1));
                Assert.That(actualResponse.Errors[0], Is.EqualTo("File size must not exceed 5 MB."));
            }
        }

        [TestCase(".dok")]
        [TestCase(".txt")]
        [TestCase(".cs")]
        public void Validate_WhenFileExtensionIsNotValid_ShouldReturnErrors(string extension)
        {
            using (var stream = new MemoryStream())
            {
                // Arrange
                var file = new FormFile(stream, 0, 1, "file", "file" + extension);

                // Act
                var actualResponse = _service.Validate(file);

                // Assert
                Assert.That(actualResponse.IsValid, Is.False);
                Assert.That(actualResponse.Errors.Count, Is.EqualTo(1));
                Assert.That(actualResponse.Errors[0], Is.EqualTo($"{extension} is not a valid format file"));
            }
        }

        [Test]
        public void Validate_WhenFileLengthIsGreaterThanMaxBytesAndFileExtensionIsNotValid_ShouldReturnErrors()
        {
            using (var stream = new MemoryStream())
            {
                // Arrange
                var maxBytes = 5L * 1024 * 1024;
                var invalidExtension = ".txt";
                var lenght = maxBytes + 1;
                var file = new FormFile(stream, 0, lenght, "file", "file" + invalidExtension);

                // Act
                var actualResponse = _service.Validate(file);

                // Assert
                Assert.That(actualResponse.IsValid, Is.False);
                Assert.That(actualResponse.Errors.Count, Is.EqualTo(2));
                Assert.That(actualResponse.Errors[0], Is.EqualTo("File size must not exceed 5 MB."));
                Assert.That(actualResponse.Errors[1], Is.EqualTo($"{invalidExtension} is not a valid format file"));
            }
        }

        [TestCase(".doc")]
        [TestCase(".docx")]
        [TestCase(".pdf")]
        public void Validate_WhenFileIsValid_ShouldReturnNoErrors(string extension)
        {
            using (var stream = new MemoryStream())
            {
                // Arrange
                var file = new FormFile(stream, 0, 1, "file", "file" + extension);

                // Act
                var actualResponse = _service.Validate(file);

                // Assert
                Assert.That(actualResponse.IsValid, Is.True);
                Assert.That(actualResponse.Errors.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void FailingTest_ToTestFailingPipeline()
        {
            Assert.That(false, Is.True);
        }
    }
}
