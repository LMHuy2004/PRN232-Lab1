using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Services.Models.Enrollments;

// ── Business Model ───────────────────────────────────────────────
public class EnrollmentModel
{
    public int Id { get; set; }
    public DateTime EnrolledDate { get; set; }
    public string Status { get; set; } = null!;
    public double? Grade { get; set; }
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string? SubjectCode { get; set; }
    public string? SemesterName { get; set; }
}

// ── Response Model ───────────────────────────────────────────────
public class EnrollmentResponse
{
    public int Id { get; set; }
    public DateTime EnrolledDate { get; set; }
    public string Status { get; set; } = null!;
    public double? Grade { get; set; }
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string? SubjectCode { get; set; }
    public string? SemesterName { get; set; }
}

// ── Request Models ───────────────────────────────────────────────
public class CreateEnrollmentRequest
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
}

public class UpdateEnrollmentRequest
{
    public EnrollmentStatus Status { get; set; }
    public double? Grade { get; set; }
}

// ── Query Model ──────────────────────────────────────────────────
public class EnrollmentQuery
{
    public int? StudentId { get; set; }
    public int? CourseId { get; set; }
    public EnrollmentStatus? Status { get; set; }
    public string? SortBy { get; set; } = "enrolleddate";
    public string? SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Fields { get; set; }
    public string? Expand { get; set; }
}
