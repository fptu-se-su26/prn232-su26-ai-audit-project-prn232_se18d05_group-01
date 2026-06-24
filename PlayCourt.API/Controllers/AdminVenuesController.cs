using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.Application.Interfaces;
using PlayCourt.API.Authorization;

namespace PlayCourt.API.Controllers;

[ApiController]
[Route("api/admin/venues")]
[Authorize(Policy = ApiPolicies.Admin)]
public sealed class AdminVenuesController : ControllerBase
{
    private readonly IAdminVenueService _adminVenueService;

    public AdminVenuesController(IAdminVenueService adminVenueService)
    {
        _adminVenueService = adminVenueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPendingVenues()
    {
        var response = await _adminVenueService.GetPendingVenuesAsync();
        return Ok(response);
    }

    [HttpPatch("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var response = await _adminVenueService.ApproveVenueAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpPatch("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var response = await _adminVenueService.RejectVenueAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpPatch("{id:int}/suspend")]
    public async Task<IActionResult> Suspend(int id)
    {
        var response = await _adminVenueService.SuspendVenueAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }
}
