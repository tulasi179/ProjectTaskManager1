using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projecttaskmanager.Migrations
{
    /// <inheritdoc />
    public partial class AddOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpireAt",
                table: "OtpCodes",
                newName: "ExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "OtpCodes",
                newName: "ExpireAt");
        }
    }
}
