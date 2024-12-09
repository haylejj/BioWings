using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class mig_add_updateLocation2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "YCoord",
            table: "Locations",
            type: "decimal(18,6)",
            precision: 18,
            scale: 6,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)");

        migrationBuilder.AlterColumn<decimal>(
            name: "XCoord",
            table: "Locations",
            type: "decimal(18,6)",
            precision: 18,
            scale: 6,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)");

        migrationBuilder.AlterColumn<decimal>(
            name: "Longitude",
            table: "Locations",
            type: "decimal(10,6)",
            precision: 10,
            scale: 6,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<decimal>(
            name: "Latitude",
            table: "Locations",
            type: "decimal(9,6)",
            precision: 9,
            scale: 6,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<decimal>(
            name: "Altitude2",
            table: "Locations",
            type: "decimal(8,2)",
            precision: 8,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<decimal>(
            name: "Altitude1",
            table: "Locations",
            type: "decimal(8,2)",
            precision: 8,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(65,30)",
            oldMaxLength: 50);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "YCoord",
            table: "Locations",
            type: "decimal(65,30)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,6)",
            oldPrecision: 18,
            oldScale: 6);

        migrationBuilder.AlterColumn<decimal>(
            name: "XCoord",
            table: "Locations",
            type: "decimal(65,30)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,6)",
            oldPrecision: 18,
            oldScale: 6);

        migrationBuilder.AlterColumn<decimal>(
            name: "Longitude",
            table: "Locations",
            type: "decimal(65,30)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(10,6)",
            oldPrecision: 10,
            oldScale: 6);

        migrationBuilder.AlterColumn<decimal>(
            name: "Latitude",
            table: "Locations",
            type: "decimal(65,30)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(9,6)",
            oldPrecision: 9,
            oldScale: 6);

        migrationBuilder.AlterColumn<decimal>(
            name: "Altitude2",
            table: "Locations",
            type: "decimal(65,30)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(8,2)",
            oldPrecision: 8,
            oldScale: 2);

        migrationBuilder.AlterColumn<decimal>(
            name: "Altitude1",
            table: "Locations",
            type: "decimal(65,30)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(8,2)",
            oldPrecision: 8,
            oldScale: 2);
    }
}
