namespace ApiStudent.Models;

public class Enrollment
{
    public int CourseId { get; set; }
    public virtual Course Course { get; set; }
    public string StudentEmail { get; set; }
    public virtual Student Student { get; set; }


}
