namespace ApiStudent.Models.Dtos.UserDto;

public record ReadUserDto
{
    public string Id { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = new();
}
