using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayCourt.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DomainEnumsAndNavigationFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_BookingId_PlayerId",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId_PlayerId",
                schema: "dbo",
                table: "Reviews",
                columns: new[] { "BookingId", "PlayerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_BookingId_PlayerId",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId_PlayerId",
                schema: "dbo",
                table: "Reviews",
                columns: new[] { "BookingId", "PlayerId" });
        }
    }
}
