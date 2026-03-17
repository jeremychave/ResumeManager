namespace ResumeManagerWebApi.Services.Documents.Responses
{
    public record UploadDocumentResponse
    {
        public bool Success { get; init; }
        
        public string? BlobName { get; init; }

        public List<string>? Errors { get; init; }
    }
}
