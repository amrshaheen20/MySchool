using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet
{
    public class Grade : BaseEntity
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int? CreatedById { get; set; }
        //[Range(2000, 3000)]
        //public required int Year { get; set; }
        [Range(1, 2)]
        public required int TermNumber { get; set; }

        [DefaultValue(0)]
        public required float Mark { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
        public virtual User Student { get; set; } = default!;
        public virtual Subject Subject { get; set; } = default!;
        public virtual User? CreatedBy { get; set; } = default!;

    }

    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.HasIndex(x => new { x.StudentId, x.SubjectId }).IsUnique();
            builder.HasIndex(x => x.StudentId);
            builder.HasIndex(x => x.SubjectId);
            builder.HasIndex(x => x.CreatedById);


            builder.Property(x => x.IsPublished).HasDefaultValue(false);

            builder.HasOne(g => g.Student)
                  .WithMany()
                  .HasForeignKey(g => g.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.CreatedBy)
                .WithMany()
                .HasForeignKey(g => g.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(g => g.Subject)
                .WithMany()
                .HasForeignKey(g => g.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
