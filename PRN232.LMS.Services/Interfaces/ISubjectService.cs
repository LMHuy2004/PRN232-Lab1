using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Subjects;

namespace PRN232.LMS.Services.Interfaces;

public interface ISubjectService
{
    Task<PagedResult<SubjectResponse>> GetAllAsync(SubjectQuery query);
    Task<SubjectResponse?> GetByIdAsync(int id);
    Task<SubjectResponse> CreateAsync(CreateSubjectRequest request);
    Task<SubjectResponse?> UpdateAsync(int id, UpdateSubjectRequest request);
    Task<bool> DeleteAsync(int id);
}
