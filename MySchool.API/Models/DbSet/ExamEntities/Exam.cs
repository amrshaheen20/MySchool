using Microsoft.EntityFrameworkCore;
using MySchool.API.Models.DbSet.ClassRoomEntities;
using MySchool.API.Models.DbSet.SubjectEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySchool.API.Models.DbSet.ExamEntities
{
    public class Exam : BaseEntity
    {
        [StringLength(255)]
        public required string Title { get; set; }

        public int? SubjectId { get; set; }
        public virtual Subject? Subject { get; set; } = default!;

        public int? ClassroomSubjectId { get; set; }
        public virtual Timetable? ClassroomSubject { get; set; } = default!;

        public required int CreatedById { get; set; }
        public virtual User CreatedBy { get; set; } = default!;

        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = default!;

        [NotMapped]
        public bool IsExamEnded => DateTime.UtcNow > EndTime;


        public static void ConfigureModel(ModelBuilder modelBuilder)
        {


        }
    }

}
