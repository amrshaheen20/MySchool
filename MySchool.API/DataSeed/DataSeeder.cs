using Bogus;
using MySchool.API.Enums;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.DbSet.ExamEntities;

namespace MySchool.API.DataSeed
{
    public static class Data
    {
        public static List<User> GenerateUsers(int count = 100)
        {
            var roles = new[] { "Admin", "Teacher", "Student", "Guardian" };
            var genders = new[] { "Male", "Female" };
            var maleNames = new[] { "Mohamed", "Ahmed", "Ali", "Omar", "Youssef", "Hassan", "Ibrahim", "Khaled", "Tamer", "Amr", "Kamal" };
            var femaleNames = new[] { "Fatma", "Sara", "Aisha", "Mona", "Nadia", "Hana", "Dina", "Rania", "Yasmin", "Laila", "Nesreen", "Riham" };

            var faker = new Faker("en");

            var users = new List<User>
            {
                new User
                {
                    Name = "admin",
                    UserName = "admin",
                    Role = eRole.Admin,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"),
                    MustChangePassword = false,
                    IsActive = true
                }
            };


            for (int i = 0; i < count; i++)
            {
                var gender = faker.PickRandom(genders);
                var role = Enum.Parse<eRole>(faker.PickRandom(roles));
                var name = gender == "Male"
                    ? $"{faker.PickRandom(maleNames)} {faker.PickRandom(maleNames)} {faker.PickRandom(maleNames)}"
                    : $"{faker.PickRandom(femaleNames)} {faker.PickRandom(maleNames)} {faker.PickRandom(maleNames)}";

                var dob = role switch
                {
                    eRole.Student => faker.Date.Past(18, DateTime.Today.AddYears(-3)),
                    _ => faker.Date.Past(70, DateTime.Today.AddYears(-18))
                };

                users.Add(new User
                {
                    Name = name,
                    UserName = faker.Random.Number(10000000, 99999999).ToString(),
                    Role = role,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"),
                    MustChangePassword = faker.Random.Bool(),
                    IsActive = faker.Random.Bool(),
                    NationalId = faker.Random.ReplaceNumbers("##############"),
                    Gender = Enum.Parse<eGender>(gender),
                    DateOfBirth = DateOnly.FromDateTime(dob),
                    Address = faker.Address.FullAddress(),
                    PhoneNumber = faker.PickRandom(new[] { "010", "011", "012", "015" }) + faker.Random.ReplaceNumbers("########"),
                    LastActiveTime = faker.Date.Recent(90, DateTime.UtcNow)
                });
            }

            return users;
        }
        public static List<ClassRoom> GenerateClassRooms(int count = 20)
        {
            var faker = new Faker();
            var classRooms = new HashSet<(string Name, int Grade)>();
            var result = new List<ClassRoom>();

            var possibleNames = new[] { "A", "B", "C", "D", "E" };
            var possibleGrades = Enumerable.Range(1, 12);

            foreach (var name in possibleNames)
            {
                foreach (var grade in possibleGrades)
                {
                    classRooms.Add((name, grade));
                }
            }

            var selected = classRooms.OrderBy(_ => faker.Random.Int()).Take(count);

            foreach (var (name, grade) in selected)
            {
                result.Add(new ClassRoom
                {
                    Name = name,
                    Grade = grade
                });
            }

            return result;
        }

        public static List<Subject> GenerateSubjects(int count = 10)
        {
            var faker = new Faker("en");
            var subjects = new List<Subject>();

            for (int i = 1; i <= count; i++)
            {
                subjects.Add(new Subject
                {
                    Name = faker.Company.CatchPhrase(),
                    Description = faker.Lorem.Sentence(10)
                });
            }

            return subjects;
        }
        public static List<Enrollment> GenerateEnrollments(List<ClassRoom> classRooms, List<User> students, int count = 100)
        {
            var faker = new Faker();
            var enrollments = new List<Enrollment>();

            foreach (var student in students)
            {
                var classRoom = faker.PickRandom(classRooms);

                enrollments.Add(new Enrollment
                {
                    ClassRoomId = classRoom.Id,
                    StudentId = student.Id,
                    Student = student,
                    ClassRoom = classRoom,
                });
            }

            return enrollments;
        }
        public static List<Timetable> GenerateTimetables(
    List<ClassRoom> classRooms,
    List<User> teachers,
    List<Subject> subjects,
    int count = 100)
        {
            var faker = new Faker();
            var timetables = new List<Timetable>();

            var daysOfWeek = Enum.GetValues<DayOfWeek>();
            var timeSlots = new List<(TimeOnly Start, TimeOnly End)>
    {
        (new TimeOnly(8, 0), new TimeOnly(9, 0)),
        (new TimeOnly(9, 15), new TimeOnly(10, 15)),
        (new TimeOnly(10, 30), new TimeOnly(11, 30)),
        (new TimeOnly(11, 45), new TimeOnly(12, 45)),
        (new TimeOnly(13, 0), new TimeOnly(14, 0)),
        (new TimeOnly(14, 15), new TimeOnly(15, 15)),
    };

            var usedSlots = new HashSet<string>();

            int attempts = 0, maxAttempts = count * 10;

            while (timetables.Count < count && attempts < maxAttempts)
            {
                var day = faker.PickRandom(daysOfWeek);
                var (startTime, endTime) = faker.PickRandom(timeSlots);
                var classRoom = faker.PickRandom(classRooms);
                var teacher = faker.PickRandom(teachers);
                var subject = faker.PickRandom(subjects);
                var isBreak = faker.Random.Bool();

                string key = $"{classRoom.Id}_{day}_{startTime}_{endTime}";
                if (usedSlots.Contains(key))
                {
                    attempts++;
                    continue;
                }

                usedSlots.Add(key);

                timetables.Add(new Timetable
                {
                    ClassRoomId = classRoom.Id,
                    ClassRoom = classRoom,
                    SubjectId = subject.Id,
                    Subject = subject,
                    TeacherId = teacher.Id,
                    Teacher = teacher,
                    Day = day,
                    StartTime = startTime,
                    EndTime = endTime,
                    isBreak = isBreak
                });
            }

            return timetables;
        }

        public static List<Attendance> GenerateAttendance(List<User> students, List<User> teachersOrAdmins, List<ClassRoom> classRooms, int days = 5)
        {
            var faker = new Faker();
            var attendances = new List<Attendance>();
            var statusOptions = Enum.GetValues<eAttendanceStatus>();



            foreach (var student in students)
            {
                for (int i = 0; i < days; i++)
                {
                    var classRoom = faker.PickRandom(classRooms);
                    var createdBy = faker.PickRandom(teachersOrAdmins);
                    var date = DateOnly.FromDateTime(DateTime.Today.AddDays(-i));

                    var status = faker.PickRandom(statusOptions);

                    attendances.Add(new Attendance
                    {
                        StudentId = student.Id,
                        Student = student,
                        ClassRoomId = classRoom.Id,
                        ClassRoom = classRoom,
                        CreatedById = createdBy.Id,
                        CreatedBy = createdBy,
                        Date = date,
                        Status = status,
                        Note = faker.Random.Bool(0.2f) ? faker.Lorem.Sentence() : null,
                    });
                }
            }

            return attendances;
        }
        public static List<Assignment> GenerateAssignments(List<ClassRoom> classRooms, List<User> teachers, List<Subject> subjects, int count = 50)
        {
            var faker = new Faker();
            var assignments = new List<Assignment>();

            for (int i = 1; i <= count; i++)
            {
                var classRoom = faker.PickRandom(classRooms);
                var createdBy = faker.PickRandom(teachers);
                var subject = faker.PickRandom(subjects);

                assignments.Add(new Assignment
                {
                    Title = faker.Lorem.Sentence(3, 2),
                    FilePath = $"file.pdf",
                    Deadline = faker.Date.Future(1),
                    ClassRoomId = classRoom.Id,
                    CreatedById = createdBy.Id,
                    SubjectId = subject.Id,
                    IsActive = faker.Random.Bool(),
                    Mark = faker.Random.Float(5, 100),
                    ClassRoom = classRoom,
                    CreatedBy = createdBy,
                    Subject = subject
                });
            }

            return assignments;
        }
        public static List<Announcement> GenerateAnnouncements(List<User> users, List<User> creators, int count = 100)
        {
            var faker = new Faker();
            var announcements = new List<Announcement>();

            for (int i = 1; i <= count; i++)
            {
                var user = faker.PickRandom(users);
                var createdBy = faker.PickRandom(creators);

                announcements.Add(new Announcement
                {
                    UserId = user.Id,
                    CreatedById = createdBy.Id,
                    Title = faker.Lorem.Sentence(3),
                    Content = faker.Lorem.Paragraph(),
                    User = user,
                    CreatedBy = createdBy
                });
            }

            return announcements;
        }
        public static List<Fee> GenerateFees(List<User> students, List<User> creators, int count = 50)
        {
            var faker = new Faker();
            var fees = new List<Fee>();

            for (int i = 1; i <= count; i++)
            {
                var student = faker.PickRandom(students);
                var createdBy = faker.PickRandom(creators);
                var total = faker.Finance.Amount(500, 5000);
                var paid = faker.Random.Bool(0.7f)
                    ? total
                    : faker.Finance.Amount(0, total);

                fees.Add(new Fee
                {
                    StudentId = student.Id,
                    CreatedById = createdBy.Id,
                    Student = student,
                    CreatedBy = createdBy,
                    TotalAmount = total,
                    PaidAmount = Math.Round(paid, 2),
                    DueDate = faker.Date.Future(),
                    Description = faker.Lorem.Sentence(6)
                });
            }

            return fees;
        }
        public static List<StudentGuardian> GenerateStudentGuardians(List<User> students, List<User> guardians, int count = 50)
        {
            var faker = new Faker();
            var relations = new[] { "Father", "Mother", "Uncle", "Aunt", "Brother", "Sister", "Cousin", "Guardian" };
            var pairs = new HashSet<(int studentId, int guardianId)>();
            var list = new List<StudentGuardian>();

            foreach (var guardian in guardians)
            {
                for (int i = 0; i < count; i++)
                {
                    var student = faker.PickRandom(students);


                    if (student.Id == guardian.Id || pairs.Contains((student.Id, guardian.Id)))
                        continue;

                    pairs.Add((student.Id, guardian.Id));

                    list.Add(new StudentGuardian
                    {
                        StudentId = student.Id,
                        GuardianId = guardian.Id,
                        Student = student,
                        Guardian = guardian,
                        RelationToStudent = faker.PickRandom(relations)
                    });
                }

            }

            return list;
        }
    }

    public sealed class DataSeeder
    {
        public static DataSeeder Instance { get; } = new DataSeeder();

        public async Task ResetDatabaseWithGeneratedDataAsync(DataBaseContext context)
        {
            await context.Database.EnsureCreatedAsync();

            var users = Data.GenerateUsers();
            var classRooms = Data.GenerateClassRooms();
            var subjects = Data.GenerateSubjects();

            var students = users.Where(u => u.Role == eRole.Student).ToList();
            var teachers = users.Where(u => u.Role == eRole.Teacher).ToList();
            var guardians = users.Where(u => u.Role == eRole.Guardian).ToList();
            var admins = users.Where(u => u.Role == eRole.Admin).ToList();

            var enrollments = Data.GenerateEnrollments(classRooms, students);
            var timetables = Data.GenerateTimetables(classRooms, teachers, subjects);
            var attendance = Data.GenerateAttendance(students, teachers.Concat(admins).ToList(), classRooms);
            var assignments = Data.GenerateAssignments(classRooms, teachers, subjects);
            var announcements = Data.GenerateAnnouncements(users, admins.Concat(teachers).ToList());
            var fees = Data.GenerateFees(students, admins);
            var studentGuardians = Data.GenerateStudentGuardians(students, guardians);


            if (!context.Users.Any())
                await context.Users.AddRangeAsync(users);

            if (!context.ClassRooms.Any())
                await context.ClassRooms.AddRangeAsync(classRooms);

            if (!context.Subjects.Any())
                await context.Subjects.AddRangeAsync(subjects);

            if (!context.Enrollments.Any())
                await context.Enrollments.AddRangeAsync(enrollments);

            if (!context.Timetables.Any())
                await context.Timetables.AddRangeAsync(timetables);

            if (!context.Attendances.Any())
                await context.Attendances.AddRangeAsync(attendance);

            if (!context.Announcements.Any())
                await context.Assignments.AddRangeAsync(assignments);

            if (!context.Announcements.Any())
                await context.Announcements.AddRangeAsync(announcements);

            if (!context.Fees.Any())
                await context.Fees.AddRangeAsync(fees);

            if (!context.Guardians.Any())
                await context.Guardians.AddRangeAsync(studentGuardians);
            await context.SaveChangesAsync();
        }


    }
}
