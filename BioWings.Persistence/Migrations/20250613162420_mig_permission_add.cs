using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class mig_permission_add : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Permissions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                ControllerName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ActionName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Definition = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ActionType = table.Column<string>(type: "varchar(255)", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                HttpType = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                MenuName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                AreaName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, defaultValue: "")
                    .Annotation("MySql:CharSet", "utf8mb4"),
                PermissionCode = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Permissions", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_Permissions_ActionType",
            table: "Permissions",
            column: "ActionType");

        migrationBuilder.CreateIndex(
            name: "IX_Permissions_Controller_Action_Area",
            table: "Permissions",
            columns: new[] { "ControllerName", "ActionName", "AreaName" });

        migrationBuilder.CreateIndex(
            name: "IX_Permissions_PermissionCode",
            table: "Permissions",
            column: "PermissionCode",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Permissions");
    }
}
