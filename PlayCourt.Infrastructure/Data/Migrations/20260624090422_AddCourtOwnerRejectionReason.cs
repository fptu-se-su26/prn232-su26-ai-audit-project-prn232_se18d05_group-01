using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayCourt.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCourtOwnerRejectionReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                schema: "dbo",
                table: "CourtOwnerProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                schema: "dbo",
                table: "CourtOwnerProfiles");
        }
    }
}
