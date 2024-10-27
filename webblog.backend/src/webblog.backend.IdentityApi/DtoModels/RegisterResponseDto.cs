namespace webblog.backend.IdentityApi.DtoModels
{
    public class RegisterResponseDto
    {
        public bool Success { get; set; }
        public string? UserId { get; set; }
        public List<string>? Errors { get; set; }
    }
}
