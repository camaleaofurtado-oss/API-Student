using System.ComponentModel.DataAnnotations;

namespace ApiStudent.Models.Dtos.UserDto;

public record CreateUserDto
{
    [Required(ErrorMessage = "O UserName é obrigatório.")]
    public string UserName { get; init; } = string.Empty;

    [Required(ErrorMessage = "O Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage = "A confirmação é obrigatória.")]
    [Compare("Password", ErrorMessage = "As senhas não conferem.")]
    public string RePassword { get; init; } = string.Empty;
}
