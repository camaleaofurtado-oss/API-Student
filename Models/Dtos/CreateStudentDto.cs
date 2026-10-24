using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace ApiStudent.Models.Dtos;

public record CreateStudentDto
(
    [Required(ErrorMessage = "O campo Email é obrigatorio")]
    [StringLength(300, ErrorMessage = "O campos Email não pode exceder 300 caracteres")]
    string Email,
    [Required(ErrorMessage = "O campo Nome é obrigatorio")]
    [StringLength(100, ErrorMessage = "O campos Nome não pode exceder 100 caracteres")]
    string Nome
);
