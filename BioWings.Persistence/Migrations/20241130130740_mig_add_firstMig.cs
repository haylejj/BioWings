using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioWings.Persistence.Migrations;

/// <inheritdoc />
public partial class mig_add_firstMig : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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
                FullName = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Email = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                PasswordHash = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
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
            name: "SpeciesTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SpeciesTypes", x => x.Id);
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
                FamilyId = table.Column<int>(type: "int", nullable: false),
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
                ProvinceId = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SquareRef = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Latitude = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Longitude = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Altitude1 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Altitude2 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                XCoord = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                YCoord = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                UtmReference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
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
                    onDelete: ReferentialAction.Restrict);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Species",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                AuthorityId = table.Column<int>(type: "int", nullable: false),
                GenusId = table.Column<int>(type: "int", nullable: false),
                SpeciesTypeId = table.Column<int>(type: "int", nullable: false),
                ScientificName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Name = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                EUName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                FullName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                TurkishName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                EnglishName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                TurkishNamesTrakel = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Trakel = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
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
                table.ForeignKey(
                    name: "FK_Species_SpeciesTypes_SpeciesTypeId",
                    column: x => x.SpeciesTypeId,
                    principalTable: "SpeciesTypes",
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
                FullName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Sex = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ObservationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Method = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Activity = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                LifeStage = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                NumberSeen = table.Column<int>(type: "int", nullable: false),
                Accuracy = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                Notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Source = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Link = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SpeciesId = table.Column<int>(type: "int", nullable: false),
                LocationId = table.Column<int>(type: "int", nullable: false),
                ObserverId = table.Column<int>(type: "int", nullable: false),
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
                    onDelete: ReferentialAction.Restrict);
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
                    onDelete: ReferentialAction.Restrict);
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
                SpeciesId = table.Column<int>(type: "int", nullable: false),
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

        migrationBuilder.CreateTable(
            name: "Media",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                MediaType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                FilePath = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ObservationId = table.Column<int>(type: "int", nullable: false),
                SpeciesId = table.Column<int>(type: "int", nullable: false),
                CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Media", x => x.Id);
                table.ForeignKey(
                    name: "FK_Media_Observations_ObservationId",
                    column: x => x.ObservationId,
                    principalTable: "Observations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Media_Species_SpeciesId",
                    column: x => x.SpeciesId,
                    principalTable: "Species",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_Genera_FamilyId",
            table: "Genera",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_Locations_ProvinceId",
            table: "Locations",
            column: "ProvinceId");

        migrationBuilder.CreateIndex(
            name: "IX_Media_ObservationId",
            table: "Media",
            column: "ObservationId");

        migrationBuilder.CreateIndex(
            name: "IX_Media_SpeciesId",
            table: "Media",
            column: "SpeciesId");

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
            name: "IX_Species_AuthorityId",
            table: "Species",
            column: "AuthorityId");

        migrationBuilder.CreateIndex(
            name: "IX_Species_GenusId",
            table: "Species",
            column: "GenusId");

        migrationBuilder.CreateIndex(
            name: "IX_Species_SpeciesTypeId",
            table: "Species",
            column: "SpeciesTypeId");

        migrationBuilder.CreateIndex(
            name: "IX_Subspecies_SpeciesId",
            table: "Subspecies",
            column: "SpeciesId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Media");

        migrationBuilder.DropTable(
            name: "Subspecies");

        migrationBuilder.DropTable(
            name: "Observations");

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
            name: "SpeciesTypes");

        migrationBuilder.DropTable(
            name: "Families");
    }
}
