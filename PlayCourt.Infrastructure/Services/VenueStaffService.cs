using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class VenueStaffService : IVenueStaffService
    {
        private readonly PlayCourtDbContext _dbContext;

        public VenueStaffService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<VenueStaffResponseDto>> AddStaffAsync(int ownerUserId, int venueId, AddVenueStaffRequestDto request)
        {
            // Find venue and check owner
            var venue = await _dbContext.Venues
                .Include(v => v.CourtOwnerProfile)
                    .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(v => v.Id == venueId && !v.IsDeleted);

            if (venue == null)
            {
                return ApiResponse<VenueStaffResponseDto>.Fail("Venue not found.");
            }

            if (venue.CourtOwnerProfile.UserProfile.UserId != ownerUserId)
            {
                return ApiResponse<VenueStaffResponseDto>.Fail("You are not authorized to manage staff for this venue.");
            }

            // Find user to add as staff
            var staffUser = await _dbContext.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);

            if (staffUser == null)
            {
                return ApiResponse<VenueStaffResponseDto>.Fail($"User with email '{request.Email}' does not exist.");
            }

            // Staff shouldn't be Admin.
            if (staffUser.Role == UserRole.Admin)
            {
                return ApiResponse<VenueStaffResponseDto>.Fail("Cannot add an Admin as staff.");
            }

            // Check existing staff association
            var existingStaff = await _dbContext.VenueStaffs
                .IgnoreQueryFilters() // check both active/inactive
                .FirstOrDefaultAsync(vs => vs.VenueId == venueId && vs.UserId == staffUser.Id);

            if (existingStaff != null)
            {
                if (existingStaff.IsActive)
                {
                    return ApiResponse<VenueStaffResponseDto>.Fail("User is already a staff member at this venue.");
                }
                else
                {
                    // Reactivate inactive staff
                    existingStaff.IsActive = true;
                    existingStaff.Role = request.Role;
                    existingStaff.CreatedAt = DateTimeOffset.Now;
                    await _dbContext.SaveChangesAsync();

                    // Load relations for mapper
                    var reactivatedStaff = await _dbContext.VenueStaffs
                        .Include(vs => vs.Venue)
                        .Include(vs => vs.User)
                            .ThenInclude(u => u.UserProfile)
                        .FirstOrDefaultAsync(vs => vs.Id == existingStaff.Id);

                    return ApiResponse<VenueStaffResponseDto>.Ok(MapToResponse(reactivatedStaff!), "Venue staff reactivated successfully.");
                }
            }

            // Create new staff mapping
            var newStaff = new VenueStaff
            {
                VenueId = venueId,
                UserId = staffUser.Id,
                Role = request.Role,
                IsActive = true,
                CreatedAt = DateTimeOffset.Now
            };

            _dbContext.VenueStaffs.Add(newStaff);
            await _dbContext.SaveChangesAsync();

            // Load relations for mapper
            var savedStaff = await _dbContext.VenueStaffs
                .Include(vs => vs.Venue)
                .Include(vs => vs.User)
                    .ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync(vs => vs.Id == newStaff.Id);

            return ApiResponse<VenueStaffResponseDto>.Ok(MapToResponse(savedStaff!), "Venue staff added successfully.");
        }

        public async Task<ApiResponse<IReadOnlyCollection<VenueStaffResponseDto>>> GetVenueStaffAsync(int userId, int venueId)
        {
            var venue = await _dbContext.Venues
                .Include(v => v.CourtOwnerProfile)
                    .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(v => v.Id == venueId && !v.IsDeleted);

            if (venue == null)
            {
                return ApiResponse<IReadOnlyCollection<VenueStaffResponseDto>>.Fail("Venue not found.");
            }

            // Check if owner
            bool isOwner = venue.CourtOwnerProfile.UserProfile.UserId == userId;

            // Check if active staff
            bool isStaff = await _dbContext.VenueStaffs
                .AnyAsync(vs => vs.VenueId == venueId && vs.UserId == userId && vs.IsActive);

            if (!isOwner && !isStaff)
            {
                return ApiResponse<IReadOnlyCollection<VenueStaffResponseDto>>.Fail("You are not authorized to view the staff list for this venue.");
            }

            var staffs = await _dbContext.VenueStaffs
                .Include(vs => vs.Venue)
                .Include(vs => vs.User)
                    .ThenInclude(u => u.UserProfile)
                .Where(vs => vs.VenueId == venueId && vs.IsActive)
                .ToListAsync();

            var dtos = staffs.Select(MapToResponse).ToList();
            return ApiResponse<IReadOnlyCollection<VenueStaffResponseDto>>.Ok(dtos, "Venue staff retrieved successfully.");
        }

        public async Task<ApiResponse<object>> RemoveStaffAsync(int ownerUserId, int venueId, int staffId)
        {
            var venue = await _dbContext.Venues
                .Include(v => v.CourtOwnerProfile)
                    .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(v => v.Id == venueId && !v.IsDeleted);

            if (venue == null)
            {
                return ApiResponse<object>.Fail("Venue not found.");
            }

            if (venue.CourtOwnerProfile.UserProfile.UserId != ownerUserId)
            {
                return ApiResponse<object>.Fail("You are not authorized to manage staff for this venue.");
            }

            var staff = await _dbContext.VenueStaffs
                .FirstOrDefaultAsync(vs => vs.Id == staffId && vs.VenueId == venueId && vs.IsActive);

            if (staff == null)
            {
                return ApiResponse<object>.Fail("Staff member not found or is already inactive.");
            }

            // Soft delete staff by setting IsActive = false
            staff.IsActive = false;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Staff member removed successfully.");
        }

        private static VenueStaffResponseDto MapToResponse(VenueStaff staff)
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
