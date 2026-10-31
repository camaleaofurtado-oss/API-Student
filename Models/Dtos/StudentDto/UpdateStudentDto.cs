using System.ComponentModel.DataAnnotations;

namespace ApiStudent.Models.Dtos.StudentDto;

public record UpdateStudentDto
(
  [Required] string Nome  
);
