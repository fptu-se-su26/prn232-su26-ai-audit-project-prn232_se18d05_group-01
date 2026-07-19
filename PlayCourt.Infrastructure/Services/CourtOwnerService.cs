using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.CourtOwners;
using PlayCourt.Application.DTOs.Notifications;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class CourtOwnerService : ICourtOwnerService
    {
        private readonly PlayCourtDbContext _dbContext;
        private readonly INotificationWriter _notificationWriter;

        public CourtOwnerService(PlayCourtDbContext dbContext, INotificationWriter notificationWriter)
        {
            _dbContext = dbContext;
            _notificationWriter = notificationWriter;
        }

        // ──────────────────────────────────────────────
        // Admin endpoints
        // ──────────────────────────────────────────────

        public async Task<ApiResponse<List<CourtOwnerListItemDto>>> GetAllAsync(
            CourtOwnerVerificationStatus? verificationStatus = null)
        {
            if (verificationStatus.HasValue && !Enum.IsDefined(verificationStatus.Value))
            {
                return ApiResponse<List<CourtOwnerListItemDto>>.Fail("Verification status is invalid.");
            }

            var query = _dbContext.CourtOwnerProfiles.AsNoTracking().AsQueryable();

            if (verificationStatus.HasValue)
            {
                query = query.Where(profile => profile.VerificationStatus == verificationStatus.Value);
            }

            var profiles = await query
                .Include(profile => profile.UserProfile)
                .ThenInclude(profile => profile.User)
                .OrderByDescending(profile => profile.CreatedAt)
                .ToListAsync();

            return ApiResponse<List<CourtOwnerListItemDto>>.Ok(
                profiles.Select(MapToListItem).ToList(),
                "Court owner profiles retrieved successfully.");
        }

        public async Task<ApiResponse<CourtOwnerDetailDto>> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail("Court owner profile not found.");
            }

            var profile = await FindProfileByIdAsync(id, asNoTracking: true);
            if (profile is null)
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail("Court owner profile not found.");
            }

            return ApiResponse<CourtOwnerDetailDto>.Ok(
                MapToDetail(profile),
                "Court owner profile retrieved successfully.");
        }

        public async Task<ApiResponse<CourtOwnerDetailDto>> UpdateVerificationStatusAsync(
            int id,
            UpdateCourtOwnerVerificationStatusRequestDto request)
        {
            if (id <= 0)
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail("Court owner profile not found.");
            }

            var targetStatus = (CourtOwnerVerificationStatus)request.VerificationStatus;

            // Chỉ cho phép Approved hoặc Rejected
            if (!Enum.IsDefined(typeof(CourtOwnerVerificationStatus), request.VerificationStatus) ||
                (targetStatus != CourtOwnerVerificationStatus.Approved &&
                 targetStatus != CourtOwnerVerificationStatus.Rejected))
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail(
                    "Verification status must be Approved or Rejected.");
            }

            var profile = await FindProfileByIdAsync(id, asNoTracking: false);
            if (profile is null)
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail("Court owner profile not found.");
            }

            // Current status phải là Pending
            if (profile.VerificationStatus != CourtOwnerVerificationStatus.Pending)
            {
                return ApiResponse<CourtOwnerDetailDto>.Fail(
                    "Only profiles with Pending status can be approved or rejected.");
            }

            // Approve: phải validate hồ sơ đầy đủ
            if (targetStatus == CourtOwnerVerificationStatus.Approved)
            {
                var validationErrors = ValidateProfileComplete(profile);
                if (validationErrors.Count > 0)
                {
                    return ApiResponse<CourtOwnerDetailDto>.Fail(
                        "Profile is incomplete. Cannot approve.", validationErrors);
                }

                profile.RejectionReason = null;
            }

            // Reject: bắt buộc RejectionReason
            if (targetStatus == CourtOwnerVerificationStatus.Rejected)
            {
                var reason = NormalizeOptional(request.RejectionReason);
                if (string.IsNullOrEmpty(reason))
                {
                    return ApiResponse<CourtOwnerDetailDto>.Fail(
                        "RejectionReason is required when rejecting a profile.");
                }

                if (reason.Length > 500)
                {
                    return ApiResponse<CourtOwnerDetailDto>.Fail(
                        "RejectionReason must not exceed 500 characters.");
                }

                profile.RejectionReason = reason;
            }

            profile.VerificationStatus = targetStatus;
            profile.UpdatedAt = DateTimeOffset.Now;

            // Notify court owner about KYC verification result
            var recipientUserId = profile.UserProfile.UserId;
            if (recipientUserId > 0)
            {
                var content = targetStatus == CourtOwnerVerificationStatus.Approved
                    ? "Hồ sơ đăng ký chủ sân của bạn đã được quản trị viên phê duyệt."
                    : "Hồ sơ đăng ký chủ sân của bạn đã bị quản trị viên từ chối.";

                if (targetStatus == CourtOwnerVerificationStatus.Rejected
                    && !string.IsNullOrWhiteSpace(profile.RejectionReason))
                {
                    content += $" Lý do: {profile.RejectionReason}";
                }

                _notificationWriter.Add(new CreateNotificationRequest
                {
                    UserId = recipientUserId,
                    Title = targetStatus == CourtOwnerVerificationStatus.Approved
                        ? "Hồ sơ chủ sân đã được phê duyệt"
                        : "Hồ sơ chủ sân đã bị từ chối",
                    Content = content,
                    Type = NotificationType.System,
                    ReferenceType = null,
                    ReferenceId = null
                });
            }

            await _dbContext.SaveChangesAsync();

            return ApiResponse<CourtOwnerDetailDto>.Ok(
                MapToDetail(profile),
                "Court owner verification status updated successfully.");
        }

        // ──────────────────────────────────────────────
        // Owner endpoints
        // ──────────────────────────────────────────────

        public async Task<ApiResponse<CourtOwnerProfileResponseDto>> GetMyProfileAsync(int userId)
        {
            var profile = await FindProfileByUserIdAsync(userId, asNoTracking: true);
            if (profile is null)
            {
                return ApiResponse<CourtOwnerProfileResponseDto>.Fail("Court owner profile not found.");
            }

            return ApiResponse<CourtOwnerProfileResponseDto>.Ok(
                MapToOwnerResponse(profile),
                "Court owner profile retrieved successfully.");
        }

        public async Task<ApiResponse<CourtOwnerProfileResponseDto>> UpdateMyProfileAsync(
            int userId,
            UpdateCourtOwnerProfileRequestDto request)
        {
            var profile = await FindProfileByUserIdAsync(userId, asNoTracking: false);
            if (profile is null)
            {
                return ApiResponse<CourtOwnerProfileResponseDto>.Fail("Court owner profile not found.");
            }

            // Chỉ cho phép chỉnh sửa khi Draft hoặc Rejected
            if (profile.VerificationStatus != CourtOwnerVerificationStatus.Draft &&
                profile.VerificationStatus != CourtOwnerVerificationStatus.Rejected)
            {
                return ApiResponse<CourtOwnerProfileResponseDto>.Fail(
                    "Profile can only be edited when status is Draft or Rejected.");
            }

            // Validate max length và gán giá trị
            if (request.BusinessName is not null)
            {
                var trimmed = request.BusinessName.Trim();
                if (trimmed.Length > 255)
                {
                    return ApiResponse<CourtOwnerProfileResponseDto>.Fail("BusinessName must not exceed 255 characters.");
                }
                profile.BusinessName = trimmed;
            }

            if (request.BusinessLicenseNo is not null)
            {
                var trimmed = request.BusinessLicenseNo.Trim();
                if (trimmed.Length > 100)
                {
                    return ApiResponse<CourtOwnerProfileResponseDto>.Fail("BusinessLicenseNo must not exceed 100 characters.");
                }
                profile.BusinessLicenseNo = string.IsNullOrEmpty(trimmed) ? null : trimmed;
            }

            if (request.TaxCode is not null)
            {
                var trimmed = request.TaxCode.Trim();
                if (trimmed.Length > 50)
                {
                    return ApiResponse<CourtOwnerProfileResponseDto>.Fail("TaxCode must not exceed 50 characters.");
                }
                profile.TaxCode = string.IsNullOrEmpty(trimmed) ? null : trimmed;
            }

            if (request.BusinessAddress is not null)
            {
                var trimmed = request.BusinessAddress.Trim();
                if (trimmed.Length > 500)
                {
                    return ApiResponse<CourtOwnerProfileResponseDto>.Fail("BusinessAddress must not exceed 500 characters.");
                }
                profile.BusinessAddress = string.IsNullOrEmpty(trimmed) ? null : trimmed;
            }

            if (request.BusinessLicenseDocumentUrl is not null)
            {
                var trimmed = request.BusinessLicenseDocumentUrl.Trim();
                if (trimmed.Length > 1000)
                {
                    return ApiResponse<CourtOwnerProfileResponseDto>.Fail("BusinessLicenseDocumentUrl must not exceed 1000 characters.");
                }

                if (!string.IsNullOrEmpty(trimmed))
                {
                    if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) ||
                        (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                    {
                        return ApiResponse<CourtOwnerProfileResponseDto>.Fail(
                            "BusinessLicenseDocumentUrl must be an absolute URL using HTTP or HTTPS.");
                    }
                }

                profile.BusinessLicenseDocumentUrl = string.IsNullOrEmpty(trimmed) ? null : trimmed;
            }

            profile.UpdatedAt = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<CourtOwnerProfileResponseDto>.Ok(
                MapToOwnerResponse(profile),
                "Court owner profile updated successfully.");
        }

        public async Task<ApiResponse<CourtOwnerProfileResponseDto>> SubmitMyProfileAsync(int userId)
        {
            var profile = await FindProfileByUserIdAsync(userId, asNoTracking: false);
            if (profile is null)
            {
                return ApiResponse<CourtOwnerProfileResponseDto>.Fail("Court owner profile not found.");
            }

            // Chỉ cho phép submit khi Draft hoặc Rejected
            if (profile.VerificationStatus != CourtOwnerVerificationStatus.Draft &&
                profile.VerificationStatus != CourtOwnerVerificationStatus.Rejected)
            {
                return ApiResponse<CourtOwnerProfileResponseDto>.Fail(
                    "Profile can only be submitted when status is Draft or Rejected.");
            }

            // Validate hồ sơ đầy đủ
            var validationErrors = ValidateProfileComplete(profile);
            if (validationErrors.Count > 0)
            {
                return ApiResponse<CourtOwnerProfileResponseDto>.Fail(
                    "Profile is incomplete. Please fill in all required fields.", validationErrors);
            }

            profile.VerificationStatus = CourtOwnerVerificationStatus.Pending;
            profile.SubmittedAt = DateTimeOffset.Now;
            profile.RejectionReason = null;
            profile.UpdatedAt = DateTimeOffset.Now;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<CourtOwnerProfileResponseDto>.Ok(
                MapToOwnerResponse(profile),
                "Court owner profile submitted for review successfully.");
        }

        // ──────────────────────────────────────────────
        // Private helpers
        // ──────────────────────────────────────────────

        private async Task<CourtOwnerProfile?> FindProfileByIdAsync(int id, bool asNoTracking)
        {
            var query = _dbContext.CourtOwnerProfiles
                .Include(profile => profile.UserProfile)
                .ThenInclude(profile => profile.User)
                .Where(profile => profile.Id == id);

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        private async Task<CourtOwnerProfile?> FindProfileByUserIdAsync(int userId, bool asNoTracking)
        {
            var query = _dbContext.CourtOwnerProfiles
                .Include(profile => profile.UserProfile)
                .ThenInclude(profile => profile.User)
                .Where(profile => profile.UserProfile.UserId == userId);

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Validate hồ sơ đầy đủ trước khi submit hoặc admin approve.
        /// Dùng chung cho cả Submit và Admin Approve.
        /// </summary>
        private static List<string> ValidateProfileComplete(CourtOwnerProfile profile)
        {
            var errors = new List<string>();

            var businessName = NormalizeOptional(profile.BusinessName);
            if (string.IsNullOrEmpty(businessName))
            {
                errors.Add("BusinessName is required.");
            }
            else if (businessName.Length > 255)
            {
                errors.Add("BusinessName must not exceed 255 characters.");
            }

            var licenseNo = NormalizeOptional(profile.BusinessLicenseNo);
            if (string.IsNullOrEmpty(licenseNo))
            {
                errors.Add("BusinessLicenseNo is required.");
            }
            else if (licenseNo.Length > 100)
            {
                errors.Add("BusinessLicenseNo must not exceed 100 characters.");
            }

            var taxCode = NormalizeOptional(profile.TaxCode);
            if (string.IsNullOrEmpty(taxCode))
            {
                errors.Add("TaxCode is required.");
            }
            else if (taxCode.Length > 50)
            {
                errors.Add("TaxCode must not exceed 50 characters.");
            }

            var address = NormalizeOptional(profile.BusinessAddress);
            if (string.IsNullOrEmpty(address))
            {
                errors.Add("BusinessAddress is required.");
            }
            else if (address.Length > 500)
            {
                errors.Add("BusinessAddress must not exceed 500 characters.");
            }

            var docUrl = NormalizeOptional(profile.BusinessLicenseDocumentUrl);
            if (string.IsNullOrEmpty(docUrl))
            {
                errors.Add("BusinessLicenseDocumentUrl is required.");
            }
            else if (docUrl.Length > 1000)
            {
                errors.Add("BusinessLicenseDocumentUrl must not exceed 1000 characters.");
            }
            else if (!Uri.TryCreate(docUrl, UriKind.Absolute, out var uri) ||
                     (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                errors.Add("BusinessLicenseDocumentUrl must be an absolute URL using HTTP or HTTPS.");
            }

            return errors;
        }

        private static CourtOwnerListItemDto MapToListItem(CourtOwnerProfile profile)
        {
            return new CourtOwnerListItemDto
            {
                Id = profile.Id,
                UserId = profile.UserProfile.UserId,
                FullName = profile.UserProfile.FullName,
                Email = profile.UserProfile.User.Email,
                Phone = profile.UserProfile.User.Phone,
                BusinessName = profile.BusinessName,
                VerificationStatus = profile.VerificationStatus.ToString(),
                CreatedAt = profile.CreatedAt
            };
        }

        private static CourtOwnerDetailDto MapToDetail(CourtOwnerProfile profile)
        {
            return new CourtOwnerDetailDto
            {
                Id = profile.Id,
                UserProfileId = profile.UserProfileId,
                UserId = profile.UserProfile.UserId,
                FullName = profile.UserProfile.FullName,
                Email = profile.UserProfile.User.Email,
                Phone = profile.UserProfile.User.Phone,
                AvatarUrl = profile.UserProfile.AvatarUrl,
                DateOfBirth = profile.UserProfile.DateOfBirth,
                Gender = profile.UserProfile.Gender?.ToString(),
                Address = profile.UserProfile.Address,
                City = profile.UserProfile.City,
                Country = profile.UserProfile.Country,
                BusinessName = profile.BusinessName,
                BusinessLicenseNo = profile.BusinessLicenseNo,
                TaxCode = profile.TaxCode,
                BusinessAddress = profile.BusinessAddress,
                BusinessLicenseDocumentUrl = profile.BusinessLicenseDocumentUrl,
                VerificationStatus = profile.VerificationStatus.ToString(),
                RejectionReason = profile.RejectionReason,
                SubmittedAt = profile.SubmittedAt,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }

        private static CourtOwnerProfileResponseDto MapToOwnerResponse(CourtOwnerProfile profile)
        {
            return new CourtOwnerProfileResponseDto
            {
                Id = profile.Id,
                BusinessName = profile.BusinessName,
                BusinessLicenseNo = profile.BusinessLicenseNo,
                TaxCode = profile.TaxCode,
                BusinessAddress = profile.BusinessAddress,
                BusinessLicenseDocumentUrl = profile.BusinessLicenseDocumentUrl,
                VerificationStatus = profile.VerificationStatus.ToString(),
                RejectionReason = profile.RejectionReason,
                SubmittedAt = profile.SubmittedAt,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
