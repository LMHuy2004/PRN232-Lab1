using PRN232.LMS.Repositories.Base;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IEnrollmentRepository : IBaseRepository<Enrollment>
{
    Task<Enrollment?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId);
    Task<IEnumerable<Enrollment>> GetByCourseAsync(int courseId);
    Task<IEnumerable<Enrollment>> SearchAsync(int? studentId, int? courseId, EnrollmentStatus? status);
    Task<bool> ExistsByStudentAndCourseAsync(int studentId, int courseId);
}
