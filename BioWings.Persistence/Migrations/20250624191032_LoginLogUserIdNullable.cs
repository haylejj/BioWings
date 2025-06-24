using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LoginLogUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginLogs_Users_UserId",
                table: "LoginLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_LoginLogs_Users_UserId",
                table: "LoginLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginLogs_Users_UserId",
                table: "LoginLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_LoginLogs_Users_UserId",
                table: "LoginLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
