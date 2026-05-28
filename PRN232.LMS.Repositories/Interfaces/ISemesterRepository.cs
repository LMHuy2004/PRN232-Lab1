using PRN232.LMS.Repositories.Base;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISemesterRepository : IBaseRepository<Semester>
{
    Task<IEnumerable<Semester>> SearchAsync(string? name, bool? isActive);
}
