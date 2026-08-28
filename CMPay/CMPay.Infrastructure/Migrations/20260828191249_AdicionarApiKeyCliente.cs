using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarApiKeyCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKeyHash",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKeyHash",
                table: "Clientes");
        }
    }
}
