namespace ApiStudent.Models.Dtos.EnrollmentDto;

public record CreateEnrollmentDto
(
    string? StudentEmail,
    int CourseId
);
