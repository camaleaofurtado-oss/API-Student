using ApiStudent.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace ApiStudent.Models;

public class Course
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string Descricao { get; set; }
    [Required]
    public CategoriaCurso Categoria { get; set; }
    [Required]
    public int CargaHoraria { get; set; }
    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Enrollment> Enrollments { get; set; }

}
