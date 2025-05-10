using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MySchool.API.Models.DbSet
{
    public class UserSession : BaseEntity
    {
        public int UserId { get; set; }
        public string TokenId { get; set; } = default!;
        public DateTime ExpirationTime { get; set; }
        public string? UserAgent { get; set; }
        public virtual User User { get; set; } = default!;
    }

    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {

            builder.HasIndex(x => new { x.UserId, x.TokenId, x.ExpirationTime })
                .IsUnique();
        }
    }
}

