using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.DTOs.Notifications;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class NotificationWriter : INotificationWriter
    {
        private readonly PlayCourtDbContext _dbContext;

        public NotificationWriter(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <b>Caller is responsible for calling SaveChangesAsync.</b>
        /// This method only validates the request, creates the entity,
        /// and adds it to the change tracker.
        /// </remarks>
        public void Add(CreateNotificationRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                ReferenceType = request.ReferenceType,
                ReferenceId = request.ReferenceId,
                IsRead = false,
                CreatedAt = DateTimeOffset.Now
            };

            _dbContext.Notifications.Add(notification);
        }
    }
}
