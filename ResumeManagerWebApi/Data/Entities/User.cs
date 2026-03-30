namespace ResumeManagerWebApi.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public List<UserDocument> Documents { get; set; } = new();
    }
}
