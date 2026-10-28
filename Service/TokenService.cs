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
    UserManager<User> _userManager;

    public TokenService(UserManager<User> userManager)
    {
        _userManager = userManager;
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
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(("sua-chave-gigante-com-mais-de-trinta-e-dois-caracteres")));

        var signCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddMinutes(10),
            claims: claims,
            signingCredentials: signCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

}
