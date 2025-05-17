using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet
{
    public class Announcement : BaseEntity
    {
        public int UserId { get; set; }
        public int? CreatedById { get; set; }

        [StringLength(100)]
        public required string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public required string Content { get; set; } = string.Empty;

        public virtual User User { get; set; } = default!;
        public virtual User? CreatedBy { get; set; } = default!;
    }

    public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
    {
        public void Configure(EntityTypeBuilder<Announcement> builder)
        {
            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.CreatedById);

            builder.HasOne(g => g.User)
             .WithMany(x => x.Announcements)
             .HasForeignKey(g => g.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.CreatedBy)
             .WithMany()
             .HasForeignKey(g => g.CreatedById)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
