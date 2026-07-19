using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayCourt.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCourtOwnerDraftAndDocumentUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_CourtOwnerProfiles_VerificationStatus",
                schema: "dbo",
                table: "CourtOwnerProfiles");

            migrationBuilder.AlterColumn<short>(
                name: "VerificationStatus",
                schema: "dbo",
                table: "CourtOwnerProfiles",
                type: "smallint",
                nullable: false,
                defaultValue: (short)3,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessAddress",
                schema: "dbo",
                table: "CourtOwnerProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessLicenseDocumentUrl",
                schema: "dbo",
                table: "CourtOwnerProfiles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SubmittedAt",
                schema: "dbo",
                table: "CourtOwnerProfiles",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_CourtOwnerProfiles_VerificationStatus",
                schema: "dbo",
                table: "CourtOwnerProfiles",
                sql: "[VerificationStatus] IN (0,1,2,3)");

            // Chuyển hồ sơ Pending cũ sang Draft
            // Vì chúng chưa có BusinessLicenseDocumentUrl, nếu giữ Pending sẽ bị khóa
            migrationBuilder.Sql(
                "UPDATE [dbo].[CourtOwnerProfiles] SET [VerificationStatus] = 3 WHERE [VerificationStatus] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_CourtOwnerProfiles_VerificationStatus",
                schema: "dbo",
                table: "CourtOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "BusinessLicenseDocumentUrl",
                schema: "dbo",
                table: "CourtOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                schema: "dbo",
                table: "CourtOwnerProfiles");

            migrationBuilder.AlterColumn<short>(
                name: "VerificationStatus",
                schema: "dbo",
                table: "CourtOwnerProfiles",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)3);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessAddress",
                schema: "dbo",
                table: "CourtOwnerProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_CourtOwnerProfiles_VerificationStatus",
                schema: "dbo",
                table: "CourtOwnerProfiles",
                sql: "[VerificationStatus] IN (0,1,2)");
        }
    }
}
