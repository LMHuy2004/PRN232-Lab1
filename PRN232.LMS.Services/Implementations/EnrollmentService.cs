using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Enrollments;

namespace PRN232.LMS.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repo;
    private readonly LmsDbContext _context;

    public EnrollmentService(IEnrollmentRepository repo, LmsDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<PagedResult<EnrollmentResponse>> GetAllAsync(EnrollmentQuery query)
    {
        var items = await _repo.SearchAsync(query.StudentId, query.CourseId, (EnrollmentStatus?)query.Status);
        items = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("enrolleddate", "asc") => items.OrderBy(x => x.EnrolledDate),
            ("grade", "asc")        => items.OrderBy(x => x.Grade),
            ("grade", _)            => items.OrderByDescending(x => x.Grade),
            _                       => items.OrderByDescending(x => x.EnrolledDate),
        };

        var total = items.Count();
        var paged = items.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();
        var responses = paged.Select(x => ToResponse(ToBusinessModel(x))).ToList();

        return new PagedResult<EnrollmentResponse>
        {
            Items = responses,
            Pagination = PaginationMeta.Create(query.Page, query.PageSize, total)
        };
    }

    public async Task<EnrollmentResponse?> GetByIdAsync(int id)
    {
        var item = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course).ThenInclude(c => c.Subject)
            .Include(e => e.Course).ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (item == null) return null;
        return ToResponse(ToBusinessModel(item));
    }

    public async Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest req)
    {
        var entity = new Enrollment
        {
            StudentId = req.StudentId, CourseId = req.CourseId,
            EnrolledDate = DateTime.UtcNow, Status = EnrollmentStatus.Enrolled
        };
        var created = await _repo.CreateAsync(entity);
        var full = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course).ThenInclude(c => c.Subject)
            .Include(e => e.Course).ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(e => e.Id == created.Id);
        return ToResponse(ToBusinessModel(full!));
    }

    public async Task<EnrollmentResponse?> UpdateAsync(int id, UpdateEnrollmentRequest req)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;
        entity.Status = req.Status; entity.Grade = req.Grade;
        await _repo.UpdateAsync(entity);
        var full = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course).ThenInclude(c => c.Subject)
            .Include(e => e.Course).ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(e => e.Id == id);
        return ToResponse(ToBusinessModel(full!));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;
        await _repo.DeleteAsync(entity);
        return true;
    }

    private static EnrollmentModel ToBusinessModel(Enrollment e) => new()
    {
        Id = e.Id, EnrolledDate = e.EnrolledDate, Status = e.Status.ToString(), Grade = e.Grade,
        StudentId = e.StudentId, StudentCode = e.Student?.StudentCode ?? "",
        StudentName = e.Student?.FullName ?? "", CourseId = e.CourseId,
        CourseCode = e.Course?.Code ?? "", CourseName = e.Course?.Name ?? "",
        SubjectCode = e.Course?.Subject?.Code, SemesterName = e.Course?.Semester?.Name
    };

    private static EnrollmentResponse ToResponse(EnrollmentModel m) => new()
    {
        Id = m.Id, EnrolledDate = m.EnrolledDate, Status = m.Status, Grade = m.Grade,
        StudentId = m.StudentId, StudentCode = m.StudentCode, StudentName = m.StudentName,
        CourseId = m.CourseId, CourseCode = m.CourseCode, CourseName = m.CourseName,
        SubjectCode = m.SubjectCode, SemesterName = m.SemesterName
    };
}
