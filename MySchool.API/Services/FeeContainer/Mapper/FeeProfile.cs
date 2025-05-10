using AutoMapper;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.FeeContainer.Mapper
{
    public class FeeProfile : Profile
    {
        public FeeProfile()
        {
            CreateMap<FeeRequestDto, Fee>()
                 .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Fee, FeeResponseDto>()
                .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.Student))
                .ForMember(dest => dest.IsPaid, opt => opt.MapFrom(src => src.IsFullyPaid))
                .ForMember(dest => dest.PaidDate, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}
