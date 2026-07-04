using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
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
        private const int OtpSaltBytes = 16;
        private const int HmacSha256Bytes = 32;
        private const string TokenHashPrefix = "hmac-sha256";
        private readonly PlayCourtDbContext _dbContext;
        private readonly byte[] _hashKey;

        public VerificationTokenService(PlayCourtDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            var hashKey = configuration["Otp:HashKey"];
            if (string.IsNullOrWhiteSpace(hashKey))
            {
                hashKey = configuration["Jwt:Key"];
            }

            if (string.IsNullOrWhiteSpace(hashKey))
            {
                throw new InvalidOperationException("Otp:HashKey or Jwt:Key is not configured.");
            }

            _hashKey = Encoding.UTF8.GetBytes(hashKey);
        }

        public async Task<string> CreateOtpAsync(int userId, VerificationTokenPurpose purpose, int expiryMinutes = 10)
        {
            var otp = GenerateOtp();
            var now = DateTimeOffset.Now;

            _dbContext.VerificationTokens.Add(new VerificationToken
            {
                UserId = userId,
                Purpose = purpose,
                TokenHash = HashOtp(userId, purpose, otp),
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

            if (!VerifyOtpHash(userId, purpose, otp.Trim(), token.TokenHash))
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

        private string HashOtp(int userId, VerificationTokenPurpose purpose, string otp)
        {
            var salt = RandomNumberGenerator.GetBytes(OtpSaltBytes);
            var saltText = Convert.ToBase64String(salt);
            var hash = ComputeOtpHash(userId, purpose, otp, saltText);

            return $"{TokenHashPrefix}:{saltText}:{Convert.ToBase64String(hash)}";
        }

        private bool VerifyOtpHash(
            int userId,
            VerificationTokenPurpose purpose,
            string otp,
            string tokenHash)
        {
            var parts = tokenHash.Split(':');
            if (parts.Length != 3 || parts[0] != TokenHashPrefix)
            {
                return false;
            }

            try
            {
                var storedHash = Convert.FromBase64String(parts[2]);
                if (storedHash.Length != HmacSha256Bytes)
                {
                    return false;
                }

                var computedHash = ComputeOtpHash(userId, purpose, otp, parts[1]);
                return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private byte[] ComputeOtpHash(int userId, VerificationTokenPurpose purpose, string otp, string salt)
        {
            var payload = $"{salt}:{userId}:{(short)purpose}:{otp}";
            return HMACSHA256.HashData(_hashKey, Encoding.UTF8.GetBytes(payload));
        }
    }
}
