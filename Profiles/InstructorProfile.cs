using ApiStudent.Models;
using ApiStudent.Models.Dtos.InstructorDto;
using AutoMapper;

namespace ApiStudent.Profiles;

public class InstructorProfile : Profile 
{
    public InstructorProfile()
    {
        CreateMap<CreateInstructorDto, Instructor>();
    }

}
