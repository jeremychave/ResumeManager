namespace ResumeManagerWebApp.DTOs.Document
{
    public class UploadDocumentRequestDto
    {
        public IFormFile File { get; set; }

        public string UserEmail { get; set; }
    }
}
