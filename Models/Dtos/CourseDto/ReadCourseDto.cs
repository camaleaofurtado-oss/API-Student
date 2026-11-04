using System.ComponentModel.DataAnnotations;

namespace ApiStudent.Models.Dtos.CourseDto;

public record ReadCourseDto
(
     int Id,
     string Descricao,
     string Categoria,
     int cargaHoraria,
     DateTime DataCriacao
);
