using System.ComponentModel.DataAnnotations;

namespace ApiStudent.Models;

public class Student
{
    [Key]
    [Required]
    public string Email { get; set; }
    [Required]
    public string Nome { get; set; }
    [Required]
    public DateTime DataCadastro { get; set; }
    [Required]
    public string UserId { get; set; }
    public virtual ICollection<Enrollment> Enrollments { get; set; }

}
