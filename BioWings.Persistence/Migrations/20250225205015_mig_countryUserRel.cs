using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class mig_countryUserRel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CountryName",
            table: "Users");

        migrationBuilder.AddColumn<int>(
            name: "CountryId",
            table: "Users",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_Users_CountryId",
            table: "Users",
            column: "CountryId");

        migrationBuilder.AddForeignKey(
            name: "FK_Users_Countries_CountryId",
            table: "Users",
            column: "CountryId",
            principalTable: "Countries",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Users_Countries_CountryId",
            table: "Users");

        migrationBuilder.DropIndex(
            name: "IX_Users_CountryId",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "CountryId",
            table: "Users");

        migrationBuilder.AddColumn<string>(
            name: "CountryName",
            table: "Users",
            type: "varchar(70)",
            maxLength: 70,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
    }
}
