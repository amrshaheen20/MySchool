using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;
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
        [DefaultValueSql("getutcdate()")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at", Order = 1001)]
        [DefaultValueSql("getutcdate()")]
        public DateTime UpdatedAt { get; set; }
    }
}
