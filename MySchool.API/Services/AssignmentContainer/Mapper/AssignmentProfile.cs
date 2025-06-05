using AutoMapper;
using MySchool.API.Models.DbSet.ExamEntities;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.AssignmentContainer.Mapper
{
    public class AssignmentProfile : Profile
    {
        public AssignmentProfile()
        {
            CreateMap<AssignmentRequestDto, Assignment>()
                .ForMember(
                    dest => dest.ClassRoomId,
                    opt => opt.MapFrom((src, dest) => src.ClassId == null ? dest.ClassRoomId : src.ClassId)
                )
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            CreateMap<Assignment, AssignmentResponseDto>()
                .ForMember(
                    dest => dest.Class,
                    opt => opt.MapFrom(src => src.ClassRoom)
                )
                .ForMember(
                    dest => dest.IsDeadlinePassed,
                    opt => opt.MapFrom(src => src.Deadline < DateTime.UtcNow)
                ).ForMember(
                    dest => dest.TotalSubmissions,
                    opt => opt.MapFrom(src => src.Submissions.Count())
                );

            CreateMap<AssignmentSubmission, AssignmentSubmissionResponseDto>();
        }
    }

}
