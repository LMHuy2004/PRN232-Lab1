using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Students;

namespace PRN232.LMS.Services.Interfaces;

public interface IStudentService
{
    Task<PagedResult<StudentResponse>> GetAllAsync(StudentQuery query);
    Task<StudentResponse?> GetByIdAsync(int id);
    Task<StudentResponse> CreateAsync(CreateStudentRequest request);
    Task<StudentResponse?> UpdateAsync(int id, UpdateStudentRequest request);
    Task<bool> DeleteAsync(int id);
}
