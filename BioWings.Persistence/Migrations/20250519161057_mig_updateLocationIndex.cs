using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class mig_updateLocationIndex : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Locations_Latitude_Longitude_SquareRef",
            table: "Locations");

        migrationBuilder.DropIndex(
            name: "IX_Locations_SquareRef",
            table: "Locations");

        migrationBuilder.CreateIndex(
            name: "IX_Locations_SquareRef_CoordinatePrecisionLevel_Latitude_Longit~",
            table: "Locations",
            columns: new[] { "SquareRef", "CoordinatePrecisionLevel", "Latitude", "Longitude" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Locations_SquareRef_CoordinatePrecisionLevel_Latitude_Longit~",
            table: "Locations");

        migrationBuilder.CreateIndex(
            name: "IX_Locations_Latitude_Longitude_SquareRef",
            table: "Locations",
            columns: new[] { "Latitude", "Longitude", "SquareRef" });

        migrationBuilder.CreateIndex(
            name: "IX_Locations_SquareRef",
            table: "Locations",
            column: "SquareRef");
    }
}
