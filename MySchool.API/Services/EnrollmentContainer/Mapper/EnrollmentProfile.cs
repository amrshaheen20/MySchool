using AutoMapper;
using MySchool.API.Models.DbSet.ClassRoomEntities;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.EnrollmentContainer.Mapper
{
    public class EnrollmentProfile : Profile
    {
        public EnrollmentProfile()
        {
            //Request
            CreateMap<EnrollmentRequestDto, Enrollment>()
                .ForMember(dest => dest.ClassRoomId, opt => opt.MapFrom(src => src.ClassId))
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId));



            //Response
            CreateMap<Enrollment, EnrollmentResponseDto>()
                .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.ClassRoom))
                .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.Student))
                .ForMember(dest => dest.EnrollmentDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.ClassRoomId));
        }



    }
}
