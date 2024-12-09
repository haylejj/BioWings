using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class mig_add_updateLocation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "Longitude",
            table: "Locations",
            type: "decimal(65,30)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50)
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<decimal>(
            name: "Latitude",
            table: "Locations",
            type: "decimal(65,30)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50)
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<decimal>(
            name: "Altitude2",
            table: "Locations",
            type: "decimal(65,30)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50)
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<decimal>(
            name: "Altitude1",
            table: "Locations",
            type: "decimal(65,30)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50)
            .OldAnnotation("MySql:CharSet", "utf8mb4");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Longitude",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)",
            oldMaxLength: 50)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "Latitude",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)",
            oldMaxLength: 50)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "Altitude2",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)",
            oldMaxLength: 50)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "Altitude1",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)",
            oldMaxLength: 50)
            .Annotation("MySql:CharSet", "utf8mb4");
    }
}
