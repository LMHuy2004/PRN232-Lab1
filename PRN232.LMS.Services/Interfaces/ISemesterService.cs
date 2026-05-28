using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Semesters;

namespace PRN232.LMS.Services.Interfaces;

public interface ISemesterService
{
    Task<PagedResult<SemesterResponse>> GetAllAsync(SemesterQuery query);
    Task<SemesterResponse?> GetByIdAsync(int id);
    Task<SemesterResponse> CreateAsync(CreateSemesterRequest request);
    Task<SemesterResponse?> UpdateAsync(int id, UpdateSemesterRequest request);
    Task<bool> DeleteAsync(int id);
}
