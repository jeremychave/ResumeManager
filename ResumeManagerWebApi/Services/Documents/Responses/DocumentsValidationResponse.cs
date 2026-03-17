namespace ResumeManagerWebApi.Services.Documents.Responses
{
    public record DocumentsValidationResponse
    {
        public List<string>? Errors { get; init; }

        public bool IsValid 
        { 
            get { return Errors == null || Errors.Count == 0; } 
        }
    }
}
