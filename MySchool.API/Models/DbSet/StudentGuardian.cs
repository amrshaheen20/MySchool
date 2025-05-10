using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet
{
    public class StudentGuardian : BaseEntity
    {
        public int StudentId { get; set; }
        public int GuardianId { get; set; }
        public virtual User Student { get; set; } = default!;
        public virtual User Guardian { get; set; } = default!;

        [Required, StringLength(100)]
        public string RelationToStudent { get; set; } = string.Empty;
    }

    public class StudentGuardianConfiguration : IEntityTypeConfiguration<StudentGuardian>
    {
        public void Configure(EntityTypeBuilder<StudentGuardian> builder)
        {
            builder.ToTable("Guardians");
            builder.HasIndex(x => new { x.StudentId, x.GuardianId }).IsUnique();
        }
    }
}
