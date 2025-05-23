﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet
{
    public class Message : BaseEntity
    {
        [StringLength(500)]
        public required string Content { get; set; } = default!;

        public int? UserId { get; set; }
        public int ConversationId { get; set; }

        public User? User { get; set; } = default!;
        public Conversation Conversation { get; set; } = default!;
    }

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.ConversationId);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Conversation)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
