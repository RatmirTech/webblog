namespace webblog.backend.BlogApi.DtoModels
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Alias { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public DateTime DatePosted { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PostBody { get; set; } = string.Empty;
    }
}
