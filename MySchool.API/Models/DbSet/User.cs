using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySchool.API.Enums;
using MySchool.API.Models.DbSet.ExamEntities;
using System.ComponentModel.DataAnnotations;
namespace MySchool.API.Models.DbSet
{
    public class User : BaseEntity
    {
        //data for login
        [StringLength(255)]
        public required string Name { get; set; }
        [StringLength(255)]
        public required string UserName { get; set; }
        public required eRole Role { get; set; }
      
        [StringLength(500)]
        public required string PasswordHash { get; set; }
        public bool MustChangePassword { get; set; } = true;
        public bool IsActive { get; set; } = false;
        [StringLength(255)]
        public string? NationalId { get; set; }
        public eGender? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        [StringLength(255)]
        public string? Address { get; set; }
        [StringLength(255)]
        public string? PhoneNumber { get; set; }
        public DateTime LastActiveTime { get; set; }

        //public virtual ICollection<UserCustomFields> CustomFields { get; set; } = new List<UserCustomFields>();
        public virtual ICollection<Conversation> ConversationsInitiated { get; set; } = new List<Conversation>();
        public virtual ICollection<Conversation> ConversationsReceived { get; set; } = new List<Conversation>();
        public virtual ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
        public virtual ICollection<Fee> Fees { get; set; } = new List<Fee>();
        public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();



        //public class UserCustomFields : BaseEntity
        //{
        //    [StringLength(255)]
        //    [RegularExpression("^[A-Za-z_]+$", ErrorMessage = "The field {0} must be alphanumeric and can only contain letters and underscores.")]
        //    public required string Key { get; set; }
        //    [StringLength(500)]
        //    public required string Value { get; set; }
        //    public int UserId { get; set; }
        //    public virtual User User { get; set; } = default!;
        //}
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //builder.HasMany(x => x.CustomFields)
            //       .WithOne(x => x.User)
            //       .HasForeignKey(x => x.UserId);

            builder.HasIndex(x => x.UserName).IsUnique();
            //builder.HasIndex(x => x.NationalId).IsUnique();


            builder.HasMany(x => x.ConversationsInitiated)
                   .WithOne(x => x.UserOne)
                   .HasForeignKey(x => x.UserOneId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.ConversationsReceived)
                        .WithOne(x => x.UserTwo)
                        .HasForeignKey(x => x.UserTwoId)
                        .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Sessions)
                     .WithOne(x => x.User)
                     .HasForeignKey(x => x.UserId)
                     .OnDelete(DeleteBehavior.Cascade);


        }
    }

    //public class UserCustomFieldsConfiguration : IEntityTypeConfiguration<UserCustomFields>
    //{
    //    public void Configure(EntityTypeBuilder<UserCustomFields> builder)
    //    {
    //        builder.HasIndex(x => new { x.UserId, x.Key }).IsUnique();
    //    }
    //}


}
