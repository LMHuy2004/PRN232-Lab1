using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Subjects;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _service;
    public SubjectsController(ISubjectService service) => _service = service;

    /// <summary>Get all subjects with search, sort, paging</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SubjectResponse>>>> GetAll([FromQuery] SubjectQuery query)
        => Ok(ApiResponse<PagedResult<SubjectResponse>>.Ok(await _service.GetAllAsync(query)));

    /// <summary>Get subject by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<SubjectResponse>.Fail($"Subject {id} not found"));
        return Ok(ApiResponse<SubjectResponse>.Ok(result));
    }

    /// <summary>Create new subject</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> Create([FromBody] CreateSubjectRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<SubjectResponse>.Ok(result, "Subject created successfully"));
    }

    /// <summary>Update subject</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> Update(int id, [FromBody] UpdateSubjectRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<SubjectResponse>.Fail($"Subject {id} not found"));
        return Ok(ApiResponse<SubjectResponse>.Ok(result, "Subject updated successfully"));
    }

    /// <summary>Delete subject</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail($"Subject {id} not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Subject deleted successfully"));
    }
}
