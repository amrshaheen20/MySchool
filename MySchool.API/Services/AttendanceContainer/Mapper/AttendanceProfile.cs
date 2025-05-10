using AutoMapper;
using MySchool.API.Models.DbSet.ClassRoomEntities;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.AttendanceContainer.Mapper
{
    public class AttendanceProfile : Profile
    {
        public AttendanceProfile()
        {
            CreateMap<AttendanceRequestDto, Attendance>()
                .ForMember(
                    dest => dest.ClassRoomId,
                    opt => opt.MapFrom((src, dest) => src.ClassId == null ? dest.ClassRoomId : src.ClassId)
                )
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Attendance, AttendanceResponseDto>()
                .ForMember(
                    dest => dest.ClassId,
                    opt => opt.MapFrom(src => src.ClassRoomId)
                );
        }
    }
}
