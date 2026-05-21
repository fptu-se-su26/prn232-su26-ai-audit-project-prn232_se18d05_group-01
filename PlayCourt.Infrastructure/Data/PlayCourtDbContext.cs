using Microsoft.EntityFrameworkCore;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Infrastructure.Data
{
    // DbContext chính của hệ thống PlayCourt.
    // File này cấu hình:
    // - DbSet cho toàn bộ bảng
    // - Primary key / composite key
    // - Unique index / filtered index
    // - Relationship FK
    // - Delete behavior
    // - Check constraint
    // - Default value SQL
    public class PlayCourtDbContext : DbContext
    {
        public PlayCourtDbContext(DbContextOptions<PlayCourtDbContext> options)
            : base(options)
        {
        }

        // Users & Profiles
        public DbSet<User> Users => Set<User>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<CourtOwnerProfile> CourtOwnerProfiles => Set<CourtOwnerProfile>();
        public DbSet<UserFavoriteVenue> UserFavoriteVenues => Set<UserFavoriteVenue>();

        // Sports
        public DbSet<Sport> Sports => Set<Sport>();
        public DbSet<PlayerSport> PlayerSports => Set<PlayerSport>();

        // Venues & Courts
        public DbSet<Venue> Venues => Set<Venue>();
        public DbSet<Court> Courts => Set<Court>();
        public DbSet<VenueImage> VenueImages => Set<VenueImage>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<VenueAmenity> VenueAmenities => Set<VenueAmenity>();
        public DbSet<CourtSchedule> CourtSchedules => Set<CourtSchedule>();
        public DbSet<PricingRule> PricingRules => Set<PricingRule>();
        public DbSet<VenueStaff> VenueStaffs => Set<VenueStaff>();
        public DbSet<VenueOpeningHour> VenueOpeningHours => Set<VenueOpeningHour>();

        // Bookings & Payments
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<BookingStatusHistory> BookingStatusHistories => Set<BookingStatusHistory>();

        // Matches
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<MatchParticipant> MatchParticipants => Set<MatchParticipant>();
        public DbSet<MatchJoinRequest> MatchJoinRequests => Set<MatchJoinRequest>();
        public DbSet<MatchInvitation> MatchInvitations => Set<MatchInvitation>();

        // Reviews
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<ReviewImage> ReviewImages => Set<ReviewImage>();

        // System
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUsers(modelBuilder);
            ConfigureUserProfiles(modelBuilder);
            ConfigureCourtOwnerProfiles(modelBuilder);
            ConfigureSports(modelBuilder);
            ConfigurePlayerSports(modelBuilder);
            ConfigureVenues(modelBuilder);
            ConfigureCourts(modelBuilder);
            ConfigureVenueImages(modelBuilder);
            ConfigureAmenities(modelBuilder);
            ConfigureVenueAmenities(modelBuilder);
            ConfigureUserFavoriteVenues(modelBuilder);
            ConfigureCourtSchedules(modelBuilder);
            ConfigurePricingRules(modelBuilder);
            ConfigureVenueStaffs(modelBuilder);
            ConfigureVenueOpeningHours(modelBuilder);
            ConfigureBookings(modelBuilder);
            ConfigurePayments(modelBuilder);
            ConfigureBookingStatusHistories(modelBuilder);
            ConfigureMatches(modelBuilder);
            ConfigureMatchParticipants(modelBuilder);
            ConfigureMatchJoinRequests(modelBuilder);
            ConfigureMatchInvitations(modelBuilder);
            ConfigureReviews(modelBuilder);
            ConfigureReviewImages(modelBuilder);
            ConfigureNotifications(modelBuilder);
        }

        private static void ConfigureUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();

                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.Status).HasDefaultValue(UserStatus.Active).IsRequired();
                entity.Property(e => e.IsEmailVerified).HasDefaultValue(false).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false).IsRequired();

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("UX_Users_Email_Active")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasIndex(e => e.Phone)
                    .IsUnique()
                    .HasDatabaseName("UX_Users_Phone_Active")
                    .HasFilter("[Phone] IS NOT NULL AND [IsDeleted] = 0");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Users_Role", "[Role] IN (0,1,2)");
                    t.HasCheckConstraint("CHK_Users_Status", "[Status] IN (0,1,2)");
                });
            });
        }

        private static void ConfigureUserProfiles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FullName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.City).HasMaxLength(255);
                entity.Property(e => e.Country).HasMaxLength(255);
                entity.Property(e => e.DateOfBirth).HasColumnType("date");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.User.IsDeleted);

                entity.HasIndex(e => e.UserId)
                    .IsUnique()
                    .HasDatabaseName("UQ_UserProfiles_UserId");

                entity.HasOne(e => e.User)
                    .WithOne(e => e.UserProfile)
                    .HasForeignKey<UserProfile>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_UserProfiles_Gender", "[Gender] IS NULL OR [Gender] IN (0,1,2)");
                });
            });
        }

        private static void ConfigureCourtOwnerProfiles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourtOwnerProfile>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.BusinessName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.BusinessLicenseNo).HasMaxLength(100);
                entity.Property(e => e.TaxCode).HasMaxLength(50);
                entity.Property(e => e.VerificationStatus).HasDefaultValue(CourtOwnerVerificationStatus.Pending).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.UserProfile.User.IsDeleted);

                entity.HasIndex(e => e.UserProfileId)
                    .IsUnique()
                    .HasDatabaseName("UQ_CourtOwnerProfiles_UserProfileId");

                entity.HasOne(e => e.UserProfile)
                    .WithOne(e => e.CourtOwnerProfile)
                    .HasForeignKey<CourtOwnerProfile>(e => e.UserProfileId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_CourtOwnerProfiles_VerificationStatus", "[VerificationStatus] IN (0,1,2)");
                });
            });
        }

        private static void ConfigureSports(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sport>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasIndex(e => e.Code)
                    .IsUnique()
                    .HasDatabaseName("UQ_Sports_Code");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Sports_PlayerCount", "[PlayerCount] IS NULL OR [PlayerCount] > 0");
                });
            });
        }

        private static void ConfigurePlayerSports(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerSport>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.SkillLevel).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.UserProfile.User.IsDeleted);

                entity.HasIndex(e => new { e.UserProfileId, e.SportId })
                    .IsUnique()
                    .HasDatabaseName("UQ_PlayerSports_UserProfileId_SportId");

                entity.HasIndex(e => e.SportId)
                    .HasDatabaseName("IX_PlayerSports_SportId");

                entity.HasOne(e => e.UserProfile)
                    .WithMany(e => e.PlayerSports)
                    .HasForeignKey(e => e.UserProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Sport)
                    .WithMany(e => e.PlayerSports)
                    .HasForeignKey(e => e.SportId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_PlayerSports_SkillLevel", "[SkillLevel] IN (0,1,2)");
                });
            });
        }

        private static void ConfigureVenues(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Latitude).HasColumnType("decimal(9,6)");
                entity.Property(e => e.Longitude).HasColumnType("decimal(9,6)");
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.OpenTime).HasColumnType("time(0)");
                entity.Property(e => e.CloseTime).HasColumnType("time(0)");
                entity.Property(e => e.Status).HasDefaultValue(VenueStatus.Pending).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false).IsRequired();

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => e.CourtOwnerProfileId)
                    .HasDatabaseName("IX_Venues_CourtOwnerProfileId")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasOne(e => e.CourtOwnerProfile)
                    .WithMany(e => e.Venues)
                    .HasForeignKey(e => e.CourtOwnerProfileId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Venues_Status", "[Status] IN (0,1,2,3)");
                    t.HasCheckConstraint("CHK_Venues_Latitude", "[Latitude] IS NULL OR ([Latitude] >= -90 AND [Latitude] <= 90)");
                    t.HasCheckConstraint("CHK_Venues_Longitude", "[Longitude] IS NULL OR ([Longitude] >= -180 AND [Longitude] <= 180)");
                });
            });
        }

        private static void ConfigureCourts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Court>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Indoor).HasDefaultValue(false).IsRequired();
                entity.Property(e => e.Status).HasDefaultValue(CourtStatus.Available).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false).IsRequired();

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => new { e.VenueId, e.Name })
                    .IsUnique()
                    .HasDatabaseName("UX_Courts_VenueId_Name_Active")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasIndex(e => e.VenueId)
                    .HasDatabaseName("IX_Courts_VenueId")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasIndex(e => e.SportId)
                    .HasDatabaseName("IX_Courts_SportId")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasOne(e => e.Venue)
                    .WithMany(e => e.Courts)
                    .HasForeignKey(e => e.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Sport)
                    .WithMany(e => e.Courts)
                    .HasForeignKey(e => e.SportId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Courts_Status", "[Status] IN (0,1,2)");
                });
            });
        }

        private static void ConfigureVenueImages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VenueImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ImageUrl).IsRequired();
                entity.Property(e => e.IsCover).HasDefaultValue(false).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.Venue.IsDeleted);

                entity.HasIndex(e => e.VenueId)
                    .IsUnique()
                    .HasDatabaseName("UX_VenueImages_OneCoverPerVenue")
                    .HasFilter("[IsCover] = 1");

                entity.HasOne(e => e.Venue)
                    .WithMany(e => e.Images)
                    .HasForeignKey(e => e.VenueId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureAmenities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Amenity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("UQ_Amenities_Name");
            });
        }

        private static void ConfigureVenueAmenities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VenueAmenity>(entity =>
            {
                entity.HasKey(e => new { e.VenueId, e.AmenityId })
                    .HasName("PK_VenueAmenities");

                entity.HasQueryFilter(e => !e.Venue.IsDeleted);

                entity.HasOne(e => e.Venue)
                    .WithMany(e => e.VenueAmenities)
                    .HasForeignKey(e => e.VenueId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Amenity)
                    .WithMany(e => e.VenueAmenities)
                    .HasForeignKey(e => e.AmenityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureUserFavoriteVenues(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFavoriteVenue>(entity =>
            {
                entity.HasKey(e => new { e.UserProfileId, e.VenueId });

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.Venue.IsDeleted);

                entity.HasOne(e => e.UserProfile)
                    .WithMany(e => e.FavoriteVenues)
                    .HasForeignKey(e => e.UserProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Venue)
                    .WithMany(e => e.FavoritedByUsers)
                    .HasForeignKey(e => e.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureCourtSchedules(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourtSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.StartAt).IsRequired();
                entity.Property(e => e.EndAt).IsRequired();
                entity.Property(e => e.Reason).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.Court.IsDeleted);

                entity.HasIndex(e => new { e.CourtId, e.StartAt, e.EndAt })
                    .HasDatabaseName("IX_CourtSchedules_CourtId_Time");

                entity.HasOne(e => e.Court)
                    .WithMany(e => e.CourtSchedules)
                    .HasForeignKey(e => e.CourtId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_CourtSchedules_Time", "[StartAt] < [EndAt]");
                });
            });
        }

        private static void ConfigurePricingRules(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PricingRule>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DayOfWeek).IsRequired();
                entity.Property(e => e.StartTime).HasColumnType("time(0)").IsRequired();
                entity.Property(e => e.EndTime).HasColumnType("time(0)").IsRequired();
                entity.Property(e => e.PricePerHour).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.EffectiveFrom).HasColumnType("date").IsRequired();
                entity.Property(e => e.EffectiveTo).HasColumnType("date");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.Court.IsDeleted);

                entity.HasIndex(e => new { e.CourtId, e.DayOfWeek })
                    .HasDatabaseName("IX_PricingRules_CourtId_DayOfWeek");

                entity.HasOne(e => e.Court)
                    .WithMany(e => e.PricingRules)
                    .HasForeignKey(e => e.CourtId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_PricingRules_DayOfWeek", "[DayOfWeek] BETWEEN 1 AND 7");
                    t.HasCheckConstraint("CHK_PricingRules_Time", "[StartTime] < [EndTime]");
                    t.HasCheckConstraint("CHK_PricingRules_Price", "[PricePerHour] >= 0");
                    t.HasCheckConstraint("CHK_PricingRules_EffectiveRange", "[EffectiveTo] IS NULL OR [EffectiveFrom] <= [EffectiveTo]");
                });
            });
        }

        private static void ConfigureVenueStaffs(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VenueStaff>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.Venue.IsDeleted && !e.User.IsDeleted);

                entity.HasIndex(e => new { e.VenueId, e.UserId })
                    .IsUnique()
                    .HasDatabaseName("UQ_VenueStaffs_VenueId_UserId");

                entity.HasOne(e => e.Venue)
                    .WithMany(e => e.Staffs)
                    .HasForeignKey(e => e.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.VenueStaffs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_VenueStaffs_Role", "[Role] IN (0,1,2)");
                });
            });
        }

        private static void ConfigureVenueOpeningHours(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VenueOpeningHour>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DayOfWeek).IsRequired();
                entity.Property(e => e.OpenTime).HasColumnType("time(0)");
                entity.Property(e => e.CloseTime).HasColumnType("time(0)");
                entity.Property(e => e.IsClosed).HasDefaultValue(false).IsRequired();

                entity.HasQueryFilter(e => !e.Venue.IsDeleted);

                entity.HasIndex(e => new { e.VenueId, e.DayOfWeek })
                    .IsUnique()
                    .HasDatabaseName("UQ_VenueOpeningHours_Venue_Day");

                entity.HasOne(e => e.Venue)
                    .WithMany(e => e.OpeningHours)
                    .HasForeignKey(e => e.VenueId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_VenueOpeningHours_Day", "[DayOfWeek] BETWEEN 1 AND 7");
                    t.HasCheckConstraint("CHK_VenueOpeningHours_Time", "[IsClosed] = 1 OR ([OpenTime] IS NOT NULL AND [CloseTime] IS NOT NULL AND [OpenTime] < [CloseTime])");
                });
            });
        }

        private static void ConfigureBookings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.StartAt).IsRequired();
                entity.Property(e => e.EndAt).IsRequired();
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(12,2)").IsRequired();
                entity.Property(e => e.PlatformFee).HasColumnType("decimal(12,2)").HasDefaultValue(0m).IsRequired();
                entity.Property(e => e.OwnerEarnings).HasColumnType("decimal(12,2)").IsRequired();
                entity.Property(e => e.Status).HasDefaultValue(BookingStatus.Pending).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false).IsRequired();

                entity.HasQueryFilter(e => !e.IsDeleted);

                // Alternate key để Reviews có thể tạo composite FK: (BookingId, PlayerId) -> (Id, UserProfileId)
                entity.HasAlternateKey(e => new { e.Id, e.UserProfileId })
                    .HasName("UQ_Bookings_Id_UserProfileId");

                entity.HasIndex(e => e.UserProfileId)
                    .HasDatabaseName("IX_Bookings_UserProfileId")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasIndex(e => new { e.CourtId, e.StartAt, e.Status })
                    .HasDatabaseName("IX_Bookings_CourtId_StartAt_Status")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasIndex(e => new { e.CourtId, e.EndAt, e.Status })
                    .HasDatabaseName("IX_Bookings_CourtId_EndAt_Status")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasIndex(e => new { e.CourtId, e.StartAt, e.EndAt })
                    .HasDatabaseName("IX_Bookings_Active_Court_Time")
                    .HasFilter("[IsDeleted] = 0 AND [Status] IN (0,1)");

                entity.HasOne(e => e.UserProfile)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(e => e.UserProfileId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Court)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(e => e.CourtId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Bookings_Time", "[StartAt] < [EndAt]");
                    t.HasCheckConstraint("CHK_Bookings_Status", "[Status] IN (0,1,2,3,4)");
                    t.HasCheckConstraint("CHK_Bookings_Amounts", "[TotalPrice] >= 0 AND [PlatformFee] >= 0 AND [OwnerEarnings] >= 0");
                    t.HasCheckConstraint("CHK_Bookings_FeeMath", "[TotalPrice] = [PlatformFee] + [OwnerEarnings]");
                });
            });
        }

        private static void ConfigurePayments(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Amount).HasColumnType("decimal(12,2)").IsRequired();
                entity.Property(e => e.Provider).HasMaxLength(50).IsRequired();
                entity.Property(e => e.TransactionCode).HasMaxLength(100);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Status).HasDefaultValue(PaymentStatus.Pending).IsRequired();
                entity.Property(e => e.Currency).HasColumnType("char(3)").HasMaxLength(3).HasDefaultValue("VND").IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.User.IsDeleted);

                entity.HasIndex(e => new { e.Provider, e.TransactionCode })
                    .IsUnique()
                    .HasDatabaseName("UX_Payments_Provider_TransactionCode")
                    .HasFilter("[TransactionCode] IS NOT NULL");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_Payments_UserId");

                entity.HasIndex(e => e.BookingId)
                    .HasDatabaseName("IX_Payments_BookingId");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Booking)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Payments_Amount", "[Amount] >= 0");
                    t.HasCheckConstraint("CHK_Payments_Type", "[Type] IN (0,1,2)");
                    t.HasCheckConstraint("CHK_Payments_Status", "[Status] IN (0,1,2)");
                    t.HasCheckConstraint("CHK_Payments_TypeBookingId", "([Type] = 0 AND [BookingId] IS NOT NULL) OR ([Type] IN (1,2))");
                });
            });
        }

        private static void ConfigureBookingStatusHistories(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingStatusHistory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.NewStatus).IsRequired();
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.Booking.IsDeleted);

                entity.HasOne(e => e.Booking)
                    .WithMany(e => e.StatusHistories)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ChangedByUser)
                    .WithMany(e => e.BookingStatusHistories)
                    .HasForeignKey(e => e.ChangedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_BookingStatusHistories_OldStatus", "[OldStatus] IS NULL OR [OldStatus] IN (0,1,2,3,4)");
                    t.HasCheckConstraint("CHK_BookingStatusHistories_NewStatus", "[NewStatus] IN (0,1,2,3,4)");
                });
            });
        }

        private static void ConfigureMatches(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LocationDescription).HasMaxLength(500);
                entity.Property(e => e.StartAt).IsRequired();
                entity.Property(e => e.EndAt).IsRequired();
                entity.Property(e => e.MaxParticipants).IsRequired();
                entity.Property(e => e.Status).HasDefaultValue(MatchStatus.Open).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false).IsRequired();

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => new { e.StartAt, e.Status, e.SportId })
                    .HasDatabaseName("IX_Matches_StartAt_Status_SportId")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasIndex(e => e.CourtId)
                    .HasDatabaseName("IX_Matches_CourtId")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasOne(e => e.Host)
                    .WithMany(e => e.HostedMatches)
                    .HasForeignKey(e => e.HostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Sport)
                    .WithMany(e => e.Matches)
                    .HasForeignKey(e => e.SportId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Court)
                    .WithMany(e => e.Matches)
                    .HasForeignKey(e => e.CourtId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Matches_Time", "[StartAt] < [EndAt]");
                    t.HasCheckConstraint("CHK_Matches_Status", "[Status] IN (0,1,2,3)");
                    t.HasCheckConstraint("CHK_Matches_MaxParticipants", "[MaxParticipants] > 0");
                    t.HasCheckConstraint("CHK_Matches_RequiredSkillLevelMin", "[RequiredSkillLevelMin] IS NULL OR [RequiredSkillLevelMin] IN (0,1,2)");
                    t.HasCheckConstraint("CHK_Matches_RequiredSkillLevelMax", "[RequiredSkillLevelMax] IS NULL OR [RequiredSkillLevelMax] IN (0,1,2)");
                    t.HasCheckConstraint("CHK_Matches_RequiredSkillLevelRange", "[RequiredSkillLevelMin] IS NULL OR [RequiredSkillLevelMax] IS NULL OR [RequiredSkillLevelMin] <= [RequiredSkillLevelMax]");
                });
            });
        }

        private static void ConfigureMatchParticipants(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchParticipant>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.JoinedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();
                entity.Property(e => e.IsHost).HasDefaultValue(false).IsRequired();

                entity.HasQueryFilter(e => !e.Match.IsDeleted);

                entity.HasIndex(e => new { e.MatchId, e.PlayerId })
                    .IsUnique()
                    .HasDatabaseName("UQ_MatchParticipants_MatchId_PlayerId");

                entity.HasIndex(e => e.MatchId)
                    .IsUnique()
                    .HasDatabaseName("UX_MatchParticipants_OneHostPerMatch")
                    .HasFilter("[IsHost] = 1");

                entity.HasIndex(e => e.PlayerId)
                    .HasDatabaseName("IX_MatchParticipants_PlayerId");

                entity.HasOne(e => e.Match)
                    .WithMany(e => e.Participants)
                    .HasForeignKey(e => e.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Player)
                    .WithMany(e => e.MatchParticipations)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureMatchJoinRequests(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchJoinRequest>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Status).HasDefaultValue(MatchJoinRequestStatus.Pending).IsRequired();
                entity.Property(e => e.RequestedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.Match.IsDeleted);

                entity.HasIndex(e => new { e.MatchId, e.PlayerId })
                    .IsUnique()
                    .HasDatabaseName("UQ_MatchJoinRequests_MatchId_PlayerId");

                entity.HasIndex(e => e.PlayerId)
                    .HasDatabaseName("IX_MatchJoinRequests_PlayerId");

                entity.HasOne(e => e.Match)
                    .WithMany(e => e.JoinRequests)
                    .HasForeignKey(e => e.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Player)
                    .WithMany(e => e.MatchJoinRequests)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_MatchJoinRequests_Status", "[Status] IN (0,1,2)");
                });
            });
        }

        private static void ConfigureMatchInvitations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchInvitation>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Status).HasDefaultValue(MatchInvitationStatus.Pending).IsRequired();
                entity.Property(e => e.Message).HasMaxLength(500);
                entity.Property(e => e.InvitedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.Match.IsDeleted);

                entity.HasIndex(e => new { e.MatchId, e.InviteeId })
                    .IsUnique()
                    .HasDatabaseName("UQ_MatchInvitations_Match_Invitee");

                entity.HasOne(e => e.Match)
                    .WithMany(e => e.Invitations)
                    .HasForeignKey(e => e.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Inviter)
                    .WithMany(e => e.SentMatchInvitations)
                    .HasForeignKey(e => e.InviterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Invitee)
                    .WithMany(e => e.ReceivedMatchInvitations)
                    .HasForeignKey(e => e.InviteeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_MatchInvitations_Status", "[Status] IN (0,1,2,3)");
                });
            });
        }

        private static void ConfigureReviews(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Rating).HasColumnType("decimal(2,1)").IsRequired();
                entity.Property(e => e.Status).HasDefaultValue(ReviewStatus.Visible).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false).IsRequired();

                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasIndex(e => e.BookingId)
                    .IsUnique()
                    .HasDatabaseName("IX_Reviews_BookingId")
                    .HasFilter("[IsDeleted] = 0");

                entity.HasOne(e => e.Player)
                    .WithMany(e => e.Reviews)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Composite FK: Review phải thuộc về đúng người đã booking.
                entity.HasOne(e => e.Booking)
                    .WithOne(e => e.Review)
                    .HasForeignKey<Review>(e => new { e.BookingId, e.PlayerId })
                    .HasPrincipalKey<Booking>(e => new { e.Id, e.UserProfileId })
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reviews_Booking_Player");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Reviews_Status", "[Status] IN (0,1,2)");
                    t.HasCheckConstraint("CHK_Reviews_Rating", "[Rating] >= 1 AND [Rating] <= 5 AND ([Rating] * 10) % 5 = 0");
                });
            });
        }

        private static void ConfigureReviewImages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReviewImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ImageUrl).IsRequired();
                entity.Property(e => e.DisplayOrder).HasDefaultValue((short)0).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.Review.IsDeleted);

                entity.HasIndex(e => e.ReviewId)
                    .HasDatabaseName("IX_ReviewImages_ReviewId");

                entity.HasOne(e => e.Review)
                    .WithMany(e => e.Images)
                    .HasForeignKey(e => e.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_ReviewImages_DisplayOrder", "[DisplayOrder] >= 0");
                });
            });
        }

        private static void ConfigureNotifications(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.IsRead).HasDefaultValue(false).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired();

                entity.HasQueryFilter(e => !e.User.IsDeleted);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Notifications_Type", "[Type] IN (0,1,2,3,4)");
                });
            });
        }
    }
}
