using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViberLounge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterarStatusParaCancelado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vendas");

            migrationBuilder.AddColumn<bool>(
                name: "Cancelado",
                table: "Vendas",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cancelado",
                table: "Vendas");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Vendas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
