namespace ResumeManagerWebApi.Services.Documents.Responses
{
    public record DownloadDocumentResponse
    {
        public bool Success { get; init; }

        public Stream? Content { get; init; }

        public string? Error { get; init; }
    }
}
