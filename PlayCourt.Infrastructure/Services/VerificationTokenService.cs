using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class VerificationTokenService : IVerificationTokenService
    {
        private const int OtpLength = 6;
        private const int MaxFailedAttempts = 5;
        private readonly PlayCourtDbContext _dbContext;

        public VerificationTokenService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreateOtpAsync(int userId, VerificationTokenPurpose purpose, int expiryMinutes = 10)
        {
            var otp = GenerateOtp();
            var now = DateTimeOffset.Now;

            _dbContext.VerificationTokens.Add(new VerificationToken
            {
                UserId = userId,
                Purpose = purpose,
                TokenHash = BCrypt.Net.BCrypt.HashPassword(otp),
                ExpiresAt = now.AddMinutes(expiryMinutes),
                CreatedAt = now,
                FailedAttempts = 0,
                IsDeleted = false
            });

            await _dbContext.SaveChangesAsync();

            return otp;
        }

        public async Task<bool> VerifyOtpAsync(int userId, VerificationTokenPurpose purpose, string otp)
        {
            if (string.IsNullOrWhiteSpace(otp))
            {
                return false;
            }

            var now = DateTimeOffset.Now;
            var token = await _dbContext.VerificationTokens
                .Where(t => t.UserId == userId
                    && t.Purpose == purpose
                    && t.UsedAt == null)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (token is null
                || token.ExpiresAt <= now
                || token.FailedAttempts >= MaxFailedAttempts)
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(otp.Trim(), token.TokenHash))
            {
                token.FailedAttempts++;
                token.UpdatedAt = now;
                await _dbContext.SaveChangesAsync();

                return false;
            }

            token.UsedAt = now;
            token.UpdatedAt = now;
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private static string GenerateOtp()
        {
            var value = RandomNumberGenerator.GetInt32(0, (int)Math.Pow(10, OtpLength));
            return value.ToString($"D{OtpLength}");
        }
    }
}
