using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Dapper;
using Microsoft.Data.SqlClient;


namespace BugTrackerAPI.Services;
public class AuthService
{
    private readonly IConfiguration _config;

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public int? GetUserIdByEmail(string email)
    {
        var connStr = _config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connStr)) return null;

        using var connection = new SqlConnection(connStr);
        return connection.QuerySingleOrDefault<int?>(
            "SELECT UserId FROM Users WHERE Email = @Email",
            new { Email = email }
        );
    }

    public string GenerateToken(string email, int userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("UserId", userId.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                Convert.ToDouble(_config["Jwt:DurationInMinutes"])
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}