using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.DbSet.ExamEntities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MySchool.API
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValueSqlAttribute : Attribute
    {
        public string Sql { get; }
        public DefaultValueSqlAttribute(string sql) => Sql = sql;
    }
    public class DataBaseContext(DbContextOptions<DataBaseContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<StudentGuardian> Guardians { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Timetable> Timetables { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            ApplyDefaultValueSqlAttributes(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


        private void ApplyDefaultValueSqlAttributes(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (clrType == null)
                    continue;

                var entityBuilder = modelBuilder.Entity(clrType);

                var properties = clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in properties)
                {
                    var attr = prop.GetCustomAttribute<DefaultValueSqlAttribute>(inherit: true);
                    if (attr != null)
                    {
                        entityBuilder.Property(prop.Name).HasDefaultValueSql(attr.Sql);
                    }
                }
            }
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
