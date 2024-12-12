using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class mig_add_updatetables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Accuracy",
            table: "Observations");

        migrationBuilder.DropColumn(
            name: "Activity",
            table: "Observations");

        migrationBuilder.DropColumn(
            name: "FullName",
            table: "Observations");

        migrationBuilder.DropColumn(
            name: "Link",
            table: "Observations");

        migrationBuilder.DropColumn(
            name: "Method",
            table: "Observations");

        migrationBuilder.DropColumn(
            name: "XCoord",
            table: "Locations");

        migrationBuilder.DropColumn(
            name: "YCoord",
            table: "Locations");

        migrationBuilder.AlterColumn<string>(
            name: "TurkishNamesTrakel",
            table: "Species",
            type: "varchar(70)",
            maxLength: 70,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(70)",
            oldMaxLength: 70)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "TurkishName",
            table: "Species",
            type: "varchar(70)",
            maxLength: 70,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(70)",
            oldMaxLength: 70)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "Trakel",
            table: "Species",
            type: "varchar(70)",
            maxLength: 70,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(70)",
            oldMaxLength: 70)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Species",
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
            table: "Species",
            type: "varchar(100)",
            maxLength: 100,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(100)",
            oldMaxLength: 100)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "EnglishName",
            table: "Species",
            type: "varchar(70)",
            maxLength: 70,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(70)",
            oldMaxLength: 70)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "EUName",
            table: "Species",
            type: "varchar(70)",
            maxLength: 70,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(70)",
            oldMaxLength: 70)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "HesselbarthName",
            table: "Species",
            type: "varchar(70)",
            maxLength: 70,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "KocakName",
            table: "Species",
            type: "varchar(70)",
            maxLength: 70,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "LocationInfo",
            table: "Observations",
            type: "longtext",
            nullable: false)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "UtmReference",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "SquareRef",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Locations",
            type: "varchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(500)",
            oldMaxLength: 500)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<int>(
            name: "CoordinatePrecisionLevel",
            table: "Locations",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "DecimalDegrees",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "DecimalMinutes",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "DegreesMinutesSeconds",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "MgrsCoordinates",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<decimal>(
            name: "SquareLatitude",
            table: "Locations",
            type: "decimal(9,6)",
            precision: 9,
            scale: 6,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "SquareLongitude",
            table: "Locations",
            type: "decimal(10,6)",
            precision: 10,
            scale: 6,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<string>(
            name: "UtmCoordinates",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "HesselbarthName",
            table: "Species");

        migrationBuilder.DropColumn(
            name: "KocakName",
            table: "Species");

        migrationBuilder.DropColumn(
            name: "LocationInfo",
            table: "Observations");

        migrationBuilder.DropColumn(
            name: "CoordinatePrecisionLevel",
            table: "Locations");

        migrationBuilder.DropColumn(
            name: "DecimalDegrees",
            table: "Locations");

        migrationBuilder.DropColumn(
            name: "DecimalMinutes",
            table: "Locations");

        migrationBuilder.DropColumn(
            name: "DegreesMinutesSeconds",
            table: "Locations");

        migrationBuilder.DropColumn(
            name: "MgrsCoordinates",
            table: "Locations");

        migrationBuilder.DropColumn(
            name: "SquareLatitude",
            table: "Locations");

        migrationBuilder.DropColumn(
            name: "SquareLongitude",
            table: "Locations");

        migrationBuilder.DropColumn(
            name: "UtmCoordinates",
            table: "Locations");

        migrationBuilder.UpdateData(
            table: "Species",
            keyColumn: "TurkishNamesTrakel",
            keyValue: null,
            column: "TurkishNamesTrakel",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "TurkishNamesTrakel",
            table: "Species",
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
            table: "Species",
            keyColumn: "TurkishName",
            keyValue: null,
            column: "TurkishName",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "TurkishName",
            table: "Species",
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
            table: "Species",
            keyColumn: "Trakel",
            keyValue: null,
            column: "Trakel",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "Trakel",
            table: "Species",
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
            table: "Species",
            keyColumn: "Name",
            keyValue: null,
            column: "Name",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Species",
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
            table: "Species",
            keyColumn: "FullName",
            keyValue: null,
            column: "FullName",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "FullName",
            table: "Species",
            type: "varchar(100)",
            maxLength: 100,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(100)",
            oldMaxLength: 100,
            oldNullable: true)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.UpdateData(
            table: "Species",
            keyColumn: "EnglishName",
            keyValue: null,
            column: "EnglishName",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "EnglishName",
            table: "Species",
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
            table: "Species",
            keyColumn: "EUName",
            keyValue: null,
            column: "EUName",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "EUName",
            table: "Species",
            type: "varchar(70)",
            maxLength: 70,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(70)",
            oldMaxLength: 70,
            oldNullable: true)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<decimal>(
            name: "Accuracy",
            table: "Observations",
            type: "decimal(65,30)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<string>(
            name: "Activity",
            table: "Observations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "FullName",
            table: "Observations",
            type: "varchar(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "Link",
            table: "Observations",
            type: "varchar(500)",
            maxLength: 500,
            nullable: false,
            defaultValue: "")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "Method",
            table: "Observations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.UpdateData(
            table: "Locations",
            keyColumn: "UtmReference",
            keyValue: null,
            column: "UtmReference",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "UtmReference",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50,
            oldNullable: true)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.UpdateData(
            table: "Locations",
            keyColumn: "SquareRef",
            keyValue: null,
            column: "SquareRef",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "SquareRef",
            table: "Locations",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50,
            oldNullable: true)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.UpdateData(
            table: "Locations",
            keyColumn: "Description",
            keyValue: null,
            column: "Description",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Locations",
            type: "varchar(500)",
            maxLength: 500,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(500)",
            oldMaxLength: 500,
            oldNullable: true)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<decimal>(
            name: "XCoord",
            table: "Locations",
            type: "decimal(18,6)",
            precision: 18,
            scale: 6,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "YCoord",
            table: "Locations",
            type: "decimal(18,6)",
            precision: 18,
            scale: 6,
            nullable: false,
            defaultValue: 0m);
    }
}
