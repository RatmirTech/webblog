using Microsoft.AspNetCore.Mvc;
using webblog.backend.BlogApi.Abstractions;
using webblog.backend.BlogApi.DtoModels;

namespace webblog.backend.BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IKafkaProducerService _kafkaProducerService;

        public PostsController(IPostService postService, IKafkaProducerService kafkaProducerService)
        {
            _postService = postService;
            _kafkaProducerService = kafkaProducerService;
        }

        /// <summary>
        /// Получение всех постов.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<PostDto>> GetAll()
        {
            return await _postService.GetAllPostsAsync();
        }

        /// <summary>
        /// Получение поста по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор поста.</param>
        [HttpGet("{id}")]
        public async Task<PostDto?> GetById(Guid id)
        {
            return await _postService.GetPostByIdAsync(id);
        }

        /// <summary>
        /// Создание нового поста.
        /// </summary>
        /// <param name="createPostDto">Данные для создания поста.</param>
        [HttpPost]
        public async Task<PostDto> Create([FromBody] CreatePostDto createPostDto)
        {
            var post = await _postService.CreatePostAsync(createPostDto);
            await _kafkaProducerService.ProduceAsync("blog-posts", new { Action = "Create", Data = post });
            return post;
        }

        /// <summary>
        /// Обновление существующего поста.
        /// </summary>
        /// <param name="id">Идентификатор поста.</param>
        /// <param name="updatePostDto">Данные для обновления поста.</param>
        [HttpPut("{id}")]
        public async Task<PostDto?> Update(Guid id, [FromBody] UpdatePostDto updatePostDto)
        {
            var post = await _postService.UpdatePostAsync(id, updatePostDto);
            if (post != null)
            {
                await _kafkaProducerService.ProduceAsync("blog-posts", new { Action = "Update", Data = post });
            }
            return post;
        }

        /// <summary>
        /// Удаление поста по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор поста.</param>
        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            var success = await _postService.DeletePostAsync(id);
            if (success)
            {
                await _kafkaProducerService.ProduceAsync("blog-posts", new { Action = "Delete", PostId = id });
            }
            return success;
        }
    }
}
