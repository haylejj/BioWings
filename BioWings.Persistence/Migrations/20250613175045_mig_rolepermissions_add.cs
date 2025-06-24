using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class mig_rolepermissions_add : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "RolePermissions",
            columns: table => new
            {
                RoleId = table.Column<int>(type: "int", nullable: false),
                PermissionId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                table.ForeignKey(
                    name: "FK_RolePermissions_Permissions_PermissionId",
                    column: x => x.PermissionId,
                    principalTable: "Permissions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RolePermissions_Roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_RolePermissions_PermissionId",
            table: "RolePermissions",
            column: "PermissionId");

        migrationBuilder.CreateIndex(
            name: "IX_RolePermissions_RoleId",
            table: "RolePermissions",
            column: "RoleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "RolePermissions");
    }
}
