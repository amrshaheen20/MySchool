using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet.ExamEntities
{
    public class AssignmentSubmission : BaseEntity
    {
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        
        [MaxLength(250)]
        public required string FilePath { get; set; } = string.Empty;

        public virtual Assignment Assignment { get; set; } = default!;
        public virtual User Student { get; set; } = default!;
    }

    public class AssignmentSubmissionConfiguration : IEntityTypeConfiguration<AssignmentSubmission>
    {
        public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
        {
            builder.HasIndex(x => new { x.AssignmentId, x.StudentId }).IsUnique();
            builder.HasIndex(x => x.AssignmentId);
            builder.HasIndex(x => x.StudentId);
        
            builder.Property(x => x.FilePath)
                .IsUnicode(false);

            builder.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
