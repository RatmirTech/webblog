namespace webblog.backend.BlogApi.Models
{
    public class PostContent
    {
        public Guid PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PostBody { get; set; } = string.Empty;
        public Post Post { get; set; } = null!;
    }
}
