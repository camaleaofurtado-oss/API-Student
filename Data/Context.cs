using ApiStudent.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiStudent.Data;

public class Context : IdentityDbContext<User>
{
    public Context (DbContextOptions<Context>opts) : base(opts)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Enrollment>().HasKey(e => new { e.StudentId, e.CourseId });
        builder.Entity<Enrollment>().HasOne(e => e.Student).WithMany(s => s.Enrollments).HasForeignKey(e => e.StudentId);
        builder.Entity<Enrollment>().HasOne(e => e.Course).WithMany(c => c.Enrollments).HasForeignKey(e => e.CourseId);

        builder.Entity<Student>().HasMany(s => s.Enrollments).WithOne(e => e.Student).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Course>().HasMany(c => c.Enrollments).WithOne(e => e.Course).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Student>().HasQueryFilter(s => !s.IsDeleted);
                
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Instructor> Instructors { get; set; }


}
