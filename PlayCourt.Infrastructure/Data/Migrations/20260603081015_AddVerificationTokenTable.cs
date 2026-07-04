using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayCourt.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerificationTokens",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<short>(type: "smallint", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationTokens", x => x.Id);
                    table.CheckConstraint("CHK_VerificationTokens_FailedAttempts", "[FailedAttempts] >= 0");
                    table.CheckConstraint("CHK_VerificationTokens_Purpose", "[Purpose] IN (0,1)");
                    table.ForeignKey(
                        name: "FK_VerificationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTokens_User_Purpose_ExpiresAt",
                schema: "dbo",
                table: "VerificationTokens",
                columns: new[] { "UserId", "Purpose", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTokens_UserId",
                schema: "dbo",
                table: "VerificationTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerificationTokens",
                schema: "dbo");
        }
    }
}
