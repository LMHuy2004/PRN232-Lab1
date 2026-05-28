using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Base;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class EnrollmentRepository : BaseRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(LmsDbContext context) : base(context) { }

    public async Task<Enrollment?> GetByIdWithDetailsAsync(int id)
        => await _dbSet
            .Include(x => x.Student)
            .Include(x => x.Course).ThenInclude(c => c.Subject)
            .Include(x => x.Course).ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId)
        => await _dbSet
            .Include(x => x.Course).ThenInclude(c => c.Subject)
            .Include(x => x.Course).ThenInclude(c => c.Semester)
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.EnrolledDate)
            .ToListAsync();

    public async Task<IEnumerable<Enrollment>> GetByCourseAsync(int courseId)
        => await _dbSet
            .Include(x => x.Student)
            .Where(x => x.CourseId == courseId)
            .OrderBy(x => x.Student.StudentCode)
            .ToListAsync();

    public async Task<IEnumerable<Enrollment>> SearchAsync(int? studentId, int? courseId, EnrollmentStatus? status)
    {
        var query = _dbSet
            .Include(x => x.Student)
            .Include(x => x.Course).ThenInclude(c => c.Subject)
            .AsQueryable();
        if (studentId.HasValue)
            query = query.Where(x => x.StudentId == studentId.Value);
        if (courseId.HasValue)
            query = query.Where(x => x.CourseId == courseId.Value);
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        return await query.OrderByDescending(x => x.EnrolledDate).ToListAsync();
    }

    public async Task<bool> ExistsByStudentAndCourseAsync(int studentId, int courseId)
        => await _dbSet.AnyAsync(x => x.StudentId == studentId && x.CourseId == courseId);
}
