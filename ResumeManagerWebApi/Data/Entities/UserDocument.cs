namespace ResumeManagerWebApi.Data.Entities
{
    public class UserDocument
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string BlobName { get; set; }

        public string FileName { get; set; }
    }
}
