using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace ApiStudent.Service;

public class StudentService
{
    IMapper _mapper;
    Context _context;
    public StudentService(IMapper mapper, Context context)
    {
        _mapper = mapper;
        _context = context;
    }
    public async Task AdicionarStudent(CreateStudentDto studentDto)
    {
        var student = _mapper.Map<Student>(studentDto);
        await _context.AddAsync(student);
        await _context.SaveChangesAsync ();
    }
    public async Task<ReadStudentDto> RetornarStudent(string email)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s=> s.Email == email);
        var studentDto = _mapper.Map<ReadStudentDto>(student);
        return studentDto;

    }
    public async Task<IEnumerable<ReadStudentDto>> RetornarStudents()
    {
        return _mapper.Map<List<ReadStudentDto>>(await _context.Students.ToListAsync());
    }


}
