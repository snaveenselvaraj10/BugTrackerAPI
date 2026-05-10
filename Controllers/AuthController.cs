using BugTrackerAPI.DTOs;
using BugTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;


namespace BugTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        if (string.IsNullOrEmpty(dto.Email))
            return BadRequest("Email required");

        var userId = _service.GetUserIdByEmail(dto.Email);
        if (userId == null)
            return Unauthorized("User not found");

        var token = _service.GenerateToken(dto.Email, userId.Value);

        return Ok(new { token });
    }
}