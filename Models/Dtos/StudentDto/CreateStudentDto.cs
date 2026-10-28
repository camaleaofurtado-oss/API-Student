using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace ApiStudent.Models.Dtos.StudentDto;

public record CreateStudentDto
(
    [Required(ErrorMessage = "O email é obrigatorio")]string Email,
    [Required(ErrorMessage =" O nome é obrigatorio")] string Nome
);
