using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadeirasAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Cadeira",
                newName: "Alocado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Alocado",
                table: "Cadeira",
                newName: "Status");
        }
    }
}
