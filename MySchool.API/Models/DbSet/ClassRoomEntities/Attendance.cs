using MySchool.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet.ClassRoomEntities
{
    public class Attendance : BaseEntity
    {
        public int CreatedById { get; set; }
        public required int StudentId { get; set; } = default!;
        public required int ClassRoomId { get; set; }
        public required DateOnly Date { get; set; }
        public required eAttendanceStatus Status { get; set; }
        [MaxLength(500)]
        public string? Note { get; set; }
        public TimeOnly? LeaveTime { get; set; }

        public virtual User CreatedBy { get; set; } = default!;
        public virtual User Student { get; set; } = default!;
        public virtual ClassRoom ClassRoom { get; set; } = default!;
    }
}
