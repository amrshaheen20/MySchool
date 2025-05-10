using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet.ExamEntities
{
    public class AssignmentGrade : BaseEntity
    {
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public int CreatedById { get; set; }
        [Required]
        public float Grade { get; set; }
        [MaxLength(500)]
        public string? Comment { get; set; }

        public virtual Assignment Assignment { get; set; } = default!;
        public virtual User Student { get; set; } = default!;
        public virtual User CreatedBy { get; set; } = default!;

    }

    public class AssignmentGradeConfiguration : IEntityTypeConfiguration<AssignmentGrade>
    {
        public void Configure(EntityTypeBuilder<AssignmentGrade> builder)
        {
            builder.HasIndex(x => new { x.AssignmentId, x.StudentId }).IsUnique();
        }
    }
}
