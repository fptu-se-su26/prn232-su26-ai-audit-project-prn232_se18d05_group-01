using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Helpers;

namespace PlayCourt.Infrastructure.Services;

public sealed class AdminVenueService : IAdminVenueService
{
    private readonly PlayCourtDbContext _dbContext;

    // State machine: key = trạng thái hiện tại, value = tập trạng thái được phép chuyển sang
    private static readonly Dictionary<VenueStatus, HashSet<VenueStatus>> _allowedTransitions = new()
    {
        [VenueStatus.Pending]   = [VenueStatus.Approved, VenueStatus.Rejected],
        [VenueStatus.Approved]  = [VenueStatus.Suspended],
        [VenueStatus.Rejected]  = [],                         // không thể chuyển từ Rejected
        [VenueStatus.Suspended] = [VenueStatus.Approved],     // restore
    };

    public AdminVenueService(PlayCourtDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<IReadOnlyCollection<VenueResponseDto>>> GetAllVenuesAsync(string? status = null)
    {
        var query = _dbContext.Venues
            .AsNoTracking()
            .Include(v => v.Images)
            .Include(v => v.VenueAmenities)
                .ThenInclude(va => va.Amenity)
            .Include(v => v.OpeningHours)
            .AsQueryable();

        // Lọc theo trạng thái nếu có truyền vào
        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<VenueStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            query = query.Where(v => v.Status == parsedStatus);
        }

        var venues = await query
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        var result = venues.Select(VenueMapper.MapToResponse).ToList();
        return ApiResponse<IReadOnlyCollection<VenueResponseDto>>.Ok(result, "Venues retrieved successfully.");
    }

    public async Task<ApiResponse<VenueResponseDto>> GetVenueByIdAsync(int venueId)
    {
        var venue = await _dbContext.Venues
            .AsNoTracking()
            .Include(v => v.Images)
            .Include(v => v.VenueAmenities)
                .ThenInclude(va => va.Amenity)
            .Include(v => v.OpeningHours)
            .FirstOrDefaultAsync(v => v.Id == venueId);

        if (venue is null)
            return ApiResponse<VenueResponseDto>.Fail("Venue not found.");

        return ApiResponse<VenueResponseDto>.Ok(VenueMapper.MapToResponse(venue), "Venue retrieved successfully.");
    }

    public async Task<ApiResponse<VenueResponseDto>> ApproveVenueAsync(int venueId)
    {
        return await ChangeVenueStatusAsync(venueId, VenueStatus.Approved);
    }

    public async Task<ApiResponse<VenueResponseDto>> RejectVenueAsync(int venueId)
    {
        return await ChangeVenueStatusAsync(venueId, VenueStatus.Rejected);
    }

    public async Task<ApiResponse<VenueResponseDto>> SuspendVenueAsync(int venueId)
    {
        return await ChangeVenueStatusAsync(venueId, VenueStatus.Suspended);
    }

    public async Task<ApiResponse<VenueResponseDto>> RestoreVenueAsync(int venueId)
    {
        return await ChangeVenueStatusAsync(venueId, VenueStatus.Approved);
    }

    /// <summary>
    /// Thực hiện chuyển đổi trạng thái venue theo state machine đã định nghĩa.
    /// Trả về lỗi 409-style message nếu transition không hợp lệ.
    /// </summary>
    private async Task<ApiResponse<VenueResponseDto>> ChangeVenueStatusAsync(int venueId, VenueStatus newStatus)
    {
        var venue = await _dbContext.Venues
            .Include(v => v.Images)
            .Include(v => v.VenueAmenities)
                .ThenInclude(va => va.Amenity)
            .Include(v => v.OpeningHours)
            .FirstOrDefaultAsync(v => v.Id == venueId);

        if (venue is null)
            return ApiResponse<VenueResponseDto>.Fail("Venue not found.");

        // Kiểm tra state machine transition
        if (!_allowedTransitions.TryGetValue(venue.Status, out var allowed) || !allowed.Contains(newStatus))
        {
            return ApiResponse<VenueResponseDto>.Fail(
                $"Cannot transition venue from '{venue.Status}' to '{newStatus}'. " +
                $"Allowed transitions from '{venue.Status}': [{string.Join(", ", _allowedTransitions[venue.Status])}].");
        }

        venue.Status = newStatus;
        venue.UpdatedAt = DateTimeOffset.Now;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<VenueResponseDto>.Ok(VenueMapper.MapToResponse(venue), $"Venue status changed to '{newStatus}' successfully.");
    }
}
