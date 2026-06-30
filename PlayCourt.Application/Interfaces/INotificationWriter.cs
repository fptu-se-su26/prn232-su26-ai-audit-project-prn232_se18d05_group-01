using PlayCourt.Application.DTOs.Notifications;

namespace PlayCourt.Application.Interfaces
{
    /// <summary>
    /// Internal writer for creating notifications within business transactions.
    /// Only adds the entity to the context — does NOT call <c>SaveChangesAsync</c>.
    /// </summary>
    /// <remarks>
    /// <b>Caller is responsible for calling SaveChangesAsync.</b>
    /// Use this before the existing <c>SaveChangesAsync</c> call so the notification
    /// is persisted in the same unit of work as the business entity change.
    /// </remarks>
    public interface INotificationWriter
    {
        /// <summary>
        /// Validates and adds a notification entity to the DbContext.
        /// Does NOT call <c>SaveChangesAsync</c>.
        /// </summary>
        void Add(CreateNotificationRequest request);
    }
}
