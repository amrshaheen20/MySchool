using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MySchool.API.Models.DbSet
{
    public class Conversation : BaseEntity
    {
        public int UserOneId { get; set; }
        public int UserTwoId { get; set; }
        public int? UserOneLastReadMessageId { get; set; }
        public int? UserTwoLastReadMessageId { get; set; }
        public virtual User UserOne { get; set; } = default!;
        public virtual User UserTwo { get; set; } = default!;
        public virtual Message? UserOneLastReadMessage { get; set; }
        public virtual Message? UserTwoLastReadMessage { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }

    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.HasOne(x => x.UserOne)
                .WithMany()
                .HasForeignKey(x => x.UserOneId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.UserTwo)
                .WithMany()
                .HasForeignKey(x => x.UserTwoId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.UserOneLastReadMessage)
                 .WithMany()
                 .HasForeignKey(x => x.UserOneLastReadMessageId)
                 .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.UserTwoLastReadMessage)
                 .WithMany()
                 .HasForeignKey(x => x.UserTwoLastReadMessageId)
                 .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => new { x.UserOneId, x.UserTwoId }).IsUnique();
            builder.HasIndex(x => x.UserOneId);
            builder.HasIndex(x => x.UserTwoId);
        }
    }
}