using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Base;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class StudentRepository : BaseRepository<Student>, IStudentRepository
{
    public StudentRepository(LmsDbContext context) : base(context) { }

    public async Task<Student?> GetByCodeAsync(string studentCode)
        => await _dbSet.FirstOrDefaultAsync(x => x.StudentCode == studentCode);

    public async Task<Student?> GetByIdWithEnrollmentsAsync(int id)
        => await _dbSet
            .Include(x => x.Enrollments).ThenInclude(e => e.Course).ThenInclude(c => c.Subject)
            .Include(x => x.Enrollments).ThenInclude(e => e.Course).ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<Student>> SearchAsync(string? name, string? email, bool? isActive)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(x => x.FullName.Contains(name) || x.StudentCode.Contains(name));
        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(x => x.Email.Contains(email));
        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);
        return await query.OrderBy(x => x.StudentCode).ToListAsync();
    }
}
