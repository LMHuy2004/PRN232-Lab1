using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Enrollments;

namespace PRN232.LMS.Services.Interfaces;

public interface IEnrollmentService
{
    Task<PagedResult<EnrollmentResponse>> GetAllAsync(EnrollmentQuery query);
    Task<EnrollmentResponse?> GetByIdAsync(int id);
    Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request);
    Task<EnrollmentResponse?> UpdateAsync(int id, UpdateEnrollmentRequest request);
    Task<bool> DeleteAsync(int id);
}
