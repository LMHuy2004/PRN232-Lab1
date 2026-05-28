using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Semesters;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _service;
    public SemestersController(ISemesterService service) => _service = service;

    /// <summary>Get all semesters with search, sort, paging</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SemesterResponse>>>> GetAll([FromQuery] SemesterQuery query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<SemesterResponse>>.Ok(result));
    }

    /// <summary>Get semester by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<SemesterResponse>.Fail($"Semester {id} not found"));
        return Ok(ApiResponse<SemesterResponse>.Ok(result));
    }

    /// <summary>Create new semester</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> Create([FromBody] CreateSemesterRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<SemesterResponse>.Ok(result, "Semester created successfully"));
    }

    /// <summary>Update semester</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> Update(int id, [FromBody] UpdateSemesterRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null)
            return NotFound(ApiResponse<SemesterResponse>.Fail($"Semester {id} not found"));
        return Ok(ApiResponse<SemesterResponse>.Ok(result, "Semester updated successfully"));
    }

    /// <summary>Delete semester</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Semester {id} not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Semester deleted successfully"));
    }
}
