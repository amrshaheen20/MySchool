using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet.ExamEntities
{
    public class AssignmentSubmission : BaseEntity
    {
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public int CreatedById { get; set; }
        [Required]
        [MaxLength(250)]
        public string FilePath { get; set; } = string.Empty;

        public virtual Assignment Assignment { get; set; } = default!;
        public virtual User Student { get; set; } = default!;
        public virtual User CreatedBy { get; set; } = default!;
    }

    public class AssignmentSubmissionConfiguration : IEntityTypeConfiguration<AssignmentSubmission>
    {
        public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
        {
            builder.HasIndex(x => new { x.AssignmentId, x.StudentId }).IsUnique();
        }
    }
}
