using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Base;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class SemesterRepository : BaseRepository<Semester>, ISemesterRepository
{
    public SemesterRepository(LmsDbContext context) : base(context) { }

    public async Task<IEnumerable<Semester>> SearchAsync(string? name, bool? isActive)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(x => x.Name.Contains(name));
        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);
        return await query.OrderByDescending(x => x.StartDate).ToListAsync();
    }
}
