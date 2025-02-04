using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataInvoicesPays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Kod = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    IdClient = table.Column<int>(type: "INTEGER", nullable: false),
                    IdScore = table.Column<double>(type: "REAL", nullable: false),
                    DateScore = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ViewScore = table.Column<string>(type: "TEXT", nullable: true),
                    payCondition = table.Column<int>(type: "INTEGER", nullable: false),
                    IdPayment = table.Column<double>(type: "REAL", nullable: false),
                    DatePayment = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ViewPayment = table.Column<string>(type: "TEXT", nullable: true),
                    DayPay = table.Column<int>(type: "INTEGER", nullable: false),
                    SumScore = table.Column<double>(type: "REAL", nullable: false),
                    SumPayment = table.Column<double>(type: "REAL", nullable: false),
                    Sum = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataInvoicesPays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InitialDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdKod = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    KodClient = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberDoc = table.Column<int>(type: "INTEGER", nullable: false),
                    DateDoc = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ViewDoc = table.Column<string>(type: "TEXT", nullable: true),
                    ConditionPay = table.Column<int>(type: "INTEGER", nullable: false),
                    Sum = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialDates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataInvoicesPays");

            migrationBuilder.DropTable(
                name: "InitialDates");
        }
    }
}
