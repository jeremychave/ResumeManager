namespace ResumeManagerWebApi.Services.Documents.Bo
{
    public class DocumentBo
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string BlobName { get; set; }

        public string FileName { get; set; }

        public long Size { get; set; }
    }
}
