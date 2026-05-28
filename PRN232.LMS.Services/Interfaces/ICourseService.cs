using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Courses;
using PRN232.LMS.Services.Models.Enrollments;

namespace PRN232.LMS.Services.Interfaces;

public interface ICourseService
{
    Task<PagedResult<CourseResponse>> GetAllAsync(CourseQuery query);
    Task<CourseResponse?> GetByIdAsync(int id);
    Task<CourseResponse> CreateAsync(CreateCourseRequest request);
    Task<CourseResponse?> UpdateAsync(int id, UpdateCourseRequest request);
    Task<bool> DeleteAsync(int id);

    /// <summary>Get enrollments of a course, optionally expand student details</summary>
    Task<PagedResult<EnrollmentResponse>?> GetEnrollmentsAsync(int courseId, string? expand, int page, int pageSize);
}
