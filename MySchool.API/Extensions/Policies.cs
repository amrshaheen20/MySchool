using MySchool.API.Enums;

namespace MySchool.API.Extensions
{
    public class Policies
    {
        public const string Admin = nameof(Admin);
        public const string Teacher = nameof(Teacher);
        public const string Student = nameof(Student);
        public const string Guardian = nameof(Guardian);
        public const string AllUsers = nameof(AllUsers);
        public const string Moderator = nameof(Moderator);

        public static readonly Dictionary<string, string[]> PolicyRolesMap = new()
        {
            { Admin,     new[] { nameof(eRole.Admin) } },
            { Teacher,   new[] { nameof(eRole.Teacher) } },
            { Student,   new[] { nameof(eRole.Student) } },
            { Guardian,  new[] { nameof(eRole.Guardian) } },

            { AllUsers,  new[]
                {
                    nameof(eRole.Admin),
                    nameof(eRole.Teacher),
                    nameof(eRole.Student),
                    nameof(eRole.Guardian)
                }
            },

            { Moderator, new[]
                {
                    nameof(eRole.Admin),
                    nameof(eRole.Teacher)
                }
            }
        };
    }
}
