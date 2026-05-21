using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayCourt.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Amenities",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sports",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerCount = table.Column<short>(type: "smallint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sports", x => x.Id);
                    table.CheckConstraint("CHK_Sports_PlayerCount", "[PlayerCount] IS NULL OR [PlayerCount] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<short>(type: "smallint", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CHK_Users_Role", "[Role] IN (0,1,2)");
                    table.CheckConstraint("CHK_Users_Status", "[Status] IN (0,1,2)");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    ReferenceType = table.Column<short>(type: "smallint", nullable: true),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    Gender = table.Column<short>(type: "smallint", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.CheckConstraint("CHK_UserProfiles_Gender", "[Gender] IS NULL OR [Gender] IN (0,1,2)");
                    table.ForeignKey(
                        name: "FK_UserProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourtOwnerProfiles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    BusinessName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BusinessLicenseNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BusinessAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationStatus = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtOwnerProfiles", x => x.Id);
                    table.CheckConstraint("CHK_CourtOwnerProfiles_VerificationStatus", "[VerificationStatus] IN (0,1,2)");
                    table.ForeignKey(
                        name: "FK_CourtOwnerProfiles_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSports",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false),
                    SkillLevel = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSports", x => x.Id);
                    table.CheckConstraint("CHK_PlayerSports_SkillLevel", "[SkillLevel] IN (0,1,2)");
                    table.ForeignKey(
                        name: "FK_PlayerSports_Sports_SportId",
                        column: x => x.SportId,
                        principalSchema: "dbo",
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerSports_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtOwnerProfileId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OpenTime = table.Column<TimeSpan>(type: "time(0)", nullable: true),
                    CloseTime = table.Column<TimeSpan>(type: "time(0)", nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                    table.CheckConstraint("CHK_Venues_Latitude", "[Latitude] IS NULL OR ([Latitude] >= -90 AND [Latitude] <= 90)");
                    table.CheckConstraint("CHK_Venues_Longitude", "[Longitude] IS NULL OR ([Longitude] >= -180 AND [Longitude] <= 180)");
                    table.CheckConstraint("CHK_Venues_Status", "[Status] IN (0,1,2,3)");
                    table.ForeignKey(
                        name: "FK_Venues_CourtOwnerProfiles_CourtOwnerProfileId",
                        column: x => x.CourtOwnerProfileId,
                        principalSchema: "dbo",
                        principalTable: "CourtOwnerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Courts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Indoor = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courts", x => x.Id);
                    table.CheckConstraint("CHK_Courts_Status", "[Status] IN (0,1,2)");
                    table.ForeignKey(
                        name: "FK_Courts_Sports_SportId",
                        column: x => x.SportId,
                        principalSchema: "dbo",
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Courts_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "dbo",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteVenues",
                schema: "dbo",
                columns: table => new
                {
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteVenues", x => new { x.UserProfileId, x.VenueId });
                    table.ForeignKey(
                        name: "FK_UserFavoriteVenues_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteVenues_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "dbo",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VenueAmenities",
                schema: "dbo",
                columns: table => new
                {
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueAmenities", x => new { x.VenueId, x.AmenityId });
                    table.ForeignKey(
                        name: "FK_VenueAmenities_Amenities_AmenityId",
                        column: x => x.AmenityId,
                        principalSchema: "dbo",
                        principalTable: "Amenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VenueAmenities_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "dbo",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VenueImages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCover = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueImages_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "dbo",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VenueOpeningHours",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "time(0)", nullable: true),
                    CloseTime = table.Column<TimeSpan>(type: "time(0)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueOpeningHours", x => x.Id);
                    table.CheckConstraint("CHK_VenueOpeningHours_Day", "[DayOfWeek] BETWEEN 1 AND 7");
                    table.CheckConstraint("CHK_VenueOpeningHours_Time", "[IsClosed] = 1 OR ([OpenTime] IS NOT NULL AND [CloseTime] IS NOT NULL AND [OpenTime] < [CloseTime])");
                    table.ForeignKey(
                        name: "FK_VenueOpeningHours_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "dbo",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VenueStaffs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<short>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueStaffs", x => x.Id);
                    table.CheckConstraint("CHK_VenueStaffs_Role", "[Role] IN (0,1,2)");
                    table.ForeignKey(
                        name: "FK_VenueStaffs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VenueStaffs_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "dbo",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    StartAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PlatformFee = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    OwnerEarnings = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.UniqueConstraint("UQ_Bookings_Id_UserProfileId", x => new { x.Id, x.UserProfileId });
                    table.CheckConstraint("CHK_Bookings_Amounts", "[TotalPrice] >= 0 AND [PlatformFee] >= 0 AND [OwnerEarnings] >= 0");
                    table.CheckConstraint("CHK_Bookings_FeeMath", "[TotalPrice] = [PlatformFee] + [OwnerEarnings]");
                    table.CheckConstraint("CHK_Bookings_Status", "[Status] IN (0,1,2,3,4)");
                    table.CheckConstraint("CHK_Bookings_Time", "[StartAt] < [EndAt]");
                    table.ForeignKey(
                        name: "FK_Bookings_Courts_CourtId",
                        column: x => x.CourtId,
                        principalSchema: "dbo",
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourtSchedules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    StartAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtSchedules", x => x.Id);
                    table.CheckConstraint("CHK_CourtSchedules_Time", "[StartAt] < [EndAt]");
                    table.ForeignKey(
                        name: "FK_CourtSchedules_Courts_CourtId",
                        column: x => x.CourtId,
                        principalSchema: "dbo",
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostId = table.Column<int>(type: "int", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: true),
                    LocationDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RequiredSkillLevelMin = table.Column<short>(type: "smallint", nullable: true),
                    RequiredSkillLevelMax = table.Column<short>(type: "smallint", nullable: true),
                    MaxParticipants = table.Column<short>(type: "smallint", nullable: false),
                    CostDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.CheckConstraint("CHK_Matches_MaxParticipants", "[MaxParticipants] > 0");
                    table.CheckConstraint("CHK_Matches_RequiredSkillLevelMax", "[RequiredSkillLevelMax] IS NULL OR [RequiredSkillLevelMax] IN (0,1,2)");
                    table.CheckConstraint("CHK_Matches_RequiredSkillLevelMin", "[RequiredSkillLevelMin] IS NULL OR [RequiredSkillLevelMin] IN (0,1,2)");
                    table.CheckConstraint("CHK_Matches_RequiredSkillLevelRange", "[RequiredSkillLevelMin] IS NULL OR [RequiredSkillLevelMax] IS NULL OR [RequiredSkillLevelMin] <= [RequiredSkillLevelMax]");
                    table.CheckConstraint("CHK_Matches_Status", "[Status] IN (0,1,2,3)");
                    table.CheckConstraint("CHK_Matches_Time", "[StartAt] < [EndAt]");
                    table.ForeignKey(
                        name: "FK_Matches_Courts_CourtId",
                        column: x => x.CourtId,
                        principalSchema: "dbo",
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Matches_Sports_SportId",
                        column: x => x.SportId,
                        principalSchema: "dbo",
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_UserProfiles_HostId",
                        column: x => x.HostId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PricingRules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time(0)", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time(0)", nullable: false),
                    PricePerHour = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingRules", x => x.Id);
                    table.CheckConstraint("CHK_PricingRules_DayOfWeek", "[DayOfWeek] BETWEEN 1 AND 7");
                    table.CheckConstraint("CHK_PricingRules_EffectiveRange", "[EffectiveTo] IS NULL OR [EffectiveFrom] <= [EffectiveTo]");
                    table.CheckConstraint("CHK_PricingRules_Price", "[PricePerHour] >= 0");
                    table.CheckConstraint("CHK_PricingRules_Time", "[StartTime] < [EndTime]");
                    table.ForeignKey(
                        name: "FK_PricingRules_Courts_CourtId",
                        column: x => x.CourtId,
                        principalSchema: "dbo",
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingStatusHistories",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<short>(type: "smallint", nullable: true),
                    NewStatus = table.Column<short>(type: "smallint", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingStatusHistories_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "dbo",
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingStatusHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    PaidAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false, defaultValue: "VND"),
                    ProviderPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.CheckConstraint("CHK_Payments_Amount", "[Amount] >= 0");
                    table.CheckConstraint("CHK_Payments_Status", "[Status] IN (0,1,2)");
                    table.CheckConstraint("CHK_Payments_Type", "[Type] IN (0,1,2)");
                    table.CheckConstraint("CHK_Payments_TypeBookingId", "([Type] = 0 AND [BookingId] IS NOT NULL) OR ([Type] IN (1,2))");
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "dbo",
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(2,1)", nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.CheckConstraint("CHK_Reviews_Rating", "[Rating] >= 1 AND [Rating] <= 5 AND ([Rating] * 10) % 5 = 0");
                    table.CheckConstraint("CHK_Reviews_Status", "[Status] IN (0,1,2)");
                    table.ForeignKey(
                        name: "FK_Reviews_Booking_Player",
                        columns: x => new { x.BookingId, x.PlayerId },
                        principalSchema: "dbo",
                        principalTable: "Bookings",
                        principalColumns: new[] { "Id", "UserProfileId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_UserProfiles_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchInvitations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    InviterId = table.Column<int>(type: "int", nullable: false),
                    InviteeId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InvitedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    RespondedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchInvitations", x => x.Id);
                    table.CheckConstraint("CHK_MatchInvitations_Status", "[Status] IN (0,1,2,3)");
                    table.ForeignKey(
                        name: "FK_MatchInvitations_Matches_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "dbo",
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchInvitations_UserProfiles_InviteeId",
                        column: x => x.InviteeId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchInvitations_UserProfiles_InviterId",
                        column: x => x.InviterId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchJoinRequests",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    RequestedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    RespondedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchJoinRequests", x => x.Id);
                    table.CheckConstraint("CHK_MatchJoinRequests_Status", "[Status] IN (0,1,2)");
                    table.ForeignKey(
                        name: "FK_MatchJoinRequests_Matches_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "dbo",
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchJoinRequests_UserProfiles_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchParticipants",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    IsHost = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchParticipants_Matches_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "dbo",
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchParticipants_UserProfiles_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewImages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewImages", x => x.Id);
                    table.CheckConstraint("CHK_ReviewImages_DisplayOrder", "[DisplayOrder] >= 0");
                    table.ForeignKey(
                        name: "FK_ReviewImages_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "dbo",
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_Amenities_Name",
                schema: "dbo",
                table: "Amenities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Active_Court_Time",
                schema: "dbo",
                table: "Bookings",
                columns: new[] { "CourtId", "StartAt", "EndAt" },
                filter: "[IsDeleted] = 0 AND [Status] IN (0,1)");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CourtId_EndAt_Status",
                schema: "dbo",
                table: "Bookings",
                columns: new[] { "CourtId", "EndAt", "Status" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CourtId_StartAt_Status",
                schema: "dbo",
                table: "Bookings",
                columns: new[] { "CourtId", "StartAt", "Status" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserProfileId",
                schema: "dbo",
                table: "Bookings",
                column: "UserProfileId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatusHistories_BookingId",
                schema: "dbo",
                table: "BookingStatusHistories",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatusHistories_ChangedByUserId",
                schema: "dbo",
                table: "BookingStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "UQ_CourtOwnerProfiles_UserProfileId",
                schema: "dbo",
                table: "CourtOwnerProfiles",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courts_SportId",
                schema: "dbo",
                table: "Courts",
                column: "SportId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Courts_VenueId",
                schema: "dbo",
                table: "Courts",
                column: "VenueId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UX_Courts_VenueId_Name_Active",
                schema: "dbo",
                table: "Courts",
                columns: new[] { "VenueId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CourtSchedules_CourtId_Time",
                schema: "dbo",
                table: "CourtSchedules",
                columns: new[] { "CourtId", "StartAt", "EndAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CourtId",
                schema: "dbo",
                table: "Matches",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_HostId",
                schema: "dbo",
                table: "Matches",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SportId",
                schema: "dbo",
                table: "Matches",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_StartAt_Status_SportId",
                schema: "dbo",
                table: "Matches",
                columns: new[] { "StartAt", "Status", "SportId" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchInvitations_InviteeId",
                schema: "dbo",
                table: "MatchInvitations",
                column: "InviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchInvitations_InviterId",
                schema: "dbo",
                table: "MatchInvitations",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "UQ_MatchInvitations_Match_Invitee",
                schema: "dbo",
                table: "MatchInvitations",
                columns: new[] { "MatchId", "InviteeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchJoinRequests_PlayerId",
                schema: "dbo",
                table: "MatchJoinRequests",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "UQ_MatchJoinRequests_MatchId_PlayerId",
                schema: "dbo",
                table: "MatchJoinRequests",
                columns: new[] { "MatchId", "PlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchParticipants_PlayerId",
                schema: "dbo",
                table: "MatchParticipants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "UQ_MatchParticipants_MatchId_PlayerId",
                schema: "dbo",
                table: "MatchParticipants",
                columns: new[] { "MatchId", "PlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_MatchParticipants_OneHostPerMatch",
                schema: "dbo",
                table: "MatchParticipants",
                column: "MatchId",
                unique: true,
                filter: "[IsHost] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "dbo",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                schema: "dbo",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                schema: "dbo",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_Payments_Provider_TransactionCode",
                schema: "dbo",
                table: "Payments",
                columns: new[] { "Provider", "TransactionCode" },
                unique: true,
                filter: "[TransactionCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSports_SportId",
                schema: "dbo",
                table: "PlayerSports",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "UQ_PlayerSports_UserProfileId_SportId",
                schema: "dbo",
                table: "PlayerSports",
                columns: new[] { "UserProfileId", "SportId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PricingRules_CourtId_DayOfWeek",
                schema: "dbo",
                table: "PricingRules",
                columns: new[] { "CourtId", "DayOfWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewImages_ReviewId",
                schema: "dbo",
                table: "ReviewImages",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId",
                schema: "dbo",
                table: "Reviews",
                column: "BookingId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId_PlayerId",
                schema: "dbo",
                table: "Reviews",
                columns: new[] { "BookingId", "PlayerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PlayerId",
                schema: "dbo",
                table: "Reviews",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "UQ_Sports_Code",
                schema: "dbo",
                table: "Sports",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteVenues_VenueId",
                schema: "dbo",
                table: "UserFavoriteVenues",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserProfiles_UserId",
                schema: "dbo",
                table: "UserProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Users_Email_Active",
                schema: "dbo",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UX_Users_Phone_Active",
                schema: "dbo",
                table: "Users",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_VenueAmenities_AmenityId",
                schema: "dbo",
                table: "VenueAmenities",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "UX_VenueImages_OneCoverPerVenue",
                schema: "dbo",
                table: "VenueImages",
                column: "VenueId",
                unique: true,
                filter: "[IsCover] = 1");

            migrationBuilder.CreateIndex(
                name: "UQ_VenueOpeningHours_Venue_Day",
                schema: "dbo",
                table: "VenueOpeningHours",
                columns: new[] { "VenueId", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Venues_CourtOwnerProfileId",
                schema: "dbo",
                table: "Venues",
                column: "CourtOwnerProfileId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_VenueStaffs_UserId",
                schema: "dbo",
                table: "VenueStaffs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_VenueStaffs_VenueId_UserId",
                schema: "dbo",
                table: "VenueStaffs",
                columns: new[] { "VenueId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingStatusHistories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CourtSchedules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MatchInvitations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MatchJoinRequests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MatchParticipants",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PlayerSports",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PricingRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ReviewImages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserFavoriteVenues",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VenueAmenities",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VenueImages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VenueOpeningHours",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VenueStaffs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Matches",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Amenities",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Bookings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Courts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Sports",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Venues",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CourtOwnerProfiles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserProfiles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");
        }
    }
}
