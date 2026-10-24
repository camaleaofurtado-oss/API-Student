using ApiStudent.Models;
using ApiStudent.Models.Dtos;
using AutoMapper;

namespace ApiStudent.Profiles;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<CreateStudentDto, Student>();
        CreateMap<Student, ReadStudentDto>();   
    }

}
