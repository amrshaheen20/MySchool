using MySchool.API.Models.DbSet;
using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class TimeTableRequestDto
    {
        [IsValid(typeof(RequiredAttribute), typeof(IsExists<ClassRoom>))]
        public int? ClassId { get; set; }

        [IsValid(typeof(RequiredAttribute), typeof(IsExists<Subject>))]
        public int? SubjectId { get; set; }

        [IsValid(typeof(RequiredAttribute), typeof(IsExists<User>))]
        public int? TeacherId { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public DayOfWeek? Day { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public TimeOnly? StartTime { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public TimeOnly? EndTime { get; set; }
    }

    public class TimeTableResponseDto : BaseResponseDto
    {
        public SubjectResponseDto Subject { get; set; } = default!;
        public AccountResponseDto Teacher { get; set; } = default!;
        public ClassResponseDto Class { get; set; } = default!;
        public DayOfWeek Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }

}

