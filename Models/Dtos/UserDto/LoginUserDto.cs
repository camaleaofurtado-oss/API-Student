using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;

namespace ApiStudent.Models.Dtos.UserDto;

public record LoginUserDto
(
    [Required(ErrorMessage ="O email é obrigatorio")] string Email,
    [Required(ErrorMessage ="A senha é obrrigatoria")] string Password

);
