using BugTrackerAPI.DTOs;
using BugTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BugTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BugController : ControllerBase
    {
        private readonly BugService _service;

        public BugController(BugService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create(CreateBugDto dto)
        {
            _service.CreateBug(dto);
            return Ok("Bug Created Successfully");
        }
        

        [HttpGet]
        public IActionResult GetAll()
        {
            var bugs = _service.GetAllBugs();
            return Ok(bugs);
        }

        [HttpPut("status")]
        public IActionResult UpdateStatus(int bugId, string status)
        {
            _service.UpdateStatus(bugId, status);
            return Ok("Status Updated");
        }

        [HttpGet("open-count")]
        public IActionResult GetOpenCount(int userId)
        {
            var count = _service.GetOpenBugCount(userId);
            return Ok(count);
        }
    }
}