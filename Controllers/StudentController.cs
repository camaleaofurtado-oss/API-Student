using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos;
using ApiStudent.Service;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiStudent.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController : ControllerBase
{
    StudentService _studentService;
    public StudentController(StudentService service)
    {
        _studentService = service;
    }
    [HttpPost]
    public async Task<IActionResult> PostStudent([FromBody] CreateStudentDto studentDto)
    {
        await _studentService.AdicionarStudent(studentDto);
        return CreatedAtAction(nameof(ReadStudentPorId), new { Email = studentDto.Email}, studentDto);
    }
    [HttpGet("{email}")]
    public async Task<IActionResult> ReadStudentPorId(string email)
    {
        var studentDto = await _studentService.RetornarStudent(email);
        if(studentDto == null)
        {
            return NotFound();
        }
        return Ok(studentDto);
    }
    [HttpGet]
    public async Task<IEnumerable<ReadStudentDto>> ReadStudents()
    {
        return await _studentService.RetornarStudents();
    }
}
