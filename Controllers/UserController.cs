using ApiStudent.Models.Dtos.UserDto;
using ApiStudent.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApiStudent.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles ="SuperAdmin")]
public class UserController : ControllerBase
{
    UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("Cadastra")]
    [AllowAnonymous]
    public async Task<IActionResult> PostUser([FromBody] CreateUserDto userDto)
    {
        var user = await _userService.CadastraAsync(userDto);
        return CreatedAtAction(nameof(ReadUserPorId), new { user.Id }, user);
    }
    [HttpPost("Role")]
    public async Task<IActionResult> PostRoleEmUser([FromBody] AddUserRoleDto dto)
    {
        await _userService.AdicionaUserRoleAsync(dto);
        return Ok("Roles adicionadas ao usuario!");

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ReadUserPorId(string id)
    {
        var userDto = await _userService.LerUserPorIdAsync(id);
        if(userDto == null)
        {
            return NotFound();
        }
        return Ok(userDto);
    }
    [HttpGet]
    public async Task<IEnumerable<ReadUserDto>>ReadUsers()
    {
        return await _userService.retornarUsuariosAsync();

    }
    [HttpPost("Login")]
    [AllowAnonymous]
   public async Task<IActionResult> Login([FromBody]LoginUserDto loginDto)
   {
        var responseDto = await _userService.Login(loginDto);
        if(!responseDto.Success)
        {
            return BadRequest(new {Message = responseDto.Message});
        }

        return Ok(responseDto);

   }


}
