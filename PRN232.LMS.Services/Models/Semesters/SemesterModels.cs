namespace PRN232.LMS.Services.Models.Semesters;

// ── Business Model ───────────────────────────────────────────────
public class SemesterModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int CourseCount { get; set; }
    public List<SemesterCourseModel>? Courses { get; set; }
}

public class SemesterCourseModel
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string? SubjectCode { get; set; }
}

// ── Response Model ───────────────────────────────────────────────
public class SemesterResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int CourseCount { get; set; }
    public List<SemesterCourseModel>? Courses { get; set; }
}

// ── Request Models ───────────────────────────────────────────────
public class CreateSemesterRequest
{
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateSemesterRequest
{
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}

// ── Query Model ──────────────────────────────────────────────────
public class SemesterQuery
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "startdate";
    public string? SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Fields { get; set; }
    public string? Expand { get; set; }
}
