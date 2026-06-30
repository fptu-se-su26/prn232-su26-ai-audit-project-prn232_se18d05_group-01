using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.CourtSchedules;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class CourtScheduleService : ICourtScheduleService
    {
        private readonly PlayCourtDbContext _dbContext;

        public CourtScheduleService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<List<CourtScheduleDto>>> GetByCourtAsync(int courtId)
        {
            if (courtId <= 0)
            {
                return ApiResponse<List<CourtScheduleDto>>.Fail("Không tìm thấy sân.");
            }

            var courtExists = await _dbContext.Courts
                .AnyAsync(c => c.Id == courtId && !c.IsDeleted);

            if (!courtExists)
            {
                return ApiResponse<List<CourtScheduleDto>>.Fail("Không tìm thấy sân.");
            }

            var schedules = await _dbContext.CourtSchedules
                .Where(s => s.CourtId == courtId)
                .OrderBy(s => s.StartAt)
                .ToListAsync();

            var dtos = schedules.Select(s => new CourtScheduleDto
            {
                Id = s.Id,
                CourtId = s.CourtId,
                StartAt = s.StartAt,
                EndAt = s.EndAt,
                Reason = s.Reason,
                CreatedAt = s.CreatedAt
            }).ToList();

            return ApiResponse<List<CourtScheduleDto>>.Ok(dtos, "Court schedules retrieved successfully.");
        }

        public async Task<ApiResponse<CourtScheduleDto>> CreateAsync(int courtId, int currentUserId, CreateCourtScheduleRequestDto request)
        {
            if (request.StartAt >= request.EndAt)
            {
                return ApiResponse<CourtScheduleDto>.Fail("Tạo lịch khóa sân thất bại.", ["Thời gian bắt đầu phải trước thời gian kết thúc."]);
            }

            var court = await _dbContext.Courts
                .Include(c => c.Venue)
                    .ThenInclude(v => v.CourtOwnerProfile)
                        .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(c => c.Id == courtId && !c.IsDeleted);

            if (court is null)
            {
                return ApiResponse<CourtScheduleDto>.Fail("Không tìm thấy sân.");
            }

            if (court.Venue.CourtOwnerProfile.UserProfile.UserId != currentUserId)
            {
                return ApiResponse<CourtScheduleDto>.Fail("Bạn không có quyền khóa sân này.");
            }

            // Kiểm tra trùng lịch khóa sân (overlap).
            var hasOverlap = await _dbContext.CourtSchedules
                .AnyAsync(s => s.CourtId == courtId
                            && s.StartAt < request.EndAt
                            && s.EndAt > request.StartAt);

            if (hasOverlap)
            {
                return ApiResponse<CourtScheduleDto>.Fail("Tạo lịch khóa sân thất bại.", ["Khoảng thời gian này bị trùng với một lịch khóa sân khác."]);
            }

            var hasActiveBookingOverlap = await _dbContext.Bookings
                .AnyAsync(b => b.CourtId == courtId
                            && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
                            && b.StartAt < request.EndAt
                            && b.EndAt > request.StartAt);

            if (hasActiveBookingOverlap)
            {
                return ApiResponse<CourtScheduleDto>.Fail("Tạo lịch khóa sân thất bại.", ["Khoảng thời gian này bị trùng với một đơn đặt sân đang hoạt động. Vui lòng xử lý đơn đặt sân trước khi khóa sân."]);
            }

            var schedule = new CourtSchedule
            {
                CourtId = courtId,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                Reason = request.Reason,
                CreatedAt = DateTimeOffset.Now
            };

            _dbContext.CourtSchedules.Add(schedule);
            await _dbContext.SaveChangesAsync();

            var dto = new CourtScheduleDto
            {
                Id = schedule.Id,
                CourtId = schedule.CourtId,
                StartAt = schedule.StartAt,
                EndAt = schedule.EndAt,
                Reason = schedule.Reason,
                CreatedAt = schedule.CreatedAt
            };

            return ApiResponse<CourtScheduleDto>.Ok(dto, "Court schedule created successfully.");
        }

        public async Task<ApiResponse<object>> DeleteAsync(int id, int currentUserId)
        {
            if (id <= 0)
            {
                return ApiResponse<object>.Fail("Không tìm thấy lịch khóa sân.");
            }

            var schedule = await _dbContext.CourtSchedules
                .Include(s => s.Court)
                    .ThenInclude(c => c.Venue)
                        .ThenInclude(v => v.CourtOwnerProfile)
                            .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule is null)
            {
                return ApiResponse<object>.Fail("Không tìm thấy lịch khóa sân.");
            }

            if (schedule.Court.Venue.CourtOwnerProfile.UserProfile.UserId != currentUserId)
            {
                return ApiResponse<object>.Fail("Bạn không có quyền xóa lịch khóa sân này.");
            }

            _dbContext.CourtSchedules.Remove(schedule);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Court schedule deleted successfully.");
        }
    }
}
