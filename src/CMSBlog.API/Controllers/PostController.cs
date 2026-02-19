using AutoMapper;
using CMSBlog.Core.Domain.Content;
using CMSBlog.Core.Models.Content;
using CMSBlog.Core.SeedWorks;
using Microsoft.AspNetCore.Mvc;

namespace CMSBlog.API.Controllers
{
    [Route("api/admin/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PostController(ILogger<PostController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var postDto = _mapper.Map<PostDto>(post);
            return Ok(postDto);
        }

        [HttpGet]
        [Route("paging")]
        public async Task<IActionResult> GetPostPaging(string? keyword, Guid? categoryId, int pageIndex, int pageSize)
        {
            var pagedPosts = await _unitOfWork.PostRepository.GetPostsPagingAsync(keyword, categoryId, pageIndex, pageSize);
            return Ok(pagedPosts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreateUpdatePostRequest request)
        {
            var post = _mapper.Map<Post>(request);
            _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost([FromRoute] Guid id, [FromBody] CreateUpdatePostRequest request)
        {
            var existingPost = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }
            _mapper.Map(request, existingPost);
            _unitOfWork.PostRepository.Update(existingPost);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var existingPost = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }
            _unitOfWork.PostRepository.Remove(existingPost);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}