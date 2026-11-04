using ApiStudent.Models;
using ApiStudent.Models.Dtos.UserDto;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace ApiStudent.Service;

public class UserService
{
    IMapper _mapper;
    UserManager<User> _userManager;
    SignInManager<User> _signManager;
    RoleManager<IdentityRole> _roleManager;
    TokenService _tokenService;
    public UserService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signManager, TokenService tokenService, RoleManager<IdentityRole> roleManager)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signManager = signManager;
        _tokenService = tokenService;
        _roleManager = roleManager;
    }


    public async Task<User> CadastraAsync(CreateUserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        var resultado = await _userManager.CreateAsync(user, userDto.Password);
        if (!resultado.Succeeded)
        {
            var erros = string.Join("; ", resultado.Errors.Select(e => e.Description));
            throw new ApplicationException($"Falha ao cadastrar usuario: {erros}");
        }
        return user;
    }
    public async Task<ReadUserDto> LerUserPorIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null) throw new ApplicationException("Usuario não encontrado");

        var userDto = _mapper.Map<ReadUserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);

        return userDto with { Roles = roles.ToList() };

    }
    public async Task<IEnumerable<ReadUserDto>> retornarUsuariosAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        var userDtos = new List<ReadUserDto>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            var userDto = _mapper.Map<ReadUserDto>(u);
            userDtos.Add(userDto with { Roles = roles.ToList() });
        }

        return userDtos;
    }
    public async Task<AuthResponseDto> LoginAsync(LoginUserDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return new AuthResponseDto( false, "Email ou senha invalidos");

        var results = await _signManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
        if(!results.Succeeded) return new AuthResponseDto(false, "Email ou senha invalidos");

        var token = await _tokenService.GenerateTokenAsync(user);
        return new AuthResponseDto(true, "Login realizado com sucesso", token);
        
        
    }
    public async Task AdicionaUserRoleAsync(AddUserRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null) throw new ApplicationException("Usuario não existe!");

        var roles = await _roleManager.Roles.Select(r=> r.Name).ToListAsync();
        
        foreach(var role in dto.Roles)
        {
            if(!roles.Contains(role)) throw new ApplicationException("Role não existe!");
        }
        await _userManager.AddToRolesAsync(user, dto.Roles);

    }

}
