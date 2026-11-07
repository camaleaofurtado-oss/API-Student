using ApiStudent.Data;
using ApiStudent.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiStudent.Service;

public class TokenService
{
    private readonly UserManager<User> _userManager;
    private readonly Context _context;
    private readonly IConfiguration _configuration;

    public TokenService(UserManager<User> userManager, Context context, IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim("id", user.Id ),
            new Claim("username",user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!)
        };
        foreach(var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        if(_context.Students.Any(s=> s.UserId == user.Id))
        {
            claims.Add(new Claim("UserType", "Student"));
        }
        if(_context.Instructors.Any(i=> i.UserId == user.Id))
        {
            claims.Add(new Claim("UserType", "Instructor"));
        }

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SymmetricSecurityKey"]!));

        var signCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddMinutes(10),
            claims: claims,
            signingCredentials: signCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

}
