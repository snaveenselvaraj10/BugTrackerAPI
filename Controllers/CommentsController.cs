using BugTrackerAPI.DTOs;
using BugTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BugTrackerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService _service;

        public CommentsController(CommentService service)
        {
            _service = service;
        }

        // POST /api/comments (create comment) - expects CreateCommentDto
        [HttpPost]
        public IActionResult Create([FromBody] CreateCommentDto dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest("UserId claim missing from token");
            }

            var created = _service.CreateComment(dto, userId);
            return Ok(created);
        }

        // GET /api/comments/bug/{bugId}?page=1&pageSize=20
        [HttpGet("bug/{bugId}")]
        public IActionResult GetByBug(int bugId, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var (items, total) = _service.GetCommentsByBug(bugId, page, pageSize);
            return Ok(new { items, total, page, pageSize });
        }

        // PUT /api/comments/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateCommentDto dto)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("UserId claim missing");

            _service.UpdateComment(id, dto.CommentText, userId);
            return Ok("Comment updated");
        }

        // DELETE /api/comments/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("UserId claim missing");

            _service.DeleteComment(id, userId);
            return Ok("Comment deleted");
        }
    }
}
