using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Courses;
using PRN232.LMS.Services.Models.Enrollments;

namespace PRN232.LMS.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;
    private readonly LmsDbContext _context;

    public CourseService(ICourseRepository repo, LmsDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<PagedResult<CourseResponse>> GetAllAsync(CourseQuery query)
    {
        var items = await _repo.SearchAsync(query.Name, query.SemesterId, query.SubjectId, query.IsActive);
        items = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("name", "asc") => items.OrderBy(x => x.Name),
            ("name", _)     => items.OrderByDescending(x => x.Name),
            (_, "desc")     => items.OrderByDescending(x => x.Code),
            _               => items.OrderBy(x => x.Code),
        };

        var total = items.Count();
        var paged = items.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();
        var expandEnrollments = query.Expand?.ToLower().Contains("enrollments") == true;
        var responses = paged.Select(x => ToResponse(ToBusinessModel(x, expandEnrollments))).ToList();

        return new PagedResult<CourseResponse>
        {
            Items = responses,
            Pagination = PaginationMeta.Create(query.Page, query.PageSize, total)
        };
    }

    public async Task<CourseResponse?> GetByIdAsync(int id)
    {
        var item = await _context.Courses
            .Include(x => x.Subject)
            .Include(x => x.Semester)
            .Include(x => x.Enrollments).ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return null;
        return ToResponse(ToBusinessModel(item, includeEnrollments: true));
    }

    public async Task<CourseResponse> CreateAsync(CreateCourseRequest req)
    {
        var entity = new Course
        {
            Code = req.Code, Name = req.Name, SubjectId = req.SubjectId,
            SemesterId = req.SemesterId, MaxStudents = req.MaxStudents,
            Room = req.Room, Schedule = req.Schedule, IsActive = req.IsActive
        };
        var created = await _repo.CreateAsync(entity);
        // reload with Subject + Semester
        var full = await _context.Courses.Include(x => x.Subject).Include(x => x.Semester)
            .FirstOrDefaultAsync(x => x.Id == created.Id);
        return ToResponse(ToBusinessModel(full!, false));
    }

    public async Task<CourseResponse?> UpdateAsync(int id, UpdateCourseRequest req)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;
        entity.Name = req.Name; entity.MaxStudents = req.MaxStudents;
        entity.Room = req.Room; entity.Schedule = req.Schedule; entity.IsActive = req.IsActive;
        await _repo.UpdateAsync(entity);
        var full = await _context.Courses.Include(x => x.Subject).Include(x => x.Semester)
            .FirstOrDefaultAsync(x => x.Id == id);
        return ToResponse(ToBusinessModel(full!, false));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;
        await _repo.DeleteAsync(entity);
        return true;
    }

    public async Task<PagedResult<EnrollmentResponse>?> GetEnrollmentsAsync(int courseId, string? expand, int page, int pageSize)
    {
        // Return null if course doesn't exist
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists) return null;

        var expandStudent = expand?.ToLower().Contains("student") == true;

        var query = _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .Include(e => e.Course).ThenInclude(c => c.Subject)
            .Include(e => e.Course).ThenInclude(c => c.Semester)
            .AsQueryable();

        if (expandStudent)
            query = query.Include(e => e.Student);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(e => e.EnrolledDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var responses = items.Select(e => new EnrollmentResponse
        {
            Id          = e.Id,
            EnrolledDate = e.EnrolledDate,
            Status      = e.Status.ToString(),
            Grade       = e.Grade,
            CourseId    = e.CourseId,
            CourseCode  = e.Course?.Code ?? "",
            CourseName  = e.Course?.Name ?? "",
            SubjectCode = e.Course?.Subject?.Code,
            SemesterName = e.Course?.Semester?.Name,
            // Student info: only populated when ?expand=student
            StudentId   = e.StudentId,
            StudentCode = expandStudent ? (e.Student?.StudentCode ?? "") : "",
            StudentName = expandStudent ? (e.Student?.FullName    ?? "") : "",
        }).ToList();

        return new PagedResult<EnrollmentResponse>
        {
            Items      = responses,
            Pagination = PaginationMeta.Create(page, pageSize, total)
        };
    }


    private static CourseModel ToBusinessModel(Course x, bool includeEnrollments) => new()
    {
        Id = x.Id, Code = x.Code, Name = x.Name, MaxStudents = x.MaxStudents,
        Room = x.Room, Schedule = x.Schedule, IsActive = x.IsActive,
        EnrollmentCount = x.Enrollments?.Count ?? 0,
        SubjectId = x.SubjectId, SubjectCode = x.Subject?.Code, SubjectName = x.Subject?.Name,
        SemesterId = x.SemesterId, SemesterName = x.Semester?.Name,
        Enrollments = includeEnrollments ? x.Enrollments?.Select(e => new CourseEnrollmentModel
        {
            EnrollmentId = e.Id, StudentCode = e.Student?.StudentCode ?? "",
            StudentName = e.Student?.FullName ?? "", Status = e.Status.ToString(), Grade = e.Grade
        }).ToList() : null
    };

    private static CourseResponse ToResponse(CourseModel m) => new()
    {
        Id = m.Id, Code = m.Code, Name = m.Name, MaxStudents = m.MaxStudents,
        Room = m.Room, Schedule = m.Schedule, IsActive = m.IsActive,
        EnrollmentCount = m.EnrollmentCount, SubjectId = m.SubjectId,
        SubjectCode = m.SubjectCode, SubjectName = m.SubjectName,
        SemesterId = m.SemesterId, SemesterName = m.SemesterName, Enrollments = m.Enrollments
    };
}
