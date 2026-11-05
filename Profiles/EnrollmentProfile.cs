using ApiStudent.Models;
using ApiStudent.Models.Dtos.EnrollmentDto;
using AutoMapper;

namespace ApiStudent.Profiles;

public class EnrollmentProfile: Profile
{

    public EnrollmentProfile()
    {
        CreateMap<CreateEnrollmentDto, Enrollment>();
        CreateMap<Enrollment, ReadEnrollmentDto>();
    }
}
