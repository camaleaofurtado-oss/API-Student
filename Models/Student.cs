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
    public string UserId { get; set; }
    [Required]
    public virtual User User { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Enrollment> Enrollments { get; set; }

}
