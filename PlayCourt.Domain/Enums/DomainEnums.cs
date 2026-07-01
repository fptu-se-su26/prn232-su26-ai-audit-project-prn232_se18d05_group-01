namespace PlayCourt.Domain.Enums
{
    public enum UserRole : short
    {
        Admin = 0,
        Player = 1,
        CourtOwner = 2
    }

    public enum UserStatus : short
    {
        Active = 0,
        Locked = 1,
        Inactive = 2
    }

    public enum Gender : short
    {
        Male = 0,
        Female = 1,
        Other = 2
    }

    public enum CourtOwnerVerificationStatus : short
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum SkillLevel : short
    {
        Beginner = 0,
        Intermediate = 1,
        Advanced = 2
    }

    public enum VenueStatus : short
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Suspended = 3
    }

    public enum CourtStatus : short
    {
        Available = 0,
        Maintenance = 1,
        Inactive = 2
    }

    public enum VenueStaffRole : short
    {
        Manager = 0,
        Receptionist = 1,
        Accountant = 2
    }

    public enum BookingStatus : short
    {
        Pending = 0,
        Confirmed = 1,
        CancelledByUser = 2,
        CancelledByOwner = 3,
        Completed = 4,
        Expired = 5
    }

    public enum PaymentType : short
    {
        BookingPayment = 0,
        Refund = 1,
        Payout = 2
    }

    public enum PaymentStatus : short
    {
        Pending = 0,
        Success = 1,
        Failed = 2
    }

    public enum MatchStatus : short
    {
        Open = 0,
        Full = 1,
        Cancelled = 2,
        Completed = 3
    }

    public enum MatchJoinRequestStatus : short
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum MatchInvitationStatus : short
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2,
        Cancelled = 3
    }

    public enum ReviewStatus : short
    {
        Visible = 0,
        Hidden = 1,
        Reported = 2
    }

    public enum NotificationType : short
    {
        Booking = 0,
        Match = 1,
        Payment = 2,
        Review = 3,
        System = 4
    }

    public enum NotificationReferenceType : short
    {
        Booking = 0,
        Match = 1,
        Payment = 2,
        Review = 3,
        Venue = 4
    }

    public enum VerificationTokenPurpose : short
    {
        EmailVerification = 0,
        PasswordReset = 1
    }
}
