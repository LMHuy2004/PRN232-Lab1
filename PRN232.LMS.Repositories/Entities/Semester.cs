namespace PRN232.LMS.Repositories.Entities;

public class Semester
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;       // e.g. "Fall2024", "Spring2025"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
