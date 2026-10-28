using ApiStudent.Models;
using ApiStudent.Models.Dtos.UserDto;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ApiStudent.Service;

public class UserService
{
    IMapper _mapper;
    UserManager<User> _userManager;
    SignInManager<User> _signManager;
    TokenService _tokenService;
    public UserService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signManager, TokenService tokenService)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signManager = signManager;
        _tokenService = tokenService;
    }


    public async Task<User> CadastraAsync(CreateUserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        var resultado = await _userManager.CreateAsync(user, userDto.Password);
        if (!resultado.Succeeded)
        {
            throw new ApplicationException($"Falha ao cadastrar");

        }
        return user;
    }
    public async Task<ReadUserDto> LerUserPorIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var userDto = _mapper.Map<ReadUserDto>(user);
        return userDto;

    }
    public async Task<IEnumerable<ReadUserDto>> retornarUsuariosAsync()
    {
        return _mapper.Map<List<ReadUserDto>>(await _userManager.Users.ToListAsync());
        
    }
    public async Task<AuthResponseDto> Login(LoginUserDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return new AuthResponseDto( false, "Email ou senha invalidos");

        var results = await _signManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
        if(!results.Succeeded) return new AuthResponseDto(false, "Email ou senha invalidos");

        var token = await _tokenService.GenerateTokenAsync(user);
        return new AuthResponseDto(true, "Login realizado com sucesso", token);
        
        
    }

}
