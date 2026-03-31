namespace ResumeManagerWebApi.Controllers.Dtos
{
    public class GetAllDocumentsResponseDto
    {
        public IEnumerable<DocumentDto> Documents { get; set; }
    }
}
