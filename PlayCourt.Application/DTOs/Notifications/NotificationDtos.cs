using System.ComponentModel.DataAnnotations;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Application.DTOs.Notifications
{
    public sealed class NotificationDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }

        /// <summary>Returns enum name via <c>ToString()</c>.</summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>Returns enum name or <c>null</c>.</summary>
        public string? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public bool IsRead { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }

    public sealed class NotificationQueryDto
    {
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;

        public bool? IsRead { get; set; }

        public NotificationType? Type { get; set; }
    }

    public sealed class UnreadNotificationCountDto
    {
        public int Count { get; set; }
    }

    public sealed class MarkAllNotificationsReadResponseDto
    {
        public int UpdatedCount { get; set; }
    }

    /// <summary>
    /// Internal request for creating a notification. Not exposed via public API.
    /// Caller must ensure <see cref="NotificationWriter.Add"/> is called before
    /// <c>SaveChangesAsync</c> so the notification is part of the same unit of work.
    /// </summary>
    public sealed class CreateNotificationRequest
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        public NotificationReferenceType? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }
    }
}
