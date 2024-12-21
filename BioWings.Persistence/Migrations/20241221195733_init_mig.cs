using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class init_mig : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Authorities",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Year = table.Column<int>(type: "int", nullable: false),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Authorities", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Families",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Families", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Observers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Surname = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                FullName = table.Column<string>(type: "varchar(255)", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Email = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                PasswordHash = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Observers", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Provinces",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ProvinceCode = table.Column<int>(type: "int", nullable: false),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Provinces", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Genera",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                FamilyId = table.Column<int>(type: "int", nullable: true),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Genera", x => x.Id);
                table.ForeignKey(
                    name: "FK_Genera_Families_FamilyId",
                    column: x => x.FamilyId,
                    principalTable: "Families",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Locations",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                ProvinceId = table.Column<int>(type: "int", nullable: true),
                SquareRef = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SquareLatitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                SquareLongitude = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: false),
                Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                Longitude = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: false),
                DecimalDegrees = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                DegreesMinutesSeconds = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                DecimalMinutes = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                UtmCoordinates = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                MgrsCoordinates = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Altitude1 = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                Altitude2 = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                UtmReference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CoordinatePrecisionLevel = table.Column<int>(type: "int", nullable: false),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Locations", x => x.Id);
                table.ForeignKey(
                    name: "FK_Locations_Provinces_ProvinceId",
                    column: x => x.ProvinceId,
                    principalTable: "Provinces",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Species",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                AuthorityId = table.Column<int>(type: "int", nullable: true),
                GenusId = table.Column<int>(type: "int", nullable: true),
                ScientificName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Name = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                EUName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                FullName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                HesselbarthName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                TurkishName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                EnglishName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                TurkishNamesTrakel = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Trakel = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                KocakName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Species", x => x.Id);
                table.ForeignKey(
                    name: "FK_Species_Authorities_AuthorityId",
                    column: x => x.AuthorityId,
                    principalTable: "Authorities",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Species_Genera_GenusId",
                    column: x => x.GenusId,
                    principalTable: "Genera",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Observations",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                SpeciesId = table.Column<int>(type: "int", nullable: false),
                LocationId = table.Column<int>(type: "int", nullable: true),
                ObserverId = table.Column<int>(type: "int", nullable: true),
                Sex = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ObservationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                LifeStage = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                NumberSeen = table.Column<int>(type: "int", nullable: false),
                Notes = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Source = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                LocationInfo = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Observations", x => x.Id);
                table.ForeignKey(
                    name: "FK_Observations_Locations_LocationId",
                    column: x => x.LocationId,
                    principalTable: "Locations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Observations_Observers_ObserverId",
                    column: x => x.ObserverId,
                    principalTable: "Observers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Observations_Species_SpeciesId",
                    column: x => x.SpeciesId,
                    principalTable: "Species",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Subspecies",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SpeciesId = table.Column<int>(type: "int", nullable: true),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Subspecies", x => x.Id);
                table.ForeignKey(
                    name: "FK_Subspecies_Species_SpeciesId",
                    column: x => x.SpeciesId,
                    principalTable: "Species",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_Authorities_Name",
            table: "Authorities",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_Authorities_Name_Year",
            table: "Authorities",
            columns: new[] { "Name", "Year" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Families_Name",
            table: "Families",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Genera_FamilyId",
            table: "Genera",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_Genera_Name",
            table: "Genera",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_Genera_Name_FamilyId",
            table: "Genera",
            columns: new[] { "Name", "FamilyId" });

        migrationBuilder.CreateIndex(
            name: "IX_Locations_Latitude_Longitude",
            table: "Locations",
            columns: new[] { "Latitude", "Longitude" });

        migrationBuilder.CreateIndex(
            name: "IX_Locations_ProvinceId",
            table: "Locations",
            column: "ProvinceId");

        migrationBuilder.CreateIndex(
            name: "IX_Locations_SquareRef",
            table: "Locations",
            column: "SquareRef");

        migrationBuilder.CreateIndex(
            name: "IX_Observations_LocationId",
            table: "Observations",
            column: "LocationId");

        migrationBuilder.CreateIndex(
            name: "IX_Observations_ObserverId",
            table: "Observations",
            column: "ObserverId");

        migrationBuilder.CreateIndex(
            name: "IX_Observations_SpeciesId",
            table: "Observations",
            column: "SpeciesId");

        migrationBuilder.CreateIndex(
            name: "IX_Observers_FullName",
            table: "Observers",
            column: "FullName");

        migrationBuilder.CreateIndex(
            name: "IX_Species_AuthorityId",
            table: "Species",
            column: "AuthorityId");

        migrationBuilder.CreateIndex(
            name: "IX_Species_GenusId",
            table: "Species",
            column: "GenusId");

        migrationBuilder.CreateIndex(
            name: "IX_Species_Name",
            table: "Species",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_Species_ScientificName",
            table: "Species",
            column: "ScientificName");

        migrationBuilder.CreateIndex(
            name: "IX_Species_ScientificName_GenusId_AuthorityId",
            table: "Species",
            columns: new[] { "ScientificName", "GenusId", "AuthorityId" });

        migrationBuilder.CreateIndex(
            name: "IX_Subspecies_SpeciesId",
            table: "Subspecies",
            column: "SpeciesId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Observations");

        migrationBuilder.DropTable(
            name: "Subspecies");

        migrationBuilder.DropTable(
            name: "Locations");

        migrationBuilder.DropTable(
            name: "Observers");

        migrationBuilder.DropTable(
            name: "Species");

        migrationBuilder.DropTable(
            name: "Provinces");

        migrationBuilder.DropTable(
            name: "Authorities");

        migrationBuilder.DropTable(
            name: "Genera");

        migrationBuilder.DropTable(
            name: "Families");
    }
}
