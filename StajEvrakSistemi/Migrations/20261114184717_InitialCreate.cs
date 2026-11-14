using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajEvrakSistemi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InternshipDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentEmail = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternshipDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentDatasets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdSoyad = table.Column<string>(type: "TEXT", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "TEXT", nullable: false),
                    Uyruk = table.Column<string>(type: "TEXT", nullable: false),
                    OgrenciNo = table.Column<string>(type: "TEXT", nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IkametgahAdresi = table.Column<string>(type: "TEXT", nullable: false),
                    Bolum = table.Column<string>(type: "TEXT", nullable: false),
                    TeslimEttigiYil = table.Column<string>(type: "TEXT", nullable: false),
                    DahaOnceYapilanStajGunSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    Eposta = table.Column<string>(type: "TEXT", nullable: false),
                    Telefon = table.Column<string>(type: "TEXT", nullable: false),
                    SaglikYardimiStatusu = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDatasets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: true),
                    TcKimlikNo = table.Column<string>(type: "TEXT", nullable: true),
                    NameSurname = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternshipDocuments");

            migrationBuilder.DropTable(
                name: "StudentDatasets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
