using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Enrollments;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;
    public EnrollmentsController(IEnrollmentService service) => _service = service;

    /// <summary>Get all enrollments with filter, sort, paging</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EnrollmentResponse>>>> GetAll([FromQuery] EnrollmentQuery query)
        => Ok(ApiResponse<PagedResult<EnrollmentResponse>>.Ok(await _service.GetAllAsync(query)));

    /// <summary>Get enrollment by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<EnrollmentResponse>.Fail($"Enrollment {id} not found"));
        return Ok(ApiResponse<EnrollmentResponse>.Ok(result));
    }

    /// <summary>Enroll student to course</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> Create([FromBody] CreateEnrollmentRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<EnrollmentResponse>.Ok(result, "Student enrolled successfully"));
    }

    /// <summary>Update enrollment status and grade</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> Update(int id, [FromBody] UpdateEnrollmentRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<EnrollmentResponse>.Fail($"Enrollment {id} not found"));
        return Ok(ApiResponse<EnrollmentResponse>.Ok(result, "Enrollment updated successfully"));
    }

    /// <summary>Delete enrollment</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail($"Enrollment {id} not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Enrollment deleted successfully"));
    }
}
