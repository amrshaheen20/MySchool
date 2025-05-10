using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.DbSet.ClassRoomEntities
{
    public class Evaluation : BaseEntity
    {
        public required int EvaluatorId { get; set; }
        public virtual User Evaluator { get; set; } = default!;

        public required int EvaluateeId { get; set; }
        public virtual User Evaluatee { get; set; } = default!;

        public required int ClassRoomId { get; set; }
        public virtual ClassRoom ClassRoom { get; set; } = default!;

        [Range(1, 5)]
        public required int Rating { get; set; }
        [StringLength(255)]
        public string? Comments { get; set; }
    }
}
