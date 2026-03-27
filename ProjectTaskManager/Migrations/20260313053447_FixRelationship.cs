using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projecttaskmanager.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tasks_AssigneeId",
                table: "tasks",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_ProjectId",
                table: "tasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_project_OwnerId",
                table: "project",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_notify_UserId",
                table: "notify",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_notify_User_UserId",
                table: "notify",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_project_User_OwnerId",
                table: "project",
                column: "OwnerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_User_AssigneeId",
                table: "tasks",
                column: "AssigneeId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_project_ProjectId",
                table: "tasks",
                column: "ProjectId",
                principalTable: "project",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notify_User_UserId",
                table: "notify");

            migrationBuilder.DropForeignKey(
                name: "FK_project_User_OwnerId",
                table: "project");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_User_AssigneeId",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_project_ProjectId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_AssigneeId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_ProjectId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_project_OwnerId",
                table: "project");

            migrationBuilder.DropIndex(
                name: "IX_notify_UserId",
                table: "notify");
        }
    }
}
