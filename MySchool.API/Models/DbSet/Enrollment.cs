

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MySchool.API.Models.DbSet
{
    public class Enrollment : BaseEntity
    {
        public int ClassRoomId { get; set; }
        public virtual ClassRoom ClassRoom { get; set; } = default!;

        public int StudentId { get; set; }
        public virtual User Student { get; set; } = default!;

    }

    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasIndex(e => new { e.StudentId }).IsUnique(); //One student can only enroll in one class
            builder.HasIndex(e => e.ClassRoomId);
        }
    }
}
