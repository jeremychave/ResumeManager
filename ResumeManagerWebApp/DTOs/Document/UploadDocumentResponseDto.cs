namespace ResumeManagerWebApp.DTOs.Document
{
    public class UploadDocumentResponseDto
    {
        public bool Success { get; set; }

        public string? FileName { get; set; }

        public List<string>? Errors { get; set; }
    }
}
