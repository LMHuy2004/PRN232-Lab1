using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Subjects;

namespace PRN232.LMS.Services.Implementations;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repo;
    private readonly LmsDbContext _context;

    public SubjectService(ISubjectRepository repo, LmsDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<PagedResult<SubjectResponse>> GetAllAsync(SubjectQuery query)
    {
        var items = await _repo.SearchAsync(query.Code, query.Name);
        items = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("name", "asc") => items.OrderBy(x => x.Name),
            ("name", _)     => items.OrderByDescending(x => x.Name),
            (_, "desc")     => items.OrderByDescending(x => x.Code),
            _               => items.OrderBy(x => x.Code),
        };

        var total = items.Count();
        var paged = items.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();
        var expandCourses = query.Expand?.ToLower().Contains("courses") == true;
        var responses = paged.Select(x => ToResponse(ToBusinessModel(x, expandCourses))).ToList();

        return new PagedResult<SubjectResponse>
        {
            Items = responses,
            Pagination = PaginationMeta.Create(query.Page, query.PageSize, total)
        };
    }

    public async Task<SubjectResponse?> GetByIdAsync(int id)
    {
        var item = await _context.Subjects
            .Include(x => x.Courses).ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return null;
        return ToResponse(ToBusinessModel(item, includeCourses: true));
    }

    public async Task<SubjectResponse> CreateAsync(CreateSubjectRequest req)
    {
        var entity = new Subject { Code = req.Code, Name = req.Name, Description = req.Description, Credits = req.Credits };
        return ToResponse(ToBusinessModel(await _repo.CreateAsync(entity), false));
    }

    public async Task<SubjectResponse?> UpdateAsync(int id, UpdateSubjectRequest req)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;
        entity.Name = req.Name; entity.Description = req.Description; entity.Credits = req.Credits;
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

    private static SubjectModel ToBusinessModel(Subject x, bool includeCourses) => new()
    {
        Id = x.Id, Code = x.Code, Name = x.Name, Description = x.Description,
        Credits = x.Credits, CourseCount = x.Courses?.Count ?? 0,
        Courses = includeCourses ? x.Courses?.Select(c => new SubjectCourseModel
        {
            CourseId = c.Id, CourseCode = c.Code, CourseName = c.Name, SemesterName = c.Semester?.Name
        }).ToList() : null
    };

    private static SubjectResponse ToResponse(SubjectModel m) => new()
    {
        Id = m.Id, Code = m.Code, Name = m.Name, Description = m.Description,
        Credits = m.Credits, CourseCount = m.CourseCount, Courses = m.Courses
    };
}
