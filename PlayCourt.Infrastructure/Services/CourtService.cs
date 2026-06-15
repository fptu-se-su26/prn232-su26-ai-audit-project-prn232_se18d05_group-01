using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Courts;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class CourtService : ICourtService
    {
        private readonly PlayCourtDbContext _dbContext;

        public CourtService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ─── GET BY VENUE ────────────────────────────────────────────────────────

        public async Task<ApiResponse<List<CourtDto>>> GetByVenueAsync(int venueId)
        {
            if (venueId <= 0)
            {
                return ApiResponse<List<CourtDto>>.Fail("Không tìm thấy venue.");
            }

            var venueExists = await _dbContext.Venues
                .AnyAsync(v => v.Id == venueId && !v.IsDeleted);

            if (!venueExists)
            {
                return ApiResponse<List<CourtDto>>.Fail("Không tìm thấy venue.");
            }

            var courts = await _dbContext.Courts
                .Include(c => c.Sport)
                .Where(c => c.VenueId == venueId && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return ApiResponse<List<CourtDto>>.Ok(
                courts.Select(MapToDto).ToList(),
                "Courts retrieved successfully.");
        }

        // ─── GET BY ID ───────────────────────────────────────────────────────────

        public async Task<ApiResponse<CourtDto>> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return ApiResponse<CourtDto>.Fail("Không tìm thấy sân.");
            }

            var court = await _dbContext.Courts
                .Include(c => c.Sport)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (court is null)
            {
                return ApiResponse<CourtDto>.Fail("Không tìm thấy sân.");
            }

            return ApiResponse<CourtDto>.Ok(
                MapToDto(court),
                "Court retrieved successfully.");
        }

        // ─── CREATE ──────────────────────────────────────────────────────────────

        public async Task<ApiResponse<CourtDto>> CreateAsync(
            int venueId,
            int currentUserId,
            CreateCourtRequestDto request)
        {
            // 1. Validate input fields.
            var errors = ValidateCreateRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<CourtDto>.Fail("Tạo sân thất bại.", errors);
            }

            // 2. Kiểm tra venue tồn tại.
            var venue = await _dbContext.Venues
                .Include(v => v.CourtOwnerProfile)
                    .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(v => v.Id == venueId && !v.IsDeleted);

            if (venue is null)
            {
                return ApiResponse<CourtDto>.Fail("Không tìm thấy venue.");
            }

            // 3. Verify ownership: chỉ CourtOwner sở hữu venue mới được tạo court.
            if (venue.CourtOwnerProfile.UserProfile.UserId != currentUserId)
            {
                return ApiResponse<CourtDto>.Fail("Bạn không có quyền thêm sân vào venue này.");
            }

            // 4. Validate SportId tồn tại.
            var sportExists = await _dbContext.Sports
                .AnyAsync(s => s.Id == request.SportId && s.IsActive);

            if (!sportExists)
            {
                return ApiResponse<CourtDto>.Fail("Tạo sân thất bại.", ["SportId không hợp lệ hoặc môn thể thao không còn hoạt động."]);
            }

            // 5. Tạo court mới.
            var now = DateTimeOffset.Now;
            var court = new Court
            {
                VenueId = venueId,
                SportId = request.SportId,
                Name = request.Name.Trim(),
                Indoor = request.Indoor,
                Status = CourtStatus.Available,
                CreatedAt = now,
                IsDeleted = false
            };

            _dbContext.Courts.Add(court);
            await _dbContext.SaveChangesAsync();

            // Reload để có navigation property Sport.
            await _dbContext.Entry(court).Reference(c => c.Sport).LoadAsync();

            return ApiResponse<CourtDto>.Ok(
                MapToDto(court),
                "Court created successfully.");
        }

        // ─── UPDATE ──────────────────────────────────────────────────────────────

        public async Task<ApiResponse<CourtDto>> UpdateAsync(
            int id,
            int currentUserId,
            UpdateCourtRequestDto request)
        {
            if (id <= 0)
            {
                return ApiResponse<CourtDto>.Fail("Không tìm thấy sân.");
            }

            // 1. Validate input fields.
            var errors = ValidateUpdateRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<CourtDto>.Fail("Cập nhật sân thất bại.", errors);
            }

            // 2. Lấy court kèm ownership chain.
            var court = await _dbContext.Courts
                .Include(c => c.Sport)
                .Include(c => c.Venue)
                    .ThenInclude(v => v.CourtOwnerProfile)
                        .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (court is null)
            {
                return ApiResponse<CourtDto>.Fail("Không tìm thấy sân.");
            }

            // 3. Verify ownership.
            if (court.Venue.CourtOwnerProfile.UserProfile.UserId != currentUserId)
            {
                return ApiResponse<CourtDto>.Fail("Bạn không có quyền cập nhật sân này.");
            }

            // 4. Validate SportId tồn tại (chỉ khi thay đổi).
            if (request.SportId != court.SportId)
            {
                var sportExists = await _dbContext.Sports
                    .AnyAsync(s => s.Id == request.SportId && s.IsActive);

                if (!sportExists)
                {
                    return ApiResponse<CourtDto>.Fail("Cập nhật sân thất bại.", ["SportId không hợp lệ hoặc môn thể thao không còn hoạt động."]);
                }
            }

            // 5. Cập nhật fields.
            court.SportId = request.SportId;
            court.Name = request.Name.Trim();
            court.Indoor = request.Indoor;
            court.Status = request.Status;
            court.UpdatedAt = DateTimeOffset.Now;

            await _dbContext.SaveChangesAsync();

            // Reload Sport nếu SportId thay đổi.
            await _dbContext.Entry(court).Reference(c => c.Sport).LoadAsync();

            return ApiResponse<CourtDto>.Ok(
                MapToDto(court),
                "Court updated successfully.");
        }

        // ─── DELETE ──────────────────────────────────────────────────────────────

        public async Task<ApiResponse<bool>> DeleteAsync(int id, int currentUserId)
        {
            if (id <= 0)
            {
                return ApiResponse<bool>.Fail("Không tìm thấy sân.");
            }

            // Lấy court kèm ownership chain.
            var court = await _dbContext.Courts
                .Include(c => c.Venue)
                    .ThenInclude(v => v.CourtOwnerProfile)
                        .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (court is null)
            {
                return ApiResponse<bool>.Fail("Không tìm thấy sân.");
            }

            // Verify ownership.
            if (court.Venue.CourtOwnerProfile.UserProfile.UserId != currentUserId)
            {
                return ApiResponse<bool>.Fail("Bạn không có quyền xóa sân này.");
            }

            // Thực hiện xóa mềm.
            court.IsDeleted = true;
            court.UpdatedAt = DateTimeOffset.Now;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Court deleted successfully.");
        }

        // ─── PRIVATE HELPERS ─────────────────────────────────────────────────────

        private static CourtDto MapToDto(Court court)
        {
            return new CourtDto
            {
                Id = court.Id,
                VenueId = court.VenueId,
                SportId = court.SportId,
                SportName = court.Sport?.Name ?? string.Empty,
                Name = court.Name,
                Indoor = court.Indoor,
                Status = court.Status.ToString(),
                CreatedAt = court.CreatedAt,
                UpdatedAt = court.UpdatedAt
            };
        }

        private static List<string> ValidateCreateRequest(CreateCourtRequestDto request)
        {
            var errors = new List<string>();
            ValidateSportId(request.SportId, errors);
            ValidateName(request.Name, errors);
            return errors;
        }

        private static List<string> ValidateUpdateRequest(UpdateCourtRequestDto request)
        {
            var errors = new List<string>();
            ValidateSportId(request.SportId, errors);
            ValidateName(request.Name, errors);
            ValidateStatus(request.Status, errors);
            return errors;
        }

        private static void ValidateSportId(int sportId, List<string> errors)
        {
            if (sportId <= 0)
            {
                errors.Add("SportId phải lớn hơn 0.");
            }
        }

        private static void ValidateName(string name, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("Tên sân không được để trống.");
                return;
            }

            if (name.Trim().Length > 100)
            {
                errors.Add("Tên sân không được vượt quá 100 ký tự.");
            }
        }

        private static void ValidateStatus(CourtStatus status, List<string> errors)
        {
            if (!Enum.IsDefined(typeof(CourtStatus), status))
            {
                errors.Add("Trạng thái sân không hợp lệ.");
            }
        }
    }
}
