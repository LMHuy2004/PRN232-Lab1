namespace PRN232.LMS.Services.Models.Courses;

// ── Business Model ───────────────────────────────────────────────
public class CourseModel
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int MaxStudents { get; set; }
    public string? Room { get; set; }
    public string? Schedule { get; set; }
    public bool IsActive { get; set; }
    public int EnrollmentCount { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectCode { get; set; }
    public string? SubjectName { get; set; }
    public int SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public List<CourseEnrollmentModel>? Enrollments { get; set; }
}

public class CourseEnrollmentModel
{
    public int EnrollmentId { get; set; }
    public string StudentCode { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public double? Grade { get; set; }
}

// ── Response Model ───────────────────────────────────────────────
public class CourseResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int MaxStudents { get; set; }
    public string? Room { get; set; }
    public string? Schedule { get; set; }
    public bool IsActive { get; set; }
    public int EnrollmentCount { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectCode { get; set; }
    public string? SubjectName { get; set; }
    public int SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public List<CourseEnrollmentModel>? Enrollments { get; set; }
}

// ── Request Models ───────────────────────────────────────────────
public class CreateCourseRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int SubjectId { get; set; }
    public int SemesterId { get; set; }
    public int MaxStudents { get; set; } = 30;
    public string? Room { get; set; }
    public string? Schedule { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCourseRequest
{
    public string Name { get; set; } = null!;
    public int MaxStudents { get; set; }
    public string? Room { get; set; }
    public string? Schedule { get; set; }
    public bool IsActive { get; set; }
}

// ── Query Model ──────────────────────────────────────────────────
public class CourseQuery
{
    public string? Name { get; set; }
    public int? SemesterId { get; set; }
    public int? SubjectId { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "code";
    public string? SortOrder { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Fields { get; set; }
    public string? Expand { get; set; }
}
