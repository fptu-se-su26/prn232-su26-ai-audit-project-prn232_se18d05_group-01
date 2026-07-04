namespace PlayCourt.Application.DTOs.Sports
{
    public class SportDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public short? PlayerCount { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
