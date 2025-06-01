namespace MySchool.API.Models.Dtos
{

    public class FeeInfoDto
    {
        public required decimal Total { get; set; }
        public required decimal Paid { get; set; }
        public decimal Remaining => Total - Paid;
        public bool IsFullyPaid => Remaining <= 0;
    }

    public class StudentDashboardResponseDto
    {
        public required AccountResponseDto Account { get; set; } = default!;
        public required int TotalAssignments { get; set; }
        public required int SubmittedAssignments { get; set; }
        public required int PendingAssignments { get; set; }


        public required int TotalAttendance { get; set; }
        public required int AbsentDays { get; set; }
        public required int PresentDays { get; set; }

        public required int TotalNotifications { get; set; }

        public required int TotalSubjects { get; set; }
        public required string ClassName { get; set; } = default!;
        public required int Grade { get; set; } = default!;

        public required FeeInfoDto FeeInfo { get; set; } = default!;
        public required AccountResponseDto? Guardian { get; set; } = default!;
    }


    public class TeacherDashboardResponseDto
    {
        public required AccountResponseDto Account { get; set; } = default!;
        public required int TotalClasses { get; set; }
        public required int TotalStudents { get; set; }
        public required int TotalSubjects { get; set; }

        public required int AssignmentsCreated { get; set; }
        public required int TotalAttendanceMarked { get; set; }
        public required int TotalNotifications { get; set; }

        public required ICollection<SubjectResponseDto> subjects { get; set; } = default!;
    }

    public class AdminDashboardResponseDto
    {
        public required int TotalStudents { get; set; }
        public required int TotalTeachers { get; set; }
        public required int TotalClasses { get; set; }
        public required int TotalSubjects { get; set; }
        public required int TotalTimetables { get; set; }

        public required int TotalNotifications { get; set; }

        public required decimal TotalFeesCollected { get; set; }
        public required decimal TotalFeesRemaining { get; set; }

        public required int TotalAssignmentsCreated { get; set; }
        public required int TotalAssignmentsSubmitted { get; set; }
        public required int TotalAttendance { get; set; }
        public required int TotalAttendanceAbsent { get; set; }
        public required int TotalAttendancePresent { get; set; }
    }


    public class GuardianDashboardResponseDto
    {
        public required AccountResponseDto Account { get; set; } = default!;
        public required int TotalStudents { get; set; }
        public required int TotalAssignments { get; set; }
        public required int SubmittedAssignments { get; set; }

        public required int TotalAttendance { get; set; }
        public required int AbsentDays { get; set; }
        public required int PresentDays { get; set; }

        public required FeeInfoDto FeeInfo { get; set; } = default!;

        public required int TotalNotifications { get; set; }
    }


}
