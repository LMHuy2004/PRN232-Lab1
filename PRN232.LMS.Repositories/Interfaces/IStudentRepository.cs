using PRN232.LMS.Repositories.Base;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IStudentRepository : IBaseRepository<Student>
{
    Task<Student?> GetByCodeAsync(string studentCode);
    Task<Student?> GetByIdWithEnrollmentsAsync(int id);
    Task<IEnumerable<Student>> SearchAsync(string? name, string? email, bool? isActive);
}
