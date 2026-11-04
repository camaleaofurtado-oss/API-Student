using ApiStudent.Models.Dtos.InstructorDto;
using ApiStudent.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiStudent.Controllers;



[ApiController]
[Route("[controller]")]
[Authorize(Policy = "AdminOnly")]
public class InstructorController : ControllerBase
{
    InstructorService _instructorService;
    public InstructorController(InstructorService instructorService)
    {
        _instructorService = instructorService;
    }
    [HttpPost("")]
    public async Task<IActionResult> PostInstructor([FromBody] CreateInstructorDto instructorDto)
    {
        await _instructorService.AdicionarInstructorAsync(instructorDto);
        return Ok("Instrutor adicionado com sucesso");
    }
    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteInstructor(string email)
    {
        var instructorRemovido = await _instructorService.RemoverInstructorAsync(email);
        if (!instructorRemovido) return NotFound();
        return NoContent();

    }

}
