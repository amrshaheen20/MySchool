using AutoMapper;
using MySchool.API.Models.DbSet.SubjectEntities;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.SubjectContainer.Mapper
{
    public class SubjectProfile : Profile
    {

        public SubjectProfile()
        {
            CreateMap<SubjectRequestDto, Subject>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Subject, SubjectResponseDto>();
        }



    }
}
