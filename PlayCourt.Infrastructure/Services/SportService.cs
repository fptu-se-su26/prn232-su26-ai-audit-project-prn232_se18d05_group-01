using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Sports;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class SportService : ISportService
    {
        private readonly PlayCourtDbContext _dbContext;

        public SportService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<List<SportDto>>> GetAllAsync(bool? isActive = null)
        {
            var query = _dbContext.Sports.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(sport => sport.IsActive == isActive.Value);
            }

            var sports = await query
                .OrderBy(sport => sport.Name)
                .ToListAsync();

            return ApiResponse<List<SportDto>>.Ok(
                sports.Select(MapToDto).ToList(),
                "Sports retrieved successfully.");
        }

        public async Task<ApiResponse<SportDto>> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return ApiResponse<SportDto>.Fail("Không tìm thấy môn thể thao.");
            }

            var sport = await _dbContext.Sports.FirstOrDefaultAsync(sport => sport.Id == id);

            if (sport is null)
            {
                return ApiResponse<SportDto>.Fail("Không tìm thấy môn thể thao.");
            }

            return ApiResponse<SportDto>.Ok(
                MapToDto(sport),
                "Sport retrieved successfully.");
        }

        public async Task<ApiResponse<SportDto>> CreateAsync(CreateSportRequestDto request)
        {
            var errors = ValidateCreateRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<SportDto>.Fail("Tạo môn thể thao thất bại.", errors);
            }

            var normalizedCode = NormalizeCode(request.Code);
            var name = NormalizeRequiredText(request.Name);
            var description = NormalizeOptionalText(request.Description);

            errors.AddRange(await ValidateCreateBusinessRulesAsync(normalizedCode, name));
            if (errors.Count > 0)
            {
                return ApiResponse<SportDto>.Fail("Tạo môn thể thao thất bại.", errors);
            }

            var sport = new Sport
            {
                Code = normalizedCode,
                Name = name,
                Description = description,
                PlayerCount = request.PlayerCount,
                IsActive = true
            };

            _dbContext.Sports.Add(sport);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<SportDto>.Ok(
                MapToDto(sport),
                "Sport created successfully.");
        }

        public async Task<ApiResponse<SportDto>> UpdateAsync(int id, UpdateSportRequestDto request)
        {
            if (id <= 0)
            {
                return ApiResponse<SportDto>.Fail("Không tìm thấy môn thể thao.");
            }

            var sport = await _dbContext.Sports.FirstOrDefaultAsync(sport => sport.Id == id);
            if (sport is null)
            {
                return ApiResponse<SportDto>.Fail("Không tìm thấy môn thể thao.");
            }

            var errors = ValidateUpdateRequest(request);
            if (errors.Count > 0)
            {
                return ApiResponse<SportDto>.Fail("Cập nhật môn thể thao thất bại.", errors);
            }

            var normalizedCode = NormalizeCode(request.Code);
            var name = NormalizeRequiredText(request.Name);
            var description = NormalizeOptionalText(request.Description);

            errors.AddRange(await ValidateUpdateBusinessRulesAsync(id, normalizedCode, name));
            if (errors.Count > 0)
            {
                return ApiResponse<SportDto>.Fail("Cập nhật môn thể thao thất bại.", errors);
            }

            sport.Code = normalizedCode;
            sport.Name = name;
            sport.Description = description;
            sport.PlayerCount = request.PlayerCount;
            sport.IsActive = request.IsActive;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<SportDto>.Ok(
                MapToDto(sport),
                "Sport updated successfully.");
        }

        public async Task<ApiResponse<SportDto>> ToggleActiveAsync(int id)
        {
            if (id <= 0)
            {
                return ApiResponse<SportDto>.Fail("Không tìm thấy môn thể thao.");
            }

            var sport = await _dbContext.Sports.FirstOrDefaultAsync(sport => sport.Id == id);
            if (sport is null)
            {
                return ApiResponse<SportDto>.Fail("Không tìm thấy môn thể thao.");
            }

            sport.IsActive = !sport.IsActive;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<SportDto>.Ok(
                MapToDto(sport),
                "Sport active status updated successfully.");
        }

        private static SportDto MapToDto(Sport sport)
        {
            return new SportDto
            {
                Id = sport.Id,
                Code = sport.Code,
                Name = sport.Name,
                Description = sport.Description,
                PlayerCount = sport.PlayerCount,
                IsActive = sport.IsActive,
                CreatedAt = sport.CreatedAt
            };
        }

        private static string NormalizeCode(string code)
        {
            return code.Trim().ToUpperInvariant();
        }

        private static string NormalizeRequiredText(string value)
        {
            return value.Trim();
        }

        private static string? NormalizeOptionalText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static List<string> ValidateCreateRequest(CreateSportRequestDto request)
        {
            var errors = new List<string>();

            ValidateCode(request.Code, errors);
            ValidateName(request.Name, errors);
            ValidatePlayerCount(request.PlayerCount, errors);

            return errors;
        }

        private static List<string> ValidateUpdateRequest(UpdateSportRequestDto request)
        {
            var errors = new List<string>();

            ValidateCode(request.Code, errors);
            ValidateName(request.Name, errors);
            ValidatePlayerCount(request.PlayerCount, errors);

            return errors;
        }

        private async Task<List<string>> ValidateCreateBusinessRulesAsync(string normalizedCode, string name)
        {
            var errors = new List<string>();

            if (await _dbContext.Sports.AnyAsync(sport => sport.Code == normalizedCode))
            {
                errors.Add("Mã môn thể thao đã tồn tại.");
            }

            if (await _dbContext.Sports.AnyAsync(sport => sport.Name == name))
            {
                errors.Add("Tên môn thể thao đã tồn tại.");
            }

            return errors;
        }

        private async Task<List<string>> ValidateUpdateBusinessRulesAsync(int id, string normalizedCode, string name)
        {
            var errors = new List<string>();

            if (await _dbContext.Sports.AnyAsync(sport => sport.Id != id && sport.Code == normalizedCode))
            {
                errors.Add("Mã môn thể thao đã tồn tại.");
            }

            if (await _dbContext.Sports.AnyAsync(sport => sport.Id != id && sport.Name == name))
            {
                errors.Add("Tên môn thể thao đã tồn tại.");
            }

            return errors;
        }

        private static void ValidateCode(string code, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                errors.Add("Code is required.");
                return;
            }

            if (code.Trim().Length > 50)
            {
                errors.Add("Code must not exceed 50 characters.");
            }
        }

        private static void ValidateName(string name, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("Name is required.");
                return;
            }

            if (name.Trim().Length > 100)
            {
                errors.Add("Name must not exceed 100 characters.");
            }
        }

        private static void ValidatePlayerCount(short? playerCount, List<string> errors)
        {
            if (playerCount.HasValue && playerCount <= 0)
            {
                errors.Add("Số lượng người chơi phải lớn hơn 0.");
            }
        }
    }
}
