using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.Migrations
{
    /// <inheritdoc />
    public partial class test11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConditionPay",
                table: "Forecasts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KodClient",
                table: "Forecasts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NumberDoc",
                table: "Forecasts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConditionPay",
                table: "Forecasts");

            migrationBuilder.DropColumn(
                name: "KodClient",
                table: "Forecasts");

            migrationBuilder.DropColumn(
                name: "NumberDoc",
                table: "Forecasts");
        }
    }
}
