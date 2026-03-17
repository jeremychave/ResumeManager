namespace ResumeManagerWebApp.DTOs
{
    public class DeleteDocumentResponseDto
    {
        public bool Success { get; set; }

        public string? FileName { get; set; }

        public List<string>? Errors { get; set; }
    }
}
