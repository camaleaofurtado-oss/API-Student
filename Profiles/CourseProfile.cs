using ApiStudent.Models;
using ApiStudent.Models.Dtos.CourseDto;
using ApiStudent.Service;
using AutoMapper;

namespace ApiStudent.Profiles;

public class CourseProfile: Profile
{
    public CourseProfile()
    {
        CreateMap<CreateCourseDto, Course>();
        CreateMap<Course, ReadCourseDto>().ForMember(courseDto=> courseDto.Categoria, opt => opt.MapFrom(course=> course.Categoria.ToString())).
                                           ForMember(courseDto=> courseDto.Enrollments, opt=> opt.MapFrom(course=> course.Enrollments));
        CreateMap<UpdateCourseDto, Course>();

    }
}
