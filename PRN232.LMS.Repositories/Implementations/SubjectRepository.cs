using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Base;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(LmsDbContext context) : base(context) { }

    public async Task<Subject?> GetByCodeAsync(string code)
        => await _dbSet.FirstOrDefaultAsync(x => x.Code == code);

    public async Task<IEnumerable<Subject>> SearchAsync(string? code, string? name)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(code))
            query = query.Where(x => x.Code.Contains(code));
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(x => x.Name.Contains(name));
        return await query.OrderBy(x => x.Code).ToListAsync();
    }
}
