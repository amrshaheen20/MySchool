﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace MySchool.API.Models.DbSet
{
    public class Timetable : BaseEntity
    {
        public int ClassRoomId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }

        [Required]
        public DayOfWeek Day { get; set; }
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }

        [DefaultValue(false)]
        public bool isBreak { get; set; } = false;

        public virtual ClassRoom ClassRoom { get; set; } = default!;
        public virtual User Teacher { get; set; } = default!;
        public virtual Subject Subject { get; set; } = default!;
    }

    public class TimetableConfiguration : IEntityTypeConfiguration<Timetable>
    {
        public void Configure(EntityTypeBuilder<Timetable> builder)
        {
            builder.HasIndex(x => new { x.ClassRoomId, x.Day, x.StartTime, x.EndTime }).IsUnique();
            builder.HasIndex(x => x.ClassRoomId);
            builder.HasIndex(x => x.SubjectId);
            builder.HasIndex(x => x.TeacherId);
        }
    }
}
