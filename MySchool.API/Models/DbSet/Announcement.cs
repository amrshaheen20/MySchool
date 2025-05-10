using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet
{
    public class Announcement : BaseEntity
    {
        public int UserId { get; set; }
        public int CreatedById { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;

        public virtual User User { get; set; } = default!;
        public virtual User CreatedBy { get; set; } = default!;
    }
}
