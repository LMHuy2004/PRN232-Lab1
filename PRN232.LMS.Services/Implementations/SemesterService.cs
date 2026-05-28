using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Semesters;

namespace PRN232.LMS.Services.Implementations;

public class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _repo;
    private readonly LmsDbContext _context;

    public SemesterService(ISemesterRepository repo, LmsDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<PagedResult<SemesterResponse>> GetAllAsync(SemesterQuery query)
    {
        var items = await _repo.SearchAsync(query.Name, query.IsActive);
        items = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("name", "asc")      => items.OrderBy(x => x.Name),
            ("name", _)          => items.OrderByDescending(x => x.Name),
            ("startdate", "asc") => items.OrderBy(x => x.StartDate),
            _                    => items.OrderByDescending(x => x.StartDate),
        };

        var total = items.Count();
        var paged = items.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();

        // Map Entity → Business Model → Response Model
        var expandCourses = query.Expand?.ToLower().Contains("courses") == true;
        var businessModels = paged.Select(x => ToBusinessModel(x, expandCourses));
        var responses = businessModels.Select(ToResponse).ToList();

        return new PagedResult<SemesterResponse>
        {
            Items = responses,
            Pagination = PaginationMeta.Create(query.Page, query.PageSize, total)
        };
    }

    public async Task<SemesterResponse?> GetByIdAsync(int id)
    {
        var item = await _context.Semesters
            .Include(x => x.Courses).ThenInclude(c => c.Subject)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return null;
        return ToResponse(ToBusinessModel(item, includeCourses: true));
    }

    public async Task<SemesterResponse> CreateAsync(CreateSemesterRequest req)
    {
        var entity = new Semester { Name = req.Name, StartDate = req.StartDate, EndDate = req.EndDate, IsActive = req.IsActive };
        var created = await _repo.CreateAsync(entity);
        return ToResponse(ToBusinessModel(created, false));
    }

    public async Task<SemesterResponse?> UpdateAsync(int id, UpdateSemesterRequest req)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;
        entity.Name = req.Name; entity.StartDate = req.StartDate;
        entity.EndDate = req.EndDate; entity.IsActive = req.IsActive;
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
    private static SemesterModel ToBusinessModel(Semester x, bool includeCourses) => new()
    {
        Id = x.Id, Name = x.Name, StartDate = x.StartDate, EndDate = x.EndDate,
        IsActive = x.IsActive, CourseCount = x.Courses?.Count ?? 0,
        Courses = includeCourses ? x.Courses?.Select(c => new SemesterCourseModel
        {
            CourseId = c.Id, CourseCode = c.Code, CourseName = c.Name, SubjectCode = c.Subject?.Code
        }).ToList() : null
    };

    // Business Model → Response Model
    private static SemesterResponse ToResponse(SemesterModel m) => new()
    {
        Id = m.Id, Name = m.Name, StartDate = m.StartDate, EndDate = m.EndDate,
        IsActive = m.IsActive, CourseCount = m.CourseCount, Courses = m.Courses
    };
}
