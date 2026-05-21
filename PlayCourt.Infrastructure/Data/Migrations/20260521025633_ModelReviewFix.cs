using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayCourt.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModelReviewFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Matches_CourtId",
                schema: "dbo",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_StartAt_Status_SportId",
                schema: "dbo",
                table: "Matches");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Matches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Notifications_Type",
                schema: "dbo",
                table: "Notifications",
                sql: "[Type] IN (0,1,2,3,4)");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CourtId",
                schema: "dbo",
                table: "Matches",
                column: "CourtId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_StartAt_Status_SportId",
                schema: "dbo",
                table: "Matches",
                columns: new[] { "StartAt", "Status", "SportId" },
                filter: "[IsDeleted] = 0");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_Notifications_Type",
                schema: "dbo",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Matches_CourtId",
                schema: "dbo",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_StartAt_Status_SportId",
                schema: "dbo",
                table: "Matches");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_BookingStatusHistories_NewStatus",
                schema: "dbo",
                table: "BookingStatusHistories");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_BookingStatusHistories_OldStatus",
                schema: "dbo",
                table: "BookingStatusHistories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Matches");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CourtId",
                schema: "dbo",
                table: "Matches",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_StartAt_Status_SportId",
                schema: "dbo",
                table: "Matches",
                columns: new[] { "StartAt", "Status", "SportId" });
        }
    }
}
