using PRN232.LMS.Repositories.Base;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISubjectRepository : IBaseRepository<Subject>
{
    Task<Subject?> GetByCodeAsync(string code);
    Task<IEnumerable<Subject>> SearchAsync(string? code, string? name);
}
