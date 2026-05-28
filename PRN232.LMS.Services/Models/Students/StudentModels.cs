namespace PRN232.LMS.Services.Models.Students;

// ── Entity → Business Model (service-level processing) ──────────
public class StudentModel
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public int EnrollmentCount { get; set; }
    // Related data (for expand)
    public List<StudentEnrollmentModel>? Enrollments { get; set; }
}

public class StudentEnrollmentModel
{
    public int EnrollmentId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string? SubjectCode { get; set; }
    public string? SemesterName { get; set; }
    public string Status { get; set; } = null!;
    public double? Grade { get; set; }
}

// ── Business Model → Response Model ─────────────────────────────
public class StudentResponse
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public int EnrollmentCount { get; set; }
    // Expanded data (only populated when ?expand=enrollments)
    public List<StudentEnrollmentModel>? Enrollments { get; set; }
}

// ── Request Models ───────────────────────────────────────────────
public class CreateStudentRequest
{
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "StudentCode is required")]
    [System.ComponentModel.DataAnnotations.StringLength(20, ErrorMessage = "StudentCode max 20 chars")]
    public string StudentCode { get; set; } = null!;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "FullName is required")]
    public string FullName { get; set; } = null!;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email is required")]
    [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

    public string? Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
}

public class UpdateStudentRequest
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
}

// ── Query Model ──────────────────────────────────────────────────
public class StudentQuery
{
    // Search/filter
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
    // Sort
    public string? SortBy { get; set; } = "studentcode";
    public string? SortOrder { get; set; } = "asc";
    // Paging
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    // Fields selection: e.g. fields=id,fullName,email
    public string? Fields { get; set; }
    // Expand related: e.g. expand=enrollments
    public string? Expand { get; set; }
}
