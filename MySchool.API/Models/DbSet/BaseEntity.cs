using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySchool.API.Models.DbSet
{
    public class BaseEntity
    {
        [Key]
        // [Column("^_id", Order = 0)]
        public int Id { get; set; } = default!;

        [Column("created_at", Order = 1000)]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at", Order = 1001)]
        public DateTime UpdatedAt { get; set; }


        public BaseEntity()
        {
            var dateTime = DateTime.UtcNow;
            CreatedAt = dateTime;
            UpdatedAt = dateTime;
        }
    }

    //public class BaseEntityConfiguration : IEntityTypeConfiguration<BaseEntity>
    //{
    //    public void Configure(EntityTypeBuilder<BaseEntity> builder)
    //    {
    //        builder.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
    //        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
    //        //CURRENT_TIMESTAMP
    //    }
    //}

}
