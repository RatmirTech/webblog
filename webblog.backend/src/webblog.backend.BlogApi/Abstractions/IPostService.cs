using webblog.backend.BlogApi.DtoModels;

namespace webblog.backend.BlogApi.Abstractions
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetAllPostsAsync();
        Task<PostDto?> GetPostByIdAsync(Guid id);
        Task<PostDto> CreatePostAsync(CreatePostDto createPostDto);
        Task<PostDto?> UpdatePostAsync(Guid id, UpdatePostDto updatePostDto);
        Task<bool> DeletePostAsync(Guid id);
    }
}
