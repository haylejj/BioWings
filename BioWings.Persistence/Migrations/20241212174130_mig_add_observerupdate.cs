using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class mig_add_observerupdate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Phone",
            table: "Observers",
            type: "varchar(20)",
            maxLength: 20,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(20)",
            oldMaxLength: 20)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "PasswordHash",
            table: "Observers",
            type: "varchar(70)",
            maxLength: 70,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(70)",
            oldMaxLength: 70)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "FullName",
            table: "Observers",
            type: "longtext",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext")
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Observers",
            type: "varchar(40)",
            maxLength: 40,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(40)",
            oldMaxLength: 40)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Observers",
            keyColumn: "Phone",
            keyValue: null,
            column: "Phone",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "Phone",
            table: "Observers",
            type: "varchar(20)",
            maxLength: 20,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(20)",
            oldMaxLength: 20,
            oldNullable: true)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.UpdateData(
            table: "Observers",
            keyColumn: "PasswordHash",
            keyValue: null,
            column: "PasswordHash",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "PasswordHash",
            table: "Observers",
            type: "varchar(70)",
            maxLength: 70,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(70)",
            oldMaxLength: 70,
            oldNullable: true)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.UpdateData(
            table: "Observers",
            keyColumn: "FullName",
            keyValue: null,
            column: "FullName",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "FullName",
            table: "Observers",
            type: "longtext",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "longtext",
            oldNullable: true)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.UpdateData(
            table: "Observers",
            keyColumn: "Email",
            keyValue: null,
            column: "Email",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Observers",
            type: "varchar(40)",
            maxLength: 40,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(40)",
            oldMaxLength: 40,
            oldNullable: true)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");
    }
}
