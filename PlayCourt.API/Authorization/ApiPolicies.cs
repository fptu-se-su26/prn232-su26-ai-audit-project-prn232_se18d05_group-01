using PlayCourt.Domain.Enums;

namespace PlayCourt.API.Authorization
{
    // Dung cac constant nay trong controller de tranh go sai ten policy.
    // Vi du: [Authorize(Policy = ApiPolicies.Admin)]
    public static class ApiPolicies
    {
        // Admin duoc phep truy cap cac API quan tri he thong.
        public const string Admin = nameof(UserRole.Admin);

        // Player duoc phep truy cap cac API danh cho nguoi choi.
        public const string Player = nameof(UserRole.Player);

        // CourtOwner duoc phep truy cap cac API danh cho chu san.
        public const string CourtOwner = nameof(UserRole.CourtOwner);
    }
}
