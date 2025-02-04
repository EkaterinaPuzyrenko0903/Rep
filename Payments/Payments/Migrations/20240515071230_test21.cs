using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.Migrations
{
    /// <inheritdoc />
    public partial class test21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomingBalance",
                table: "DataInvoicesPays");

            migrationBuilder.DropColumn(
                name: "OutgoingBalance",
                table: "DataInvoicesPays");

            migrationBuilder.AddColumn<int>(
                name: "InitialDatesId",
                table: "DataInvoicesPays",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialDatesId",
                table: "DataInvoicesPays");

            migrationBuilder.AddColumn<double>(
                name: "IncomingBalance",
                table: "DataInvoicesPays",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OutgoingBalance",
                table: "DataInvoicesPays",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
