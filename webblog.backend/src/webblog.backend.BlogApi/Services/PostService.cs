using Microsoft.EntityFrameworkCore;
using webblog.backend.BlogApi.Abstractions;
using webblog.backend.BlogApi.Data;
using webblog.backend.BlogApi.DtoModels;
using webblog.backend.BlogApi.Models;

namespace webblog.backend.BlogApi.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IKafkaProducerService _kafkaProducerService;
        private const string KafkaTopic = "blog-posts";

        public PostService(ApplicationDbContext context, IKafkaProducerService kafkaProducerService)
        {
            _context = context;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
        {
            var posts = await _context.Posts
                .Include(p => p.Content)
                .ToListAsync();

            return posts.Select(p => new PostDto
            {
                Id = p.Id,
                Alias = p.Alias,
                AuthorId = p.AuthorId,
                DatePosted = p.DatePosted,
                Title = p.Content.Title,
                PostBody = p.Content.PostBody
            });
        }

        public async Task<PostDto?> GetPostByIdAsync(Guid postId)
        {
            var post = await _context.Posts
                .Include(p => p.Content)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return null;

            return new PostDto
            {
                Id = post.Id,
                Alias = post.Alias,
                AuthorId = post.AuthorId,
                DatePosted = post.DatePosted,
                Title = post.Content.Title,
                PostBody = post.Content.PostBody
            };
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto)
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Alias = createPostDto.Alias,
                AuthorId = createPostDto.AuthorId,
                DatePosted = DateTime.UtcNow,
                Content = new PostContent
                {
                    Title = createPostDto.Title,
                    PostBody = createPostDto.PostBody
                }
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var postDto = new PostDto
            {
                Id = post.Id,
                Alias = post.Alias,
                AuthorId = post.AuthorId,
                DatePosted = post.DatePosted,
                Title = post.Content.Title,
                PostBody = post.Content.PostBody
            };

            await _kafkaProducerService.ProduceAsync(KafkaTopic, new
            {
                Action = "Create",
                Data = postDto
            });

            return postDto;
        }

        public async Task<PostDto?> UpdatePostAsync(Guid postId, UpdatePostDto updatePostDto)
        {
            var post = await _context.Posts
                .Include(p => p.Content)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return null;

            post.Content.Title = updatePostDto.Title;
            post.Content.PostBody = updatePostDto.PostBody;

            await _context.SaveChangesAsync();

            var updatedPostDto = new PostDto
            {
                Id = post.Id,
                Alias = post.Alias,
                AuthorId = post.AuthorId,
                DatePosted = post.DatePosted,
                Title = post.Content.Title,
                PostBody = post.Content.PostBody
            };

            await _kafkaProducerService.ProduceAsync(KafkaTopic, new
            {
                Action = "Update",
                Data = updatedPostDto
            });

            return updatedPostDto;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            await _kafkaProducerService.ProduceAsync(KafkaTopic, new
            {
                Action = "Delete",
                Data = new { Id = postId }
            });

            return true;
        }
    }
}
