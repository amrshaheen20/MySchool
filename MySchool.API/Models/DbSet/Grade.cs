using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySchool.API.Models.DbSet.SubjectEntities;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet
{
    public class Grade : BaseEntity
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int CreatedById { get; set; }
        //[Range(2000, 3000)]
        //public required int Year { get; set; }
        [Range(1, 2)]
        public required int TermNumber { get; set; }

        public required float Mark { get; set; }
        public bool IsPublished { get; set; } = false;
        public virtual User Student { get; set; } = default!;
        public virtual Subject Subject { get; set; } = default!;
        public virtual User CreatedBy { get; set; } = default!;
    }

    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.HasIndex(x => new { x.StudentId, x.SubjectId }).IsUnique();
        }
    }
}
