using PlayCourt.Application.DTOs;
using PlayCourt.Domain.Entities;

namespace PlayCourt.Infrastructure.Helpers
{
    internal static class VenueStaffMapper
    {
        internal static VenueStaffResponseDto MapToResponse(VenueStaff staff)
        {
            return new VenueStaffResponseDto
            {
                Id = staff.Id,
                VenueId = staff.VenueId,
                VenueName = staff.Venue?.Name ?? string.Empty,
                UserId = staff.UserId,
                FullName = staff.User?.UserProfile?.FullName ?? string.Empty,
                Email = staff.User?.Email ?? string.Empty,
                Role = staff.Role.ToString(),
                IsActive = staff.IsActive,
                CreatedAt = staff.CreatedAt
            };
        }
    }
}
