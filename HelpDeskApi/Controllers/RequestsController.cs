// Controllers/RequestsController.cs
using Microsoft.AspNetCore.Mvc;
using HelpDeskApi.Models;
using HelpDeskApi.Services;

namespace HelpDeskApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IRequestService _service;

    public RequestsController(IRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<SupportRequest>>> GetAll()
    {
        var requests = await _service.GetAllAsync();
        return Ok(requests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupportRequest>> GetById(Guid id)
    {
        var request = await _service.GetByIdAsync(id);
        if (request == null)
            return NotFound();
        return Ok(request);
    }

    [HttpPost]
    public async Task<ActionResult<SupportRequest>> Create([FromBody] SupportRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SupportRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        if (!updated)
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] RequestStatus status)
    {
        var changed = await _service.ChangeStatusAsync(id, status);
        if (!changed)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}