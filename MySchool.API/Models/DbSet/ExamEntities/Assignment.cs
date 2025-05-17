using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySchool.API.Models.DbSet.ClassRoomEntities;
using MySchool.API.Models.DbSet.SubjectEntities;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet.ExamEntities
{
    public class Assignment : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        public DateTime Deadline { get; set; }
        public int ClassRoomId { get; set; }

        public int? CreatedById { get; set; }
        public int SubjectId { get; set; }
        public bool IsActive { get; set; } = true;
        public float Mark { get; set; }

        public virtual ClassRoom ClassRoom { get; set; } = default!;
        public virtual User? CreatedBy { get; set; } = default!;
        public virtual Subject Subject { get; set; } = default!;
        public virtual ICollection<AssignmentSubmission> Submissions { get; set; } = new HashSet<AssignmentSubmission>();
    }


    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.HasIndex(x => new { x.ClassRoomId, x.SubjectId, x.Title }).IsUnique();

            builder.HasIndex(x => x.CreatedById);
            builder.HasIndex(x => x.SubjectId);


            builder.Property(x => x.FilePath).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Deadline).IsRequired();
            builder.Property(x => x.Mark).IsRequired();

            builder.HasMany(x => x.Submissions)
                .WithOne(x => x.Assignment)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);




        }
    }
}
