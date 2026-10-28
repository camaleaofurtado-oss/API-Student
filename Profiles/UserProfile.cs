using ApiStudent.Models;
using ApiStudent.Models.Dtos.UserDto;
using AutoMapper;

namespace ApiStudent.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
        CreateMap<User, ReadUserDto>();
        CreateMap<Student, CreateUserDto>();

    }

}
