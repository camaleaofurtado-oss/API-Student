
using System.ComponentModel.DataAnnotations;

namespace ApiStudent.Models.Dtos.InstructorDto;

public record CreateInstructorDto(  
    [Required(ErrorMessage = "O email é obrigatorio")] string Email,
    [Required(ErrorMessage = " O nome é obrigatorio")] string Nome
);

