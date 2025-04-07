namespace study_buddys_backend_v2.Models.DTOS
{
    public class PasswordDTO
    {
        public string? Salt { get; set; }
        public string? Hash { get; set; }
    }
}