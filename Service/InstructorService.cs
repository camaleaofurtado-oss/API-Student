using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos.InstructorDto;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiStudent.Service;

public class InstructorService
{
    private readonly UserManager<User> _userManager;
    private readonly Context _context;
    private readonly IMapper _mapper;

    public InstructorService(UserManager<User> userManager, Context context, IMapper mapper)
    {
        _userManager = userManager;
        _context = context;
        _mapper = mapper;
    }

    public async Task AdicionarInstructorAsync(CreateInstructorDto instructorDto)
    {
        var user = await _userManager.FindByEmailAsync(instructorDto.Email);
        if (user == null) throw new ApplicationException("Usuario não existe!");

        bool instructorExist = await _context.Instructors.AnyAsync(i=> i.UserId == user.Id);
        if (instructorExist) throw new ApplicationException("Ja existe um usuario vinculado a esse instrutor");

        var instructor = _mapper.Map<Instructor>(instructorDto);
        instructor.UserId = user.Id;

        await _context.AddAsync(instructor);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> RemoverInstructorAsync(string email)
    {
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i=> i.Email == email);
        if (instructor == null) return false;
        _context.Remove(instructor);
        await _context.SaveChangesAsync();
        return true;
    }
}
