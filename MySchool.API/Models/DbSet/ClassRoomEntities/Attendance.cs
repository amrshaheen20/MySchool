using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySchool.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet.ClassRoomEntities
{
    public class Attendance : BaseEntity
    {
        public int? CreatedById { get; set; }
        public required int StudentId { get; set; } = default!;
        public required int ClassRoomId { get; set; }
        public required DateOnly Date { get; set; }
        public required eAttendanceStatus Status { get; set; }
        [MaxLength(500)]
        public string? Note { get; set; }
        public TimeOnly? LeaveTime { get; set; }

        public virtual User? CreatedBy { get; set; } = default!;
        public virtual User Student { get; set; } = default!;
        public virtual ClassRoom ClassRoom { get; set; } = default!;
    }


    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.HasIndex(a => new { a.StudentId, a.ClassRoomId, a.Date }).IsUnique();

            builder.HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.CreatedBy)
                .WithMany()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.ClassRoom)
                .WithMany()
                .HasForeignKey(a => a.ClassRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }


}
