namespace PRN232.LMS.Repositories.Entities;

public class Course
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;       // e.g. "SE1234"
    public string Name { get; set; } = null!;
    public int MaxStudents { get; set; }
    public string? Room { get; set; }
    public string? Schedule { get; set; }           // e.g. "Mon-Wed 7:30-9:30"
    public bool IsActive { get; set; } = true;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public int SemesterId { get; set; }
    public Semester Semester { get; set; } = null!;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
