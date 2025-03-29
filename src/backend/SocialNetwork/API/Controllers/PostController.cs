using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto postDto)
        {
            var result = await _postService.Create(postDto);
            
            if (result.IsFailed)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtAction(nameof(GetPost), new { id = result.Value.Id }, result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(Guid id)
        {
            var post = await _postService.GetById(id);
            
            if (post.IsFailed)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPost("{postId}/likes")]
        public async Task<IActionResult> AddLike(Guid postId)
        {
            var result = await _postService.AddLike(postId);
            
            if (result.IsFailed)
            {
                if (result.HasError(error => error.Message == "Not found"))
                {
                    return NotFound();
                }
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpDelete("{postId}/likes")]
        public async Task<IActionResult> UnLike(Guid postId)
        {
            var result = await _postService.UnLike(postId);
            
            if (result.IsFailed)
            {
                if (result.HasError(error => error.Message == "Not found"))
                {
                    return NotFound();
                }
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            var result = await _postService.Delete(postId);
            
            if (result.IsFailed)
            {
                if (result.HasError(error => error.Message == "Not found"))
                {
                    return NotFound();
                }
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpPost("{postId}/comments")]
        public async Task<IActionResult> AddComment(Guid postId, [FromBody] CreateCommentDto commentDto)
        {
            commentDto.PostId = postId; // Ensure the postId matches the route
            
            var result = await _postService.AddComment(postId, commentDto);
            
            if (result.IsFailed)
            {
                if (result.HasError(error => error.Message == "Not found"))
                {
                    return NotFound();
                }
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpDelete("{postId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid postId, Guid commentId)
        {
            var result = await _postService.DeleteComment(postId, commentId);
            
            if (result.IsFailed)
            {
                if (result.HasError(error => error.Message == "Not found"))
                {
                    return NotFound();
                }
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}