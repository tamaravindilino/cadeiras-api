using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadeirasAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "cadeira_id",
                table: "Alocacao",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Alocacao_cadeira_id",
                table: "Alocacao",
                column: "cadeira_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Alocacao_Cadeira_cadeira_id",
                table: "Alocacao",
                column: "cadeira_id",
                principalTable: "Cadeira",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alocacao_Cadeira_cadeira_id",
                table: "Alocacao");

            migrationBuilder.DropIndex(
                name: "IX_Alocacao_cadeira_id",
                table: "Alocacao");

            migrationBuilder.AlterColumn<int>(
                name: "cadeira_id",
                table: "Alocacao",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
