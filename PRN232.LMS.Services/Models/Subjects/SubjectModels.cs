namespace PRN232.LMS.Services.Models.Subjects;

// ── Business Model ───────────────────────────────────────────────
public class SubjectModel
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Credits { get; set; }
    public int CourseCount { get; set; }
    public List<SubjectCourseModel>? Courses { get; set; }
}

public class SubjectCourseModel
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string? SemesterName { get; set; }
}

// ── Response Model ───────────────────────────────────────────────
public class SubjectResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Credits { get; set; }
    public int CourseCount { get; set; }
    public List<SubjectCourseModel>? Courses { get; set; }
}

// ── Request Models ───────────────────────────────────────────────
public class CreateSubjectRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Credits { get; set; }
}

public class UpdateSubjectRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Credits { get; set; }
}

// ── Query Model ──────────────────────────────────────────────────
public class SubjectQuery
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? SortBy { get; set; } = "code";
    public string? SortOrder { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Fields { get; set; }
    public string? Expand { get; set; }
}
