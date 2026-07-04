namespace PlayCourt.Application.DTOs.Courts
{
    public class CourtDto
    {
        public int Id { get; set; }

        public int VenueId { get; set; }

        public int SportId { get; set; }

        // Tên môn thể thao (join từ Sports table).
        public string SportName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public bool Indoor { get; set; }

        // "Available" | "Maintenance" | "Inactive"
        public string Status { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
