using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadeirasAPI.Migrations
{
    /// <inheritdoc />
    public partial class updates_ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alocacao_Cadeira_Cadeiraid",
                table: "Alocacao");

            migrationBuilder.DropIndex(
                name: "IX_Alocacao_Cadeiraid",
                table: "Alocacao");

            migrationBuilder.DropColumn(
                name: "Cadeiraid",
                table: "Alocacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Cadeiraid",
                table: "Alocacao",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Alocacao_Cadeiraid",
                table: "Alocacao",
                column: "Cadeiraid");

            migrationBuilder.AddForeignKey(
                name: "FK_Alocacao_Cadeira_Cadeiraid",
                table: "Alocacao",
                column: "Cadeiraid",
                principalTable: "Cadeira",
                principalColumn: "id");
        }
    }
}
