using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data;

public class LmsDbContext : DbContext
{
    public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options) { }

    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Semester
        modelBuilder.Entity<Semester>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
        });

        // Subject
        modelBuilder.Entity<Subject>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).IsRequired().HasMaxLength(20);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Code).IsUnique();
        });

        // Course
        modelBuilder.Entity<Course>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).IsRequired().HasMaxLength(20);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.HasOne(x => x.Subject).WithMany(x => x.Courses)
                .HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Semester).WithMany(x => x.Courses)
                .HasForeignKey(x => x.SemesterId).OnDelete(DeleteBehavior.Restrict);
        });

        // Student
        modelBuilder.Entity<Student>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.StudentCode).IsRequired().HasMaxLength(20);
            e.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).IsRequired().HasMaxLength(100);
            e.HasIndex(x => x.StudentCode).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });

        // Enrollment
        modelBuilder.Entity<Enrollment>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Student).WithMany(x => x.Enrollments)
                .HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Course).WithMany(x => x.Enrollments)
                .HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.StudentId, x.CourseId }).IsUnique();
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // 5 Semesters
        var semesters = new[]
        {
            new Semester { Id = 1, Name = "Spring2023", StartDate = new DateTime(2023,1,9), EndDate = new DateTime(2023,4,28), IsActive = false },
            new Semester { Id = 2, Name = "Summer2023", StartDate = new DateTime(2023,5,8), EndDate = new DateTime(2023,8,25), IsActive = false },
            new Semester { Id = 3, Name = "Fall2023",   StartDate = new DateTime(2023,9,4), EndDate = new DateTime(2023,12,22), IsActive = false },
            new Semester { Id = 4, Name = "Spring2024", StartDate = new DateTime(2024,1,8), EndDate = new DateTime(2024,4,26), IsActive = false },
            new Semester { Id = 5, Name = "Fall2024",   StartDate = new DateTime(2024,9,2), EndDate = new DateTime(2024,12,20), IsActive = true  },
        };
        modelBuilder.Entity<Semester>().HasData(semesters);

        // 10 Subjects
        var subjects = new[]
        {
            new Subject { Id = 1, Code = "PRN212", Name = "Basic Cross-Platform Application Programming With .NET", Credits = 3 },
            new Subject { Id = 2, Code = "PRN221", Name = "Advanced Cross-Platform Application Programming With .NET", Credits = 3 },
            new Subject { Id = 3, Code = "PRN231", Name = "ASP.NET Core MVC and RESTful API", Credits = 3 },
            new Subject { Id = 4, Code = "PRN232", Name = "ASP.NET Core Web API", Credits = 3 },
            new Subject { Id = 5, Code = "SWD392", Name = "Software Architecture and Design", Credits = 3 },
            new Subject { Id = 6, Code = "SWP391", Name = "Application Development Project", Credits = 3 },
            new Subject { Id = 7, Code = "SEP490", Name = "Capstone Project", Credits = 3 },
            new Subject { Id = 8, Code = "MAE101", Name = "Mathematics for Engineering", Credits = 3 },
            new Subject { Id = 9, Code = "ENW492", Name = "English for IT", Credits = 3 },
            new Subject { Id = 10, Code = "SSL101", Name = "Academic Skills for University Success", Credits = 3 },
        };
        modelBuilder.Entity<Subject>().HasData(subjects);

        // 20 Courses (4 per semester)
        var courses = new List<Course>();
        int cId = 1;
        var courseTemplates = new[] {
            (SubjectId: 1, Name: "PRN212", MaxStudents: 30, Room: "A101", Schedule: "Mon-Wed 7:30-9:30"),
            (SubjectId: 2, Name: "PRN221", MaxStudents: 30, Room: "A102", Schedule: "Tue-Thu 9:30-11:30"),
            (SubjectId: 3, Name: "PRN231", MaxStudents: 30, Room: "B201", Schedule: "Mon-Wed 13:00-15:00"),
            (SubjectId: 4, Name: "PRN232", MaxStudents: 30, Room: "B202", Schedule: "Fri 7:30-11:30"),
        };
        for (int s = 1; s <= 5; s++)
        {
            for (int t = 0; t < 4; t++)
            {
                var tpl = courseTemplates[t];
                courses.Add(new Course
                {
                    Id = cId++,
                    Code = $"SE{1000 + cId}",
                    Name = $"{tpl.Name} - Group {s}{(char)('A' + t)}",
                    SubjectId = tpl.SubjectId,
                    SemesterId = s,
                    MaxStudents = tpl.MaxStudents,
                    Room = tpl.Room,
                    Schedule = tpl.Schedule,
                    IsActive = s == 5
                });
            }
        }
        modelBuilder.Entity<Course>().HasData(courses);

        // 50 Students
        var students = new List<Student>();
        var firstNames = new[] { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Vu", "Dang", "Bui", "Do", "Ho" };
        var lastNames = new[] { "Van An", "Thi Bich", "Minh Chau", "Duc Dat", "Thi Em", "Van Phuc", "Thi Giang", "Minh Hai", "Van Lan", "Thi Mai" };
        for (int i = 1; i <= 50; i++)
        {
            students.Add(new Student
            {
                Id = i,
                StudentCode = $"SE{180000 + i}",
                FullName = $"{firstNames[(i - 1) % 10]} {lastNames[(i - 1) % 10]}",
                Email = $"se{180000 + i}@fpt.edu.vn",
                Phone = $"09{i:D8}",
                DateOfBirth = new DateTime(2002, (i % 12) + 1, (i % 28) + 1),
                Address = $"District {(i % 12) + 1}, Ho Chi Minh City",
                IsActive = true
            });
        }
        modelBuilder.Entity<Student>().HasData(students);

        // 500 Enrollments (25 students per course, 20 courses)
        var enrollments = new List<Enrollment>();
        int eId = 1;
        var random = new Random(42);
        var statuses = Enum.GetValues<EnrollmentStatus>();
        for (int cIdx = 0; cIdx < courses.Count; cIdx++)
        {
            var course = courses[cIdx];
            var selectedStudents = Enumerable.Range(1, 50)
                .OrderBy(_ => random.Next())
                .Take(25)
                .OrderBy(x => x)
                .ToList();

            foreach (var sid in selectedStudents)
            {
                var status = course.SemesterId < 5
                    ? (EnrollmentStatus)random.Next(1, 4)   // completed/dropped/failed for past
                    : EnrollmentStatus.Enrolled;
                double? grade = status == EnrollmentStatus.Completed
                    ? Math.Round(random.NextDouble() * 6 + 4, 1)  // 4.0 - 10.0
                    : null;

                enrollments.Add(new Enrollment
                {
                    Id = eId++,
                    StudentId = sid,
                    CourseId = course.Id,
                    EnrolledDate = course.SemesterId switch
                    {
                        1 => new DateTime(2023, 1, 5),
                        2 => new DateTime(2023, 5, 2),
                        3 => new DateTime(2023, 9, 1),
                        4 => new DateTime(2024, 1, 4),
                        _ => new DateTime(2024, 8, 28)
                    },
                    Status = status,
                    Grade = grade
                });
            }
        }
        modelBuilder.Entity<Enrollment>().HasData(enrollments);
    }
}
