namespace ResumeManagerWebApi.Controllers.Dtos.Document
{
    public class UploadDocumentRequestDto
    {
        public IFormFile File { get; set; }

        public string UserEmail { get; set; }
    }
}
