using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class mig_updateObserver : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Email",
            table: "Observers");

        migrationBuilder.DropColumn(
            name: "PasswordHash",
            table: "Observers");

        migrationBuilder.DropColumn(
            name: "Phone",
            table: "Observers");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "Observers",
            type: "varchar(40)",
            maxLength: 40,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "PasswordHash",
            table: "Observers",
            type: "varchar(70)",
            maxLength: 70,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "Phone",
            table: "Observers",
            type: "varchar(20)",
            maxLength: 20,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
    }
}
