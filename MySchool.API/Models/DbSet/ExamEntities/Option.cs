using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet.ExamEntities
{
    public class Option : BaseEntity
    {
        public required int QuestionId { get; set; }
        public virtual Question Question { get; set; } = default!;
        [StringLength(255)]
        public required string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
