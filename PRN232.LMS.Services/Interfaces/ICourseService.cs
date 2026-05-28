using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Courses;

namespace PRN232.LMS.Services.Interfaces;

public interface ICourseService
{
    Task<PagedResult<CourseResponse>> GetAllAsync(CourseQuery query);
    Task<CourseResponse?> GetByIdAsync(int id);
    Task<CourseResponse> CreateAsync(CreateCourseRequest request);
    Task<CourseResponse?> UpdateAsync(int id, UpdateCourseRequest request);
    Task<bool> DeleteAsync(int id);
}
