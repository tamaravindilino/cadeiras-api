using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadeirasAPI.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alocacao_Cadeira_CadeiraId",
                table: "Alocacao");

            migrationBuilder.DropColumn(
                name: "Alocado",
                table: "Cadeira");

            migrationBuilder.RenameColumn(
                name: "Numero",
                table: "Cadeira",
                newName: "numero");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "Cadeira",
                newName: "descricao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Cadeira",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CadeiraId",
                table: "Alocacao",
                newName: "Cadeiraid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Alocacao",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DataHoraInicio",
                table: "Alocacao",
                newName: "data_hora_init");

            migrationBuilder.RenameColumn(
                name: "DataHoraFim",
                table: "Alocacao",
                newName: "data_hora_fim");

            migrationBuilder.RenameIndex(
                name: "IX_Alocacao_CadeiraId",
                table: "Alocacao",
                newName: "IX_Alocacao_Cadeiraid");

            migrationBuilder.AddColumn<int>(
                name: "id_alocacao",
                table: "Cadeira",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "Cadeiraid",
                table: "Alocacao",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "num_cadeiras",
                table: "Alocacao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Alocacao_Cadeira_Cadeiraid",
                table: "Alocacao",
                column: "Cadeiraid",
                principalTable: "Cadeira",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alocacao_Cadeira_Cadeiraid",
                table: "Alocacao");

            migrationBuilder.DropColumn(
                name: "id_alocacao",
                table: "Cadeira");

            migrationBuilder.DropColumn(
                name: "num_cadeiras",
                table: "Alocacao");

            migrationBuilder.RenameColumn(
                name: "numero",
                table: "Cadeira",
                newName: "Numero");

            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "Cadeira",
                newName: "Descricao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Cadeira",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Cadeiraid",
                table: "Alocacao",
                newName: "CadeiraId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Alocacao",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "data_hora_init",
                table: "Alocacao",
                newName: "DataHoraInicio");

            migrationBuilder.RenameColumn(
                name: "data_hora_fim",
                table: "Alocacao",
                newName: "DataHoraFim");

            migrationBuilder.RenameIndex(
                name: "IX_Alocacao_Cadeiraid",
                table: "Alocacao",
                newName: "IX_Alocacao_CadeiraId");

            migrationBuilder.AddColumn<bool>(
                name: "Alocado",
                table: "Cadeira",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "CadeiraId",
                table: "Alocacao",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Alocacao_Cadeira_CadeiraId",
                table: "Alocacao",
                column: "CadeiraId",
                principalTable: "Cadeira",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
