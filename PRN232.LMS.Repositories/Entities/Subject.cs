namespace PRN232.LMS.Repositories.Entities;

public class Subject
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;       // e.g. "PRN232"
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Credits { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
