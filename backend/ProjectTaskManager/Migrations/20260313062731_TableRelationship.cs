using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projecttaskmanager.Migrations
{
    /// <inheritdoc />
    public partial class TableRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_project_ProjectId",
                table: "tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_project_ProjectId",
                table: "tasks",
                column: "ProjectId",
                principalTable: "project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_project_ProjectId",
                table: "tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_project_ProjectId",
                table: "tasks",
                column: "ProjectId",
                principalTable: "project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
