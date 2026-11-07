using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos.EnrollmentDto;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiStudent.Service;

public class EnrollmentService
{
    private readonly IMapper _mapper;
    private readonly Context _context;
    private readonly IHttpContextAccessor _httpContextAcessor;
    public EnrollmentService(IMapper mapper, Context context, IHttpContextAccessor httpContextAcessor)
    {
        _mapper = mapper;
        _context = context;
        _httpContextAcessor = httpContextAcessor;
    }
    public async Task<ReadEnrollmentDto> AdicionarEnrollmentAsync(CreateEnrollmentDto enrollmentDto)
    {
        var userClaim = _httpContextAcessor.HttpContext!.User;
        bool IsAdmin = userClaim.IsInRole("Admin") || userClaim.IsInRole("SuperAdmin");
        Student? student = null;

        if ( !IsAdmin && !string.IsNullOrEmpty(enrollmentDto.StudentEmail))
        {
            throw new ApplicationException("Um estudante não deve passar seu email no corpo da requisição");
        }else if(!IsAdmin)
        {
            var userId = userClaim.FindFirst("id")?.Value;
            student =await _context.Students.FirstOrDefaultAsync(s=> s.UserId == userId);
            
        }else if(!string.IsNullOrEmpty(enrollmentDto.StudentEmail))
        {
            student = await _context.Students.FirstOrDefaultAsync(s => s.Email == enrollmentDto.StudentEmail);
        }
        if(student == null)
        {
            throw new KeyNotFoundException("Estudante não foi encontrado");
        }

        var course = await _context.Courses.FirstOrDefaultAsync(c=> c.Id == enrollmentDto.CourseId);
        if(course == null)
        {
            throw new KeyNotFoundException("Curso não foi encontrado");
        }

        var enrollment = _mapper.Map<Enrollment>(enrollmentDto);
        if(string.IsNullOrEmpty(enrollment.StudentEmail))
        {
            enrollment.StudentEmail = student.Email;
        }
        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();

        var enrollmentReadDto = _mapper.Map<ReadEnrollmentDto>(enrollment);
        return enrollmentReadDto;
    }
    public async Task<ReadEnrollmentDto> RetornarEnrollmentPorIdAsync(string studentEmail, int courseId)
    {
        var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e=> e.StudentEmail == studentEmail && e.CourseId == courseId);
        var enrollmentDto = _mapper.Map<ReadEnrollmentDto>(enrollment);
        return enrollmentDto;

    }

}
