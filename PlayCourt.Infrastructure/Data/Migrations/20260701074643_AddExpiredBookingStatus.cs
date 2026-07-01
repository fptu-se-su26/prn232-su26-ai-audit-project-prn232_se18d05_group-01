using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayCourt.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExpiredBookingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_BookingStatusHistories_NewStatus",
                schema: "dbo",
                table: "BookingStatusHistories");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_BookingStatusHistories_OldStatus",
                schema: "dbo",
                table: "BookingStatusHistories");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Bookings_Status",
                schema: "dbo",
                table: "Bookings");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_BookingStatusHistories_NewStatus",
                schema: "dbo",
                table: "BookingStatusHistories",
                sql: "[NewStatus] IN (0,1,2,3,4,5)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_BookingStatusHistories_OldStatus",
                schema: "dbo",
                table: "BookingStatusHistories",
                sql: "[OldStatus] IS NULL OR [OldStatus] IN (0,1,2,3,4,5)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Bookings_Status",
                schema: "dbo",
                table: "Bookings",
                sql: "[Status] IN (0,1,2,3,4,5)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_BookingStatusHistories_NewStatus",
                schema: "dbo",
                table: "BookingStatusHistories");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_BookingStatusHistories_OldStatus",
                schema: "dbo",
                table: "BookingStatusHistories");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Bookings_Status",
                schema: "dbo",
                table: "Bookings");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_BookingStatusHistories_NewStatus",
                schema: "dbo",
                table: "BookingStatusHistories",
                sql: "[NewStatus] IN (0,1,2,3,4)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_BookingStatusHistories_OldStatus",
                schema: "dbo",
                table: "BookingStatusHistories",
                sql: "[OldStatus] IS NULL OR [OldStatus] IN (0,1,2,3,4)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Bookings_Status",
                schema: "dbo",
                table: "Bookings",
                sql: "[Status] IN (0,1,2,3,4)");
        }
    }
}
