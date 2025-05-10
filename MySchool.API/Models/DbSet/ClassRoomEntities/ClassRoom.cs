using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySchool.API.Models.DbSet.ExamEntities;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet.ClassRoomEntities
{
    public class ClassRoom : BaseEntity
    {
        [Required, StringLength(255)]
        public required string Name { get; set; }

        [Required]
        public required int Grade { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }

    public class ClassRoomConfiguration : IEntityTypeConfiguration<ClassRoom>
    {
        public void Configure(EntityTypeBuilder<ClassRoom> builder)
        {
            builder.HasIndex(x => new { x.Name, x.Grade }).IsUnique();
            builder.HasMany(x => x.Enrollments)
                   .WithOne(x => x.ClassRoom)
                   .HasForeignKey(x => x.ClassRoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Timetables)
                     .WithOne(x => x.ClassRoom)
                     .HasForeignKey(x => x.ClassRoomId)
                     .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Attendances)
                        .WithOne(x => x.ClassRoom)
                        .HasForeignKey(x => x.ClassRoomId)
                        .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(x => x.Assignments)
            //         .WithOne(x => x.ClassRoom)
            //         .HasForeignKey(x => x.ClassRoomId)
            //         .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
