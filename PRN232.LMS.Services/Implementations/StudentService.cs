using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Students;

namespace PRN232.LMS.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;
    private readonly LmsDbContext _context;

    public StudentService(IStudentRepository repo, LmsDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<PagedResult<StudentResponse>> GetAllAsync(StudentQuery query)
    {
        var items = await _repo.SearchAsync(query.Name, query.Email, query.IsActive);
        items = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("fullname", "asc") => items.OrderBy(x => x.FullName),
            ("fullname", _)     => items.OrderByDescending(x => x.FullName),
            ("email", "asc")    => items.OrderBy(x => x.Email),
            ("email", _)        => items.OrderByDescending(x => x.Email),
            (_, "desc")         => items.OrderByDescending(x => x.StudentCode),
            _                   => items.OrderBy(x => x.StudentCode),
        };

        var total = items.Count();
        var paged = items.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();

        var expandEnrollments = query.Expand?.ToLower().Contains("enrollments") == true;
        var responses = paged.Select(x => ToResponse(ToBusinessModel(x, expandEnrollments))).ToList();

        return new PagedResult<StudentResponse>
        {
            Items = responses,
            Pagination = PaginationMeta.Create(query.Page, query.PageSize, total)
        };
    }

    public async Task<StudentResponse?> GetByIdAsync(int id)
    {
        var item = await _context.Students
            .Include(x => x.Enrollments)
                .ThenInclude(e => e.Course).ThenInclude(c => c.Subject)
            .Include(x => x.Enrollments)
                .ThenInclude(e => e.Course).ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return null;
        return ToResponse(ToBusinessModel(item, includeCourses: true));
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest req)
    {
        var entity = new Student
        {
            StudentCode = req.StudentCode, FullName = req.FullName, Email = req.Email,
            Phone = req.Phone, DateOfBirth = req.DateOfBirth, Address = req.Address, IsActive = true
        };
        return ToResponse(ToBusinessModel(await _repo.CreateAsync(entity), false));
    }

    public async Task<StudentResponse?> UpdateAsync(int id, UpdateStudentRequest req)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;
        entity.FullName = req.FullName; entity.Email = req.Email; entity.Phone = req.Phone;
        entity.DateOfBirth = req.DateOfBirth; entity.Address = req.Address; entity.IsActive = req.IsActive;
        await _repo.UpdateAsync(entity);
        return ToResponse(ToBusinessModel(entity, false));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;
        await _repo.DeleteAsync(entity);
        return true;
    }

    // Entity → Business Model
    private static StudentModel ToBusinessModel(Student x, bool includeCourses) => new()
    {
        Id = x.Id, StudentCode = x.StudentCode, FullName = x.FullName,
        Email = x.Email, Phone = x.Phone, DateOfBirth = x.DateOfBirth,
        Address = x.Address, IsActive = x.IsActive,
        EnrollmentCount = x.Enrollments?.Count ?? 0,
        Enrollments = includeCourses ? x.Enrollments?.Select(e => new StudentEnrollmentModel
        {
            EnrollmentId = e.Id,
            CourseCode = e.Course?.Code ?? "",
            CourseName = e.Course?.Name ?? "",
            SubjectCode = e.Course?.Subject?.Code,
            SemesterName = e.Course?.Semester?.Name,
            Status = e.Status.ToString(),
            Grade = e.Grade
        }).ToList() : null
    };

    // Business Model → Response Model
    private static StudentResponse ToResponse(StudentModel m) => new()
    {
        Id = m.Id, StudentCode = m.StudentCode, FullName = m.FullName,
        Email = m.Email, Phone = m.Phone, DateOfBirth = m.DateOfBirth,
        Address = m.Address, IsActive = m.IsActive,
        EnrollmentCount = m.EnrollmentCount, Enrollments = m.Enrollments
    };
}
