namespace PRN232.LMS.Repositories.Entities;

public class Student
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = null!;   // e.g. "SE182000"
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
