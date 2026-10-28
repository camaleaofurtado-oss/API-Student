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
    [Required]
    public DateTime DataNascimento { get; set; }
    [Required]
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    [Required]
    public float Pontuacao { get; set; } = 0;
    
    public virtual ICollection<Enrollment> Enrollments { get; set; }

}
