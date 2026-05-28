namespace PRN232.LMS.Repositories.Entities;

public enum EnrollmentStatus
{
    Enrolled = 0,
    Completed = 1,
    Dropped = 2,
    Failed = 3
}

public class Enrollment
{
    public int Id { get; set; }
    public DateTime EnrolledDate { get; set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;
    public double? Grade { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
}
