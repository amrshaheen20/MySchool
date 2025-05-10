using AutoMapper;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.GradeContainer.Mapper
{
    public class GradeProfile : Profile
    {
        public GradeProfile()
        {
            CreateMap<GradeRequestDto, Grade>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Grade, GradeResponseDto>();
        }
    }
}