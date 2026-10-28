using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos;
using ApiStudent.Models.Dtos.StudentDto;
using ApiStudent.Models.Dtos.UserDto;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace ApiStudent.Service;

public class StudentService
{
    IMapper _mapper;
    Context _context;
    UserManager<User> _userManager;
    public StudentService(IMapper mapper, Context context, UserManager<User> userManager)
    {
        _mapper = mapper;
        _context = context;
        _userManager = userManager;
    }
    public async Task AdicionarStudentAsync(CreateStudentDto studentDto)
    {
        var user = await _userManager.FindByEmailAsync(studentDto.Email);
        if (user == null) throw new ApplicationException("Usuario não existe!");

        bool studantExiste = await _context.Students.AnyAsync(s=> s.UserId == user.Id);
        if (studantExiste) throw new ApplicationException("ja tem um estudante vinculado com esse usuario!");

        var student = _mapper.Map<Student>(studentDto);
        student.UserId = user.Id;

        await _context.AddAsync(student);
        await _context.SaveChangesAsync();
    }
    public async Task<ReadStudentDto> RetornarStudentAsync(string email)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s=> s.Email == email);
        var studentDto = _mapper.Map<ReadStudentDto>(student);
        return studentDto;

    }
    public async Task<IEnumerable<ReadStudentDto>> RetornarStudentsAsync()
    {
        return _mapper.Map<List<ReadStudentDto>>(await _context.Students.ToListAsync());
    }


}
