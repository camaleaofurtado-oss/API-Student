using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos;
using ApiStudent.Models.Dtos.StudentDto;
using ApiStudent.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ApiStudent.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy ="AdminOnly")]
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
        await _studentService.AdicionarStudentAsync(studentDto);
        return CreatedAtAction(nameof(ReadStudentPorId), new { Email = studentDto.Email}, studentDto);
    }
    [HttpGet("{email}")]
    [Authorize(Policy = "IsStudent")]
    [AllowAnonymous]
    public async Task<IActionResult> ReadStudentPorId(string email)
    {
        var studentDto = await _studentService.RetornarStudentAsync(email);
        if(studentDto == null)
        {
            return NotFound();
        }
        return Ok(studentDto);
    }
    [HttpGet]
    public async Task<IEnumerable<ReadStudentDto>> ReadStudents()
    {
        return await _studentService.RetornarStudentsAsync();
    }
    [HttpPut("{email}")]
    public async Task<IActionResult> PutStudent(string email, [FromBody] UpdateStudentDto studentDto)
    {
        string mensagem = await _studentService.AtualizarStudentAsync(email, studentDto);
        if (string.Empty == mensagem) return NotFound();
        return Ok(mensagem);
    }
    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteStudent(string email)
    {
        bool studentRemovido = await _studentService.RemoverStudentAsync(email);
        if(!studentRemovido)
        {
            return NotFound();
        }
        return NoContent();

    }



}
