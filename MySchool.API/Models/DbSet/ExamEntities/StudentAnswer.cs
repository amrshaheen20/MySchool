using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySchool.API.Models.DbSet.ExamEntities
{
    public class StudentAnswer : BaseEntity
    {
        [ForeignKey(nameof(Student))]
        public required int StudentId { get; set; }
        public virtual User Student { get; set; } = default!;

        [ForeignKey(nameof(Question))]
        public required int QuestionId { get; set; }
        public virtual Question Question { get; set; } = default!;

        //if question is multiple choice
        [ForeignKey(nameof(Option))]
        public int? OptionId { get; set; }
        public virtual Option? Option { get; set; } = default!;

        //if question is essay
        public string? AnswerText { get; set; }
        public float? Score { get; set; }
        [MaxLength(500)]
        public string? TeacherFeedback { get; set; }
        public bool IsReviewed { get; set; } = false;
    }
}
