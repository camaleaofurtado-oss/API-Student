using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos.CourseDto;
using ApiStudent.Models.Enum;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ApiStudent.Service;

public class CourseService
{
    IMapper _mapper;
    Context _context;
    public CourseService( Context context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }
    public async Task<Course?> CriarCourseAsync(CreateCourseDto courseDto)
    {
       int valorCategoria=(int)courseDto.Categoria;
        if (validaEnum(valorCategoria))
        {
            return null;
        }

        var course = _mapper.Map<Course>(courseDto);
        await _context.AddAsync(course);
        await _context.SaveChangesAsync();
        return course;
    }
    public async Task<ReadCourseDto?> RetornarCoursePorIdAsync(int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c=> c.Id == id);
        var courseDto = _mapper.Map<ReadCourseDto>(course);
        return courseDto;

    }
    public async Task<IEnumerable<ReadCourseDto>> RetornarCoursesAsync(string? categoria, int? cargaMinima, DateTime? dataMinima)
    {
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrEmpty(categoria))
        {
            if (Enum.TryParse<CategoriaCurso>(categoria,true, out var categoriaEnum))
            {
                query = query.Where(c => c.Categoria == categoriaEnum);
            }
            else
            {
                return Enumerable.Empty<ReadCourseDto>();
            }
        }
        if(cargaMinima is not null)
        {
            query = query.Where(c => c.CargaHoraria >= cargaMinima);   
        }
        if( dataMinima is not null)
        {
            query = query.Where(c => c.DataCriacao >= dataMinima);
        }
        var listaAsync = await query.ToListAsync();
        return _mapper.Map<List<ReadCourseDto>>(listaAsync);
    }
    public async Task<int?> AtualizaCourseAsync(int id, UpdateCourseDto courseDto)
    {
        int valorCategoria = (int)courseDto.Categoria;
        if (validaEnum(valorCategoria))
        {
            throw new ArgumentException("A categoria informada é inválida.");
        }
        var course =await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if(course == null)
        {
            throw new KeyNotFoundException("Curso não encontrado.");
        }
        _mapper.Map(courseDto, course);
        await _context.SaveChangesAsync();
        return course.Id;
    }
    public async Task<int?> DeletarCourseAsync(int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c=> c.Id == id);
        if (course is null) return null;
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return course.Id;
    }

    public bool validaEnum(int valorCategoriaDto )
    {
        int totalValues = Enum.GetValues(typeof(CategoriaCurso)).Length;
        return valorCategoriaDto >= totalValues || valorCategoriaDto < 0;
    }
}
