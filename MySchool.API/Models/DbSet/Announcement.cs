using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet
{
    public class Announcement : BaseEntity
    {
        public int UserId { get; set; }
        public int? CreatedById { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;

        public virtual User User { get; set; } = default!;
        public virtual User? CreatedBy { get; set; } = default!;
    }

    public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
    {
        public void Configure(EntityTypeBuilder<Announcement> builder)
        {
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
