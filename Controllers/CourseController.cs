using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos.CourseDto;
using ApiStudent.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ApiStudent.Controllers;


[ApiController]
[Route("[controller]")]
public class CourseController : ControllerBase
{
    CourseService _courseService;
    public CourseController(CourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpPost]
    [Authorize(Policy = "IsInstructor")]
    public async Task<IActionResult> PostCourse([FromBody] CreateCourseDto courseDto)
    {
        var course = await _courseService.CriarCourseAsync(courseDto);
        if(course == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(ReadCoursePorId), new { Id = course.Id }, course);

    }
    [HttpGet("{id}")]
    public async Task<IActionResult> ReadCoursePorId(int id)
    {
        var courseDto = await _courseService.RetornarCoursePorIdAsync(id);
        if (courseDto == null) return NotFound();
        return Ok(courseDto);
    }
    [HttpGet]
    public async Task<IEnumerable<ReadCourseDto>> ReadCourse([FromQuery] DateTime? dataMinima, [FromQuery]int? cargaMinima, [FromQuery]string categoria = "" )
    {
        return await _courseService.RetornarCoursesAsync(categoria,cargaMinima,dataMinima);
    }
    [HttpPut("{id}")]
    [Authorize(Policy = "IsInstructor")]
    public async Task<IActionResult> PutCourse(int id, [FromBody] UpdateCourseDto courseDto)
    {
        try
        {
            var courseId = await _courseService.AtualizaCourseAsync(id, courseDto);
            return Ok($"Curso com Id {courseId} atualizado com sucesso!");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); 
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message); 
        }
    }
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult>DeleteCourse(int id)
    {
        var courseId = await _courseService.DeletarCourseAsync(id);
        if (courseId == null) return NotFound();
        return NoContent();

    }

    

}
