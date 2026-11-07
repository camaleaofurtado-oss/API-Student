using ApiStudent.Models.Dtos.EnrollmentDto;
using ApiStudent.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiStudent.Controllers;

[ApiController]
[Route("[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly EnrollmentService _enrollmentService;
    public EnrollmentController(EnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }
    [HttpPost]
    [Authorize(Policy = "IsStudent")]
    public async Task<IActionResult> PostEnrollment(CreateEnrollmentDto enrollmentDto)
    {
        try
        {
            var enrollmentReadDto = await _enrollmentService.AdicionarEnrollmentAsync(enrollmentDto);
            return CreatedAtAction(nameof(ReadEnrollmentPorId), new { studentEmail = enrollmentReadDto.StudentEmail, courseId = enrollmentReadDto.CourseId }, enrollmentReadDto);

        }catch(ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpGet("{studentEmail}/{courseId}")]
    [Authorize(Policy="AdminOnly")]
    public async Task<IActionResult>ReadEnrollmentPorId(string studentEmail, int courseId)
    {
        var enrollmentDto = await _enrollmentService.RetornarEnrollmentPorIdAsync(studentEmail, courseId);
        if (enrollmentDto == null) return NotFound();
        return Ok(enrollmentDto);
    }

}
