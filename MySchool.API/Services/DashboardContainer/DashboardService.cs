using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Enums;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.DbSet.ExamEntities;
using MySchool.API.Models.Dtos;
using System.Net;

namespace MySchool.API.Services.DashboardContainer
{
    public class DashboardService(
        IUnitOfWork unitOfWork,
        IMapper mapper
        ) : IServiceInjector
    {
        private IGenericRepository<User> UserRepo => unitOfWork.GetRepository<User>();
        private IGenericRepository<Assignment> AssignmentRepo => unitOfWork.GetRepository<Assignment>();
        private IGenericRepository<AssignmentSubmission> SubmissionRepo => unitOfWork.GetRepository<AssignmentSubmission>();
        private IGenericRepository<Attendance> AttendanceRepo => unitOfWork.GetRepository<Attendance>();
        private IGenericRepository<Announcement> AnnouncementRepo => unitOfWork.GetRepository<Announcement>();
        private IGenericRepository<ClassRoom> ClassRoomRepo => unitOfWork.GetRepository<ClassRoom>();
        private IGenericRepository<Enrollment> EnrollmentRepo => unitOfWork.GetRepository<Enrollment>();
        private IGenericRepository<Fee> FeeRepo => unitOfWork.GetRepository<Fee>();
        private IGenericRepository<Timetable> TimetableRepo => unitOfWork.GetRepository<Timetable>();
        private IGenericRepository<StudentGuardian> genericRepository => unitOfWork.GetRepository<StudentGuardian>();

        public async Task<IBaseResponse<StudentDashboardResponseDto>> GetStudentDashboardAsync(int StudentId)
        {
            var User = await UserRepo.GetByIdAsync<AccountResponseDto>(StudentId);
            if (User == null || User.Role != eRole.Student)
            {
                return new BaseResponse<StudentDashboardResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Invalid user id");
            }



            var userId = User.Id;

            var enrollment = await EnrollmentRepo.GetByAsync(new CommandsInjector<Enrollment>().AddCommand(x => x.Include(x => x.ClassRoom)).Where(x => x.StudentId == userId));
            if (enrollment == null)
            {
                return new BaseResponse<StudentDashboardResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Student not enrolled in any class");
            }

            var classRoomId = enrollment.ClassRoomId;


            // subjects
            int totalSubjects = TimetableRepo.GetAllBy(new CommandsInjector<Timetable>().Where(x => x.ClassRoomId == classRoomId)).Count();

            // Assignments
            var totalAssignments = AssignmentRepo.GetAllBy(new CommandsInjector<Assignment>().Where(x => x.ClassRoomId == classRoomId)).Count();
            var submittedAssignments = SubmissionRepo.GetAllBy(new CommandsInjector<AssignmentSubmission>().Where(x => x.StudentId == userId)).Count();
            var pendingAssignments = totalAssignments - submittedAssignments;

            // Attendance
            var totalAttendance = AttendanceRepo.GetAllBy(new CommandsInjector<Attendance>().Where(x => x.ClassRoomId == classRoomId)).Count();
            var presentDays = AttendanceRepo.GetAllBy(new CommandsInjector<Attendance>().Where(x => x.ClassRoomId == classRoomId && x.StudentId == userId)).Count();
            var absentDays = totalAttendance - presentDays;

            // Announcements
            var announcementsCount = AnnouncementRepo.GetAllBy(new CommandsInjector<Announcement>().Where(x => x.UserId == userId)).Count();


            // Notifications
            var totalNotifications = AnnouncementRepo.GetAllBy(new CommandsInjector<Announcement>().Where(x => x.UserId == userId)).Count();

            // Fee Info
            var fee = FeeRepo.GetAllBy(new CommandsInjector<Fee>().Where(x => x.StudentId == userId));
            var feeDto = new FeeInfoDto
            {
                Total = fee.Sum(x => x.TotalAmount),
                Paid = fee.Sum(x => x.PaidAmount),
            };


            //Guardian
            var guardian = genericRepository.GetAllBy(new CommandsInjector<StudentGuardian>().Where(x => x.StudentId == userId))
            .Select(x => mapper.Map<AccountResponseDto>(x.Guardian)).FirstOrDefault();

            var dto = new StudentDashboardResponseDto
            {
                Account = User,
                TotalAssignments = totalAssignments,
                SubmittedAssignments = submittedAssignments,
                PendingAssignments = pendingAssignments,
                TotalAttendance = totalAttendance,
                PresentDays = presentDays,
                AbsentDays = absentDays,
                TotalNotifications = totalNotifications,
                TotalSubjects = totalSubjects,
                ClassName = enrollment.ClassRoom.Name,
                Grade = enrollment.ClassRoom.Grade,
                FeeInfo = feeDto,
                Guardian = guardian,
            };

            return new BaseResponse<StudentDashboardResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(dto);
        }



        public async Task<IBaseResponse<TeacherDashboardResponseDto>> GetTeacherDashboardAsync(int TeacherId)
        {
            var User = await UserRepo.GetByIdAsync<AccountResponseDto>(TeacherId);
            if (User == null || User.Role != eRole.Teacher)
            {
                return new BaseResponse<TeacherDashboardResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Invalid user id");
            }




            var userId = User.Id;

            // Get all classes taught by this teacher
            var classes = ClassRoomRepo.GetAllBy(new CommandsInjector<ClassRoom>().Where(x => x.Timetables.Any(t => t.TeacherId == userId)));
            if (classes == null || !classes.Any())
            {
                return new BaseResponse<TeacherDashboardResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Teacher not assigned to any class");
            }

            var classIds = classes.Select(c => c.Id).ToList();

            // Count total students in all classes
            var totalStudents = EnrollmentRepo.GetAllBy(new CommandsInjector<Enrollment>().Where(x => classIds.Contains(x.ClassRoomId))).Count();

            // Count total subjects taught by this teacher
            var totalSubjects = TimetableRepo.GetAllBy(new CommandsInjector<Timetable>().Where(x => classIds.Contains(x.ClassRoomId))).Count();

            // Count assignments created by this teacher
            var assignmentsCreated = AssignmentRepo.GetAllBy(new CommandsInjector<Assignment>().Where(x => x.CreatedById == userId)).Count();

            // Count attendance records marked by this teacher
            var attendanceMarked = AttendanceRepo.GetAllBy(new CommandsInjector<Attendance>().Where(x => x.CreatedById == userId)).Count();

            // Count notifications/announcements made by this teacher
            var notifications = AnnouncementRepo.GetAllBy(new CommandsInjector<Announcement>().Where(x => x.UserId == userId)).Count();

            // Get subjects information
            var subjectsData = TimetableRepo.GetAllBy(
                new CommandsInjector<Timetable>().Where(x => classIds.Contains(x.ClassRoomId) && x.TeacherId == userId)
            ).Select(x => mapper.Map<SubjectResponseDto>(x.Subject));
            ;



            var dto = new TeacherDashboardResponseDto
            {
                Account = User,
                TotalClasses = classes.Count(),
                TotalStudents = totalStudents,
                TotalSubjects = totalSubjects,
                AssignmentsCreated = assignmentsCreated,
                TotalAttendanceMarked = attendanceMarked,
                TotalNotifications = notifications,
                subjects = subjectsData.ToList()
            };

            return new BaseResponse<TeacherDashboardResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(dto);
        }

        public IBaseResponse<AdminDashboardResponseDto> GetAdminDashboard()
        {
            // Count total students
            var totalStudents = UserRepo.GetAllBy(new CommandsInjector<User>().Where(x => x.Role == eRole.Student)).Count();

            // Count total teachers
            var totalTeachers = UserRepo.GetAllBy(new CommandsInjector<User>().Where(x => x.Role == eRole.Teacher)).Count();

            // Count total classes
            var totalClasses = ClassRoomRepo.GetAll().Count();

            // Count total subjects
            var totalSubjects = TimetableRepo.GetAll().Count();

            // Count total notifications/announcements
            var totalNotifications = AnnouncementRepo.GetAll().Count();

            // Get fee information
            var feeInfo = FeeRepo.GetAll();
            var totalFeesCollected = feeInfo.Sum(x => x.PaidAmount);
            var totalFeesRemaining = feeInfo.Sum(x => x.TotalAmount - x.PaidAmount);

            // Count assignments
            var totalAssignmentsCreated = AssignmentRepo.GetAll().Count();
            var totalAssignmentsSubmitted = SubmissionRepo.GetAll().Count();

            // Count attendance records
            var totalAttendance = AttendanceRepo.GetAll().Count();
            var totalAttendanceAbsent = AttendanceRepo.GetAllBy(new CommandsInjector<Attendance>().Where(x => x.Status == eAttendanceStatus.Absent)).Count();
            var totalAttendancePresent = totalAttendance - totalAttendanceAbsent;



            var dto = new AdminDashboardResponseDto
            {
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                TotalClasses = totalClasses,
                TotalSubjects = totalSubjects,
                TotalNotifications = totalNotifications,
                TotalFeesCollected = totalFeesCollected,
                TotalFeesRemaining = totalFeesRemaining,
                TotalAssignmentsCreated = totalAssignmentsCreated,
                TotalAssignmentsSubmitted = totalAssignmentsSubmitted,
                TotalAttendance = totalAttendance,
                TotalAttendanceAbsent = totalAttendanceAbsent,
                TotalAttendancePresent = totalAttendancePresent,
            };

            return new BaseResponse<AdminDashboardResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(dto);
        }

        public async Task<IBaseResponse<GuardianDashboardResponseDto>> GetGuardianDashboardAsync(int GuardianId)
        {
            var User = await UserRepo.GetByIdAsync<AccountResponseDto>(GuardianId);
            if (User == null || User.Role != eRole.Guardian)
            {
                return new BaseResponse<GuardianDashboardResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Invalid user id");
            }
            var guardianId = User.Id;

            // Get all students associated with this guardian
            var students = genericRepository.GetAllBy(new CommandsInjector<StudentGuardian>().Where(x => x.GuardianId == guardianId));
            if (students == null || !students.Any())
            {
                return new BaseResponse<GuardianDashboardResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("No students found for this guardian");
            }

            var studentIds = students.Select(s => s.StudentId).ToList();

            // Get all enrollments for these students
            var enrollments = EnrollmentRepo.GetAllBy(new CommandsInjector<Enrollment>().Where(x => studentIds.Contains(x.StudentId)));
            var classRoomIds = enrollments.Select(e => e.ClassRoomId).Distinct().ToList();

            // Assignments information
            var totalAssignments = AssignmentRepo.GetAllBy(new CommandsInjector<Assignment>().Where(x => classRoomIds.Contains(x.ClassRoomId))).Count();
            var submittedAssignments = SubmissionRepo.GetAllBy(new CommandsInjector<AssignmentSubmission>().Where(x => studentIds.Contains(x.StudentId))).Count();

            // Attendance information
            var totalAttendance = AttendanceRepo.GetAllBy(new CommandsInjector<Attendance>().Where(x => classRoomIds.Contains(x.ClassRoomId))).Count();
            var presentDays = AttendanceRepo.GetAllBy(new CommandsInjector<Attendance>().Where(x => studentIds.Contains(x.StudentId) && x.Status == eAttendanceStatus.Present)).Count();
            var absentDays = totalAttendance - presentDays;

            // Fee information
            var feeInfo = FeeRepo.GetAllBy(new CommandsInjector<Fee>().Where(x => studentIds.Contains(x.StudentId)));
            var feeDto = new FeeInfoDto
            {
                Total = feeInfo.Sum(x => x.TotalAmount),
                Paid = feeInfo.Sum(x => x.PaidAmount)
            };

            // Notifications
            var totalNotifications = AnnouncementRepo.GetAllBy(new CommandsInjector<Announcement>().Where(x => x.UserId == guardianId)).Count();

            var dto = new GuardianDashboardResponseDto
            {
                Account = User,
                TotalStudents = students.Count(),
                TotalAssignments = totalAssignments,
                SubmittedAssignments = submittedAssignments,
                TotalAttendance = totalAttendance,
                PresentDays = presentDays,
                AbsentDays = absentDays,
                FeeInfo = feeDto,
                TotalNotifications = totalNotifications
            };

            return new BaseResponse<GuardianDashboardResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(dto);
        }
    }
}
