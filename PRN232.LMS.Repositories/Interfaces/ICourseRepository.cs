using PRN232.LMS.Repositories.Base;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ICourseRepository : IBaseRepository<Course>
{
    Task<Course?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Course>> SearchAsync(string? name, int? semesterId, int? subjectId, bool? isActive);
    Task<IEnumerable<Course>> GetBySemesterAsync(int semesterId);
}
