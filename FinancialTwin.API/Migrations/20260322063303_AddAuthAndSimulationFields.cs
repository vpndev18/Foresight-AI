using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialTwin.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthAndSimulationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "AnnualReturnRate",
                table: "Simulations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialSavings",
                table: "Simulations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyContribution",
                table: "Simulations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AnnualReturnRate",
                table: "Simulations");

            migrationBuilder.DropColumn(
                name: "InitialSavings",
                table: "Simulations");

            migrationBuilder.DropColumn(
                name: "MonthlyContribution",
                table: "Simulations");
        }
    }
}
