using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarIdempotencyKeyPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pagamentos_IDCliente",
                table: "Pagamentos");

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "Pagamentos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PayloadHash",
                table: "Pagamentos",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_IDCliente_IdempotencyKey",
                table: "Pagamentos",
                columns: new[] { "IDCliente", "IdempotencyKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pagamentos_IDCliente_IdempotencyKey",
                table: "Pagamentos");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "Pagamentos");

            migrationBuilder.DropColumn(
                name: "PayloadHash",
                table: "Pagamentos");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_IDCliente",
                table: "Pagamentos",
                column: "IDCliente");
        }
    }
}
