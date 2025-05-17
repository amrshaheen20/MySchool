using AutoMapper;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.TimeTableContainer.Mapper
{
    public class TimeTableProfile : Profile
    {
        public TimeTableProfile()
        {
            //Request
            CreateMap<TimeTableRequestDto, Timetable>()
                .ForMember(dest => dest.ClassRoomId, opt => opt.MapFrom(src => src.ClassId))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            //Response
            CreateMap<Timetable, TimeTableResponseDto>()
                .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.ClassRoom));
        }



    }
}
