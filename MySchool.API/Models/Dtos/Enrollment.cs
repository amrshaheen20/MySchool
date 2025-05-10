using MySchool.API.Enums;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.DbSet.ClassRoomEntities;
using MySchool.API.Validators;

namespace MySchool.API.Models.Dtos
{
    public class EnrollmentRequestDto
    {
        [IsExists<User>(Role = eRole.Student)]
        public int StudentId { get; set; }
        [IsExists<ClassRoom>]
        public int ClassId { get; set; }
    }

    public class EnrollmentResponseDto : BaseResponseDto
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public AccountResponseDto Student { get; set; } = default!;
        public ClassResponseDto Class { get; set; } = default!;
        public DateTime EnrollmentDate { get; set; }
    }


}
