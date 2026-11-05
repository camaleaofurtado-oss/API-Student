using ApiStudent.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace ApiStudent.Models.Dtos.CourseDto
{
    public record UpdateCourseDto
    (
        [Required(ErrorMessage = "A descrição é obrigatoria")] string Descricao,
        [Required(ErrorMessage = "A categoria é obrigatoria")] CategoriaCurso Categoria,
        [Required(ErrorMessage = "A carga horaria é obrigatoria")][Range(0, int.MaxValue, ErrorMessage = "O valor deve ser positivo")] int cargaHoraria

    );
}
