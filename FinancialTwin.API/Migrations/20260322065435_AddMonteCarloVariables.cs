using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialTwin.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMonteCarloVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AnnualVolatility",
                table: "Simulations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<List<decimal>>(
                name: "P10Trajectory",
                table: "Simulations",
                type: "numeric[]",
                nullable: false,
                defaultValueSql: "'{}'");

            migrationBuilder.AddColumn<List<decimal>>(
                name: "P50Trajectory",
                table: "Simulations",
                type: "numeric[]",
                nullable: false,
                defaultValueSql: "'{}'");

            migrationBuilder.AddColumn<List<decimal>>(
                name: "P90Trajectory",
                table: "Simulations",
                type: "numeric[]",
                nullable: false,
                defaultValueSql: "'{}'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualVolatility",
                table: "Simulations");

            migrationBuilder.DropColumn(
                name: "P10Trajectory",
                table: "Simulations");

            migrationBuilder.DropColumn(
                name: "P50Trajectory",
                table: "Simulations");

            migrationBuilder.DropColumn(
                name: "P90Trajectory",
                table: "Simulations");
        }
    }
}
