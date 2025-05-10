using AutoMapper;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.GuardianContainer.Mapper
{
    public class GuardianProfile : Profile
    {
        public GuardianProfile()
        {
            CreateMap<StudentGuardianRequestDto, StudentGuardian>();

            CreateMap<StudentGuardian, StudentGuardianResponseDto>();
        }
    }
}
