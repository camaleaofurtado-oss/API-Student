using ApiStudent.Models.Dtos.EnrollmentDto;

namespace ApiStudent.Models.Dtos;

public record ReadStudentDto(
    
  string Email,
  string Nome, 
  string DataCadastro,
  ICollection<ReadEnrollmentDto> Enrollments
);

