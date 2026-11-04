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
    

}
