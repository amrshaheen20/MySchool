using MySchool.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet.ExamEntities
{
    public class Question : BaseEntity
    {
        public required int ExamId { get; set; }
        public virtual Exam Exam { get; set; } = default!;
        [StringLength(255)]
        public required string QuestionText { get; set; }
        public required eQuestionType QuestionType { get; set; }
        public required decimal Mark { get; set; }
        public virtual ICollection<Option>? Options { get; set; } = default!;

        public virtual ICollection<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>();
    }
}
