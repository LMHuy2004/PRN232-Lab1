using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Students;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    public StudentsController(IStudentService service) => _service = service;

    /// <summary>Get all students with search, sort, paging</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<StudentResponse>>>> GetAll([FromQuery] StudentQuery query)
        => Ok(ApiResponse<PagedResult<StudentResponse>>.Ok(await _service.GetAllAsync(query)));

    /// <summary>Get student by ID (includes enrollment history)</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<StudentResponse>.Fail($"Student {id} not found"));
        return Ok(ApiResponse<StudentResponse>.Ok(result));
    }

    /// <summary>Create new student</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> Create([FromBody] CreateStudentRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<StudentResponse>.Ok(result, "Student created successfully"));
    }

    /// <summary>Update student</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<StudentResponse>.Fail($"Student {id} not found"));
        return Ok(ApiResponse<StudentResponse>.Ok(result, "Student updated successfully"));
    }

    /// <summary>Delete student</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail($"Student {id} not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Student deleted successfully"));
    }
}
