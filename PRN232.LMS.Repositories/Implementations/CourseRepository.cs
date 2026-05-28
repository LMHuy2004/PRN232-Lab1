using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Base;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(LmsDbContext context) : base(context) { }

    public async Task<Course?> GetByIdWithDetailsAsync(int id)
        => await _dbSet
            .Include(x => x.Subject)
            .Include(x => x.Semester)
            .Include(x => x.Enrollments).ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<Course>> SearchAsync(string? name, int? semesterId, int? subjectId, bool? isActive)
    {
        var query = _dbSet.Include(x => x.Subject).Include(x => x.Semester).AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(x => x.Name.Contains(name) || x.Code.Contains(name));
        if (semesterId.HasValue)
            query = query.Where(x => x.SemesterId == semesterId.Value);
        if (subjectId.HasValue)
            query = query.Where(x => x.SubjectId == subjectId.Value);
        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);
        return await query.OrderBy(x => x.Code).ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetBySemesterAsync(int semesterId)
        => await _dbSet
            .Include(x => x.Subject)
            .Where(x => x.SemesterId == semesterId)
            .OrderBy(x => x.Code)
            .ToListAsync();
}
