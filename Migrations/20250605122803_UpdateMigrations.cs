using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadeirasAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_alocacao",
                table: "Cadeira");

            migrationBuilder.RenameColumn(
                name: "num_cadeiras",
                table: "Alocacao",
                newName: "cadeira_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cadeira_id",
                table: "Alocacao",
                newName: "num_cadeiras");

            migrationBuilder.AddColumn<int>(
                name: "id_alocacao",
                table: "Cadeira",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
