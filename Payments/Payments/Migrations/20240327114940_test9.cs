using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.Migrations
{
    /// <inheritdoc />
    public partial class test9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Unpaids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdKod = table.Column<int>(type: "INTEGER", nullable: false),
                    KodClient = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberDoc = table.Column<string>(type: "TEXT", nullable: true),
                    ViewDocInv = table.Column<string>(type: "TEXT", nullable: true),
                    DateDoc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConditionPay = table.Column<string>(type: "TEXT", nullable: true),
                    Sum = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unpaids", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Unpaids");
        }
    }
}
