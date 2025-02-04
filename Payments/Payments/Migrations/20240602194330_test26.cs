using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.Migrations
{
    /// <inheritdoc />
    public partial class test26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Branch",
                table: "paymentForecasts",
                newName: "NumberBranch");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "proceeds",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameBranch",
                table: "paymentForecasts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Payings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "proceeds");

            migrationBuilder.DropColumn(
                name: "NameBranch",
                table: "paymentForecasts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Payings");

            migrationBuilder.RenameColumn(
                name: "NumberBranch",
                table: "paymentForecasts",
                newName: "Branch");
        }
    }
}
