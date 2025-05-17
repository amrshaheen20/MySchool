using Microsoft.AspNetCore.Mvc;
using MySchool.API.Enums;
using MySchool.API.Models.DbSet;
using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class AttendanceRequestDto
    {
        [IsValid(typeof(RequiredAttribute), typeof(IsExists<User>))]
        public int? StudentId { get; set; }
        [IsValid(typeof(RequiredAttribute), typeof(IsExists<ClassRoom>))]
        public int? ClassId { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public DateOnly? Date { get; set; }
        [IsValid(typeof(RequiredAttribute))]
        public eAttendanceStatus? Status { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
        public TimeOnly? LeaveTime { get; set; }
    }

    public class AttendanceResponseDto : BaseResponseDto
    {
        public AccountResponseDto Student { get; set; } = default!;
        public ClassResponseDto Class { get; set; } = default!;
        public DateOnly Date { get; set; }
        public eAttendanceStatus Status { get; set; }
        public string? Note { get; set; }
        public TimeOnly? LeaveTime { get; set; }
    }


    public class ClassAttendanceRequestDto
    {
        /// <summary>
        /// The ID of the class to retrieve attendance for
        /// </summary>
        [Required]
        [FromQuery]
        public int ClassId { get; set; }

        /// <summary>
        /// The specific date to check attendance
        /// </summary>
        [Required]
        [FromQuery]
        public DateOnly Date { get; set; }
    }


    public class ClassAttendanceResponseDto
    {
        public AccountResponseDto Student { get; set; } = default!;
        public AttendanceResponseDto? Attendance { get; set; } = default!;
    }
}
