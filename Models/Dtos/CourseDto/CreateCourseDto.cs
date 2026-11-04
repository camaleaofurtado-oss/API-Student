using ApiStudent.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace ApiStudent.Models.Dtos.CourseDto;

public record CreateCourseDto
(
    [Required(ErrorMessage = "A descrição é obrigatoria")] string Descricao,
    [Required(ErrorMessage = "A categoria é obrigatoria")]CategoriaCurso Categoria,
    [Required(ErrorMessage = "A carga horaria é obrigatoria")] int cargaHoraria
);
