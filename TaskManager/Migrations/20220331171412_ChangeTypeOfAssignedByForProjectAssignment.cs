using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    public partial class ChangeTypeOfAssignedByForProjectAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AssignedById",
                table: "ProjectAssignments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_AssignedById",
                table: "ProjectAssignments",
                column: "AssignedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAssignments_AspNetUsers_AssignedById",
                table: "ProjectAssignments",
                column: "AssignedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAssignments_AspNetUsers_AssignedById",
                table: "ProjectAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAssignments_AssignedById",
                table: "ProjectAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedById",
                table: "ProjectAssignments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
