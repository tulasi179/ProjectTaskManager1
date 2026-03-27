using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projecttaskmanager.Migrations
{
    /// <inheritdoc />
    public partial class taskDepenCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dependent",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    DependentTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dependent", x => new { x.TaskId, x.DependentTaskId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dependent");
        }
    }
}
