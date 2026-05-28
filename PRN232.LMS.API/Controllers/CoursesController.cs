using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Courses;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;
    public CoursesController(ICourseService service) => _service = service;

    /// <summary>Get all courses with search, sort, paging</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CourseResponse>>>> GetAll([FromQuery] CourseQuery query)
        => Ok(ApiResponse<PagedResult<CourseResponse>>.Ok(await _service.GetAllAsync(query)));

    /// <summary>Get course by ID (includes subject, semester, enrollments)</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<CourseResponse>.Fail($"Course {id} not found"));
        return Ok(ApiResponse<CourseResponse>.Ok(result));
    }

    /// <summary>Create new course</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> Create([FromBody] CreateCourseRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<CourseResponse>.Ok(result, "Course created successfully"));
    }

    /// <summary>Update course</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> Update(int id, [FromBody] UpdateCourseRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<CourseResponse>.Fail($"Course {id} not found"));
        return Ok(ApiResponse<CourseResponse>.Ok(result, "Course updated successfully"));
    }

    /// <summary>Delete course</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail($"Course {id} not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Course deleted successfully"));
    }
}
