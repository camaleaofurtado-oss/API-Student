using ApiStudent.Models;
using ApiStudent.Models.Dtos;
using ApiStudent.Models.Dtos.StudentDto;
using AutoMapper;

namespace ApiStudent.Profiles;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<CreateStudentDto, Student>();
        CreateMap<Student, ReadStudentDto>().ForMember(studentDto=> studentDto.Enrollments, opt=> opt.MapFrom(student=> student.Enrollments));
        CreateMap<UpdateStudentDto, Student>();
        
    }

}
