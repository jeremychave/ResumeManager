namespace ResumeManagerWebApp.DTOs
{
    public class UploadDocumentResponseDto
    {
        public bool Success { get; set; }

        public string? BlobName { get; set; }

        public List<string>? Errors { get; set; }
    }
}
