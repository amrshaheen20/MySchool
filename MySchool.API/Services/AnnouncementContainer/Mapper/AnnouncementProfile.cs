using AutoMapper;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.AnnouncementContainer.Mapper
{
    public class AnnouncementProfile : Profile
    {
        public AnnouncementProfile()
        {
            CreateMap<AnnouncementRequestDto, Announcement>();
            CreateMap<Announcement, AnnouncementResponseDto>()
                .ForMember(dest => dest.time, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}
