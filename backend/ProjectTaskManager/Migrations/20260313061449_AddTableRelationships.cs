using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projecttaskmanager.Migrations
{
    /// <inheritdoc />
    public partial class AddTableRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_project_User_OwnerId",
                table: "project");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_User_AssigneeId",
                table: "tasks");

            migrationBuilder.CreateIndex(
                name: "IX_dependent_DependentTaskId",
                table: "dependent",
                column: "DependentTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_dependent_tasks_DependentTaskId",
                table: "dependent",
                column: "DependentTaskId",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dependent_tasks_TaskId",
                table: "dependent",
                column: "TaskId",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_project_User_OwnerId",
                table: "project",
                column: "OwnerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_User_AssigneeId",
                table: "tasks",
                column: "AssigneeId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dependent_tasks_DependentTaskId",
                table: "dependent");

            migrationBuilder.DropForeignKey(
                name: "FK_dependent_tasks_TaskId",
                table: "dependent");

            migrationBuilder.DropForeignKey(
                name: "FK_project_User_OwnerId",
                table: "project");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_User_AssigneeId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_dependent_DependentTaskId",
                table: "dependent");

            migrationBuilder.AddForeignKey(
                name: "FK_project_User_OwnerId",
                table: "project",
                column: "OwnerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_User_AssigneeId",
                table: "tasks",
                column: "AssigneeId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
