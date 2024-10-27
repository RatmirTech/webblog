namespace webblog.backend.IdentityApi.DtoModels
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string? UserId { get; set; }
        public List<string>? Errors { get; set; }
    }
}
