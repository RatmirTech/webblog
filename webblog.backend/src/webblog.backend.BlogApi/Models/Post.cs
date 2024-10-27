namespace webblog.backend.BlogApi.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Alias { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public DateTime DatePosted { get; set; }
        public PostContent Content { get; set; } = null!;
    }
}
