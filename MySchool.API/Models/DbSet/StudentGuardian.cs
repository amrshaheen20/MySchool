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

            builder.HasIndex(x => x.StudentId);
            builder.HasIndex(x => x.GuardianId);


            builder.HasOne(g => g.Student)
                .WithMany()
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Cascade);           
            
            builder.HasOne(g => g.Guardian)
                .WithMany()
                .HasForeignKey(g => g.GuardianId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
