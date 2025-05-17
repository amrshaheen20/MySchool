using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySchool.API.Models.DbSet
{
    public class Fee : BaseEntity
    {
        public int StudentId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime DueDate { get; set; }
        [StringLength(500)]
        public string Description { get; set; } = default!;

        [NotMapped]
        public bool IsFullyPaid => PaidAmount >= TotalAmount;

        public int? CreatedById { get; set; }

        public virtual User Student { get; set; } = default!;
        public virtual User? CreatedBy { get; set; } = default!;
    }

    public class FeeConfiguration : IEntityTypeConfiguration<Fee>
    {
        public void Configure(EntityTypeBuilder<Fee> builder)
        {
            builder.Property(f => f.TotalAmount).HasPrecision(18, 2);
            builder.Property(f => f.PaidAmount).HasPrecision(18, 2);

            builder.HasOne(g => g.Student)
             .WithMany(x=>x.Fees)
             .HasForeignKey(g => g.StudentId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.CreatedBy)
             .WithMany()
             .HasForeignKey(g => g.CreatedById)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
