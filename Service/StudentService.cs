using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos;
using ApiStudent.Models.Dtos.StudentDto;
using ApiStudent.Models.Dtos.UserDto;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace ApiStudent.Service;

public class StudentService
{
    IMapper _mapper;
    Context _context;
    UserManager<User> _userManager;
    IHttpContextAccessor _httpContextAcessor;
    public StudentService(IMapper mapper, Context context, UserManager<User> userManager, IHttpContextAccessor httpContext)
    {
        _mapper = mapper;
        _context = context;
        _userManager = userManager;
        _httpContextAcessor = httpContext;
    }
    public async Task<Student> AdicionarStudentAsync(CreateStudentDto studentDto)
    {
        var student = await _context.Students.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Email == studentDto.Email);
        if (student != null)
        {
            student.IsDeleted = false;
            await _context.SaveChangesAsync();

            return student;
        }

        var user = await _userManager.FindByEmailAsync(studentDto.Email);
        if (user == null) throw new ApplicationException("Usuario não existe!");

        bool studantExiste = await _context.Students.AnyAsync(s=> s.UserId == user.Id);
        if (studantExiste) throw new ApplicationException("ja tem um estudante vinculado com esse usuario!");

        student = _mapper.Map<Student>(studentDto);
        student.UserId = user.Id;

        await _context.AddAsync(student);
        await _context.SaveChangesAsync();
        return student;
    }
    public async Task<ReadStudentDto> RetornarStudentAsync(string email)
    {
        var userClaim = _httpContextAcessor.HttpContext?.User;
        var emailClaim = userClaim!.FindFirst(ClaimTypes.Email)?.Value;
        bool isAdmin = userClaim.IsInRole("SuperAdmin") || userClaim.IsInRole("Admin");
        Student? student;
        if(!isAdmin && emailClaim != email)
        {
            throw new ApplicationException("Email passado pelo endpoint diferente do email do estudante");
        }
        student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
        var studentDto = _mapper.Map<ReadStudentDto>(student);
        return studentDto;

    }
    public async Task<IEnumerable<ReadStudentDto>> RetornarStudentsAsync()
    {
        return _mapper.Map<List<ReadStudentDto>>(await _context.Students.ToListAsync());
    }
    public async Task<string> AtualizarStudentAsync(string email,UpdateStudentDto studentDto)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s=> s.Email == email);
        if (student == null) return "";
        _mapper.Map(studentDto, student);
        await _context.SaveChangesAsync();
        return "Usuario alterado";
    }
    public async Task<bool> RemoverStudentAsync(string email)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
        if (student == null)
        {
            return false;
        }
        student.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;

    }
        



}
