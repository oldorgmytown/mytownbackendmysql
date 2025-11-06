using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mytown.Models.mytown.DataAccess;
using mytown.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }
    private readonly AppDbContext _context;

    public TokenService(AppDbContext context)
    {
        _context = context;
    }

    public string GenerateToken(int userId, string email, string role, string sessionId)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role),
        new Claim("SessionId", sessionId) 
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8), // keep your chosen expiry
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task<bool> InvalidateSessionAsync(string sessionId)
    {
        var session = await _context.UserSessions
            .FirstOrDefaultAsync(s => s.SessionGuid == sessionId);

        if (session == null)
            return false;

        _context.UserSessions.Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }

}
