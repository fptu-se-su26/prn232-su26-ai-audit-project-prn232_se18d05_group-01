using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;
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

    /// <summary>
    /// Lấy danh sách tất cả các venue. Hỗ trợ filter theo status.
    /// Ví dụ: GET /api/admin/venues?status=Pending
    /// Các giá trị hợp lệ: Pending, Approved, Rejected, Suspended
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status = null)
    {
        var response = await _adminVenueService.GetAllVenuesAsync(status);
        return Ok(response);
    }

    /// <summary>
    /// Lấy chi tiết một venue theo ID (không giới hạn trạng thái).
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _adminVenueService.GetVenueByIdAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    /// <summary>
    /// Duyệt venue. Chỉ hợp lệ khi venue đang ở trạng thái Pending.
    /// </summary>
    [HttpPatch("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var response = await _adminVenueService.ApproveVenueAsync(id);
        if (!response.Success) return HandleAdminVenueError(response);
        return Ok(response);
    }

    /// <summary>
    /// Từ chối venue. Chỉ hợp lệ khi venue đang ở trạng thái Pending.
    /// </summary>
    [HttpPatch("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var response = await _adminVenueService.RejectVenueAsync(id);
        if (!response.Success) return HandleAdminVenueError(response);
        return Ok(response);
    }

    /// <summary>
    /// Đình chỉ venue. Chỉ hợp lệ khi venue đang ở trạng thái Approved.
    /// </summary>
    [HttpPatch("{id:int}/suspend")]
    public async Task<IActionResult> Suspend(int id)
    {
        var response = await _adminVenueService.SuspendVenueAsync(id);
        if (!response.Success) return HandleAdminVenueError(response);
        return Ok(response);
    }

    /// <summary>
    /// Khôi phục venue từ Suspended về Approved.
    /// </summary>
    [HttpPatch("{id:int}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var response = await _adminVenueService.RestoreVenueAsync(id);
        if (!response.Success) return HandleAdminVenueError(response);
        return Ok(response);
    }

    /// <summary>
    /// Xử lý lỗi từ admin venue service:
    /// - "not found" → 404 NotFound
    /// - "Cannot transition" → 409 Conflict (invalid state machine transition)
    /// - Khác → 400 BadRequest
    /// </summary>
    private IActionResult HandleAdminVenueError(ApiResponse<VenueResponseDto> response)
    {
        if (response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(response);

        if (response.Message.Contains("Cannot transition", StringComparison.OrdinalIgnoreCase))
            return Conflict(response);

        return BadRequest(response);
    }
}
