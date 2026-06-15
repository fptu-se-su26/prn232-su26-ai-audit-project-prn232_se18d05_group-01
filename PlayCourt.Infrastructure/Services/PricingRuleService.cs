using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.PricingRules;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class PricingRuleService : IPricingRuleService
    {
        private readonly PlayCourtDbContext _dbContext;

        public PricingRuleService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ─── GET BY COURT ────────────────────────────────────────────────────────

        public async Task<ApiResponse<List<PricingRuleDto>>> GetByCourtAsync(int courtId)
        {
            if (courtId <= 0)
            {
                return ApiResponse<List<PricingRuleDto>>.Fail("Không tìm thấy sân.");
            }

            var courtExists = await _dbContext.Courts
                .AnyAsync(c => c.Id == courtId && !c.IsDeleted);

            if (!courtExists)
            {
                return ApiResponse<List<PricingRuleDto>>.Fail("Không tìm thấy sân.");
            }

            var rules = await _dbContext.PricingRules
                .Where(r => r.CourtId == courtId)
                .OrderBy(r => r.DayOfWeek)
                .ThenBy(r => r.StartTime)
                .ToListAsync();

            return ApiResponse<List<PricingRuleDto>>.Ok(
                rules.Select(MapToDto).ToList(),
                "Pricing rules retrieved successfully.");
        }

        // ─── CREATE ──────────────────────────────────────────────────────────────

        public async Task<ApiResponse<PricingRuleDto>> CreateAsync(
            int courtId,
            int currentUserId,
            CreatePricingRuleRequestDto request)
        {
            // 1. Parse time fields.
            var parseResult = ParseTimes(request.StartTime, request.EndTime);
            if (!parseResult.IsValid)
            {
                return ApiResponse<PricingRuleDto>.Fail("Tạo pricing rule thất bại.", parseResult.Errors);
            }

            // 2. Validate các field.
            var errors = ValidateFields(request.DayOfWeek, parseResult.StartTime, parseResult.EndTime,
                request.PricePerHour, request.EffectiveFrom, request.EffectiveTo);

            if (errors.Count > 0)
            {
                return ApiResponse<PricingRuleDto>.Fail("Tạo pricing rule thất bại.", errors);
            }

            // 3. Kiểm tra court tồn tại + verify ownership.
            var court = await _dbContext.Courts
                .Include(c => c.Venue)
                    .ThenInclude(v => v.CourtOwnerProfile)
                        .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(c => c.Id == courtId && !c.IsDeleted);

            if (court is null)
            {
                return ApiResponse<PricingRuleDto>.Fail("Không tìm thấy sân.");
            }

            if (court.Venue.CourtOwnerProfile.UserProfile.UserId != currentUserId)
            {
                return ApiResponse<PricingRuleDto>.Fail("Bạn không có quyền thêm pricing rule cho sân này.");
            }

            // 4. Check overlap trong cùng court, cùng ngày, giao time & date range.
            var overlapError = await CheckOverlapAsync(courtId, request.DayOfWeek,
                parseResult.StartTime, parseResult.EndTime,
                request.EffectiveFrom, request.EffectiveTo,
                excludeId: null);

            if (overlapError is not null)
            {
                return ApiResponse<PricingRuleDto>.Fail("Tạo pricing rule thất bại.", [overlapError]);
            }

            // 5. Tạo mới.
            var rule = new PricingRule
            {
                CourtId = courtId,
                DayOfWeek = request.DayOfWeek,
                StartTime = parseResult.StartTime,
                EndTime = parseResult.EndTime,
                PricePerHour = request.PricePerHour,
                EffectiveFrom = request.EffectiveFrom.Date,
                EffectiveTo = request.EffectiveTo?.Date,
                CreatedAt = DateTimeOffset.Now
            };

            _dbContext.PricingRules.Add(rule);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<PricingRuleDto>.Ok(
                MapToDto(rule),
                "Pricing rule created successfully.");
        }

        // ─── UPDATE ──────────────────────────────────────────────────────────────

        public async Task<ApiResponse<PricingRuleDto>> UpdateAsync(
            int id,
            int currentUserId,
            UpdatePricingRuleRequestDto request)
        {
            if (id <= 0)
            {
                return ApiResponse<PricingRuleDto>.Fail("Không tìm thấy pricing rule.");
            }

            // 1. Parse time fields.
            var parseResult = ParseTimes(request.StartTime, request.EndTime);
            if (!parseResult.IsValid)
            {
                return ApiResponse<PricingRuleDto>.Fail("Cập nhật pricing rule thất bại.", parseResult.Errors);
            }

            // 2. Validate các field.
            var errors = ValidateFields(request.DayOfWeek, parseResult.StartTime, parseResult.EndTime,
                request.PricePerHour, request.EffectiveFrom, request.EffectiveTo);

            if (errors.Count > 0)
            {
                return ApiResponse<PricingRuleDto>.Fail("Cập nhật pricing rule thất bại.", errors);
            }

            // 3. Lấy rule kèm ownership chain.
            var rule = await _dbContext.PricingRules
                .Include(r => r.Court)
                    .ThenInclude(c => c.Venue)
                        .ThenInclude(v => v.CourtOwnerProfile)
                            .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rule is null)
            {
                return ApiResponse<PricingRuleDto>.Fail("Không tìm thấy pricing rule.");
            }

            // 4. Verify ownership.
            if (rule.Court.Venue.CourtOwnerProfile.UserProfile.UserId != currentUserId)
            {
                return ApiResponse<PricingRuleDto>.Fail("Bạn không có quyền cập nhật pricing rule này.");
            }

            // 5. Check overlap, exclude chính rule đang update.
            var overlapError = await CheckOverlapAsync(rule.CourtId, request.DayOfWeek,
                parseResult.StartTime, parseResult.EndTime,
                request.EffectiveFrom, request.EffectiveTo,
                excludeId: id);

            if (overlapError is not null)
            {
                return ApiResponse<PricingRuleDto>.Fail("Cập nhật pricing rule thất bại.", [overlapError]);
            }

            // 6. Cập nhật.
            rule.DayOfWeek = request.DayOfWeek;
            rule.StartTime = parseResult.StartTime;
            rule.EndTime = parseResult.EndTime;
            rule.PricePerHour = request.PricePerHour;
            rule.EffectiveFrom = request.EffectiveFrom.Date;
            rule.EffectiveTo = request.EffectiveTo?.Date;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<PricingRuleDto>.Ok(
                MapToDto(rule),
                "Pricing rule updated successfully.");
        }

        // ─── DELETE ──────────────────────────────────────────────────────────────

        public async Task<ApiResponse<object>> DeleteAsync(int id, int currentUserId)
        {
            if (id <= 0)
            {
                return ApiResponse<object>.Fail("Không tìm thấy pricing rule.");
            }

            var rule = await _dbContext.PricingRules
                .Include(r => r.Court)
                    .ThenInclude(c => c.Venue)
                        .ThenInclude(v => v.CourtOwnerProfile)
                            .ThenInclude(cop => cop.UserProfile)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rule is null)
            {
                return ApiResponse<object>.Fail("Không tìm thấy pricing rule.");
            }

            if (rule.Court.Venue.CourtOwnerProfile.UserProfile.UserId != currentUserId)
            {
                return ApiResponse<object>.Fail("Bạn không có quyền xóa pricing rule này.");
            }

            _dbContext.PricingRules.Remove(rule);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Pricing rule deleted successfully.");
        }

        // ─── PRIVATE HELPERS ─────────────────────────────────────────────────────

        private static PricingRuleDto MapToDto(PricingRule rule)
        {
            return new PricingRuleDto
            {
                Id = rule.Id,
                CourtId = rule.CourtId,
                DayOfWeek = rule.DayOfWeek,
                StartTime = rule.StartTime.ToString(@"hh\:mm"),
                EndTime = rule.EndTime.ToString(@"hh\:mm"),
                PricePerHour = rule.PricePerHour,
                EffectiveFrom = rule.EffectiveFrom.ToString("yyyy-MM-dd"),
                EffectiveTo = rule.EffectiveTo?.ToString("yyyy-MM-dd"),
                CreatedAt = rule.CreatedAt
            };
        }

        // Parse "HH:mm" → TimeSpan.
        private static (bool IsValid, TimeSpan StartTime, TimeSpan EndTime, List<string> Errors) ParseTimes(
            string? startTimeStr, string? endTimeStr)
        {
            var errors = new List<string>();

            if (!TimeSpan.TryParseExact(startTimeStr?.Trim(), @"hh\:mm", null, out var startTime))
            {
                errors.Add("StartTime không hợp lệ. Định dạng yêu cầu: HH:mm (ví dụ: 08:00).");
            }

            if (!TimeSpan.TryParseExact(endTimeStr?.Trim(), @"hh\:mm", null, out var endTime))
            {
                errors.Add("EndTime không hợp lệ. Định dạng yêu cầu: HH:mm (ví dụ: 22:00).");
            }

            return (errors.Count == 0, startTime, endTime, errors);
        }

        private static List<string> ValidateFields(
            int dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            decimal pricePerHour,
            DateTime effectiveFrom,
            DateTime? effectiveTo)
        {
            var errors = new List<string>();

            if (dayOfWeek is < 1 or > 7)
            {
                errors.Add("DayOfWeek phải từ 1 (Monday) đến 7 (Sunday).");
            }

            if (startTime >= endTime)
            {
                errors.Add("StartTime phải nhỏ hơn EndTime.");
            }

            if (pricePerHour <= 0)
            {
                errors.Add("PricePerHour phải lớn hơn 0.");
            }

            if (effectiveTo.HasValue && effectiveTo.Value.Date < effectiveFrom.Date)
            {
                errors.Add("EffectiveTo phải lớn hơn hoặc bằng EffectiveFrom.");
            }

            return errors;
        }

        // Kiểm tra overlap: cùng court, cùng DayOfWeek, time range giao nhau, date range giao nhau.
        private async Task<string?> CheckOverlapAsync(
            int courtId,
            int dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            DateTime effectiveFrom,
            DateTime? effectiveTo,
            int? excludeId)
        {
            var query = _dbContext.PricingRules
                .Where(r => r.CourtId == courtId
                    && r.DayOfWeek == dayOfWeek
                    && r.StartTime < endTime
                    && r.EndTime > startTime
                    && r.EffectiveFrom <= (effectiveTo ?? DateTime.MaxValue)
                    && (r.EffectiveTo == null || r.EffectiveTo >= effectiveFrom));

            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }

            var hasOverlap = await query.AnyAsync();

            return hasOverlap
                ? "Khung giờ này bị trùng với pricing rule đã tồn tại trong cùng ngày và khoảng thời gian hiệu lực."
                : null;
        }
    }
}
