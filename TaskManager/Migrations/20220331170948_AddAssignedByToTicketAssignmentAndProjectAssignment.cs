using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    public partial class AddAssignedByToTicketAssignmentAndProjectAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_AssignedById",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AssignedById",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "Tickets");

            migrationBuilder.AddColumn<string>(
                name: "AssignedById",
                table: "TicketAssignments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedById",
                table: "ProjectAssignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketAssignments_AssignedById",
                table: "TicketAssignments",
                column: "AssignedById");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAssignments_AspNetUsers_AssignedById",
                table: "TicketAssignments",
                column: "AssignedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAssignments_AspNetUsers_AssignedById",
                table: "TicketAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TicketAssignments_AssignedById",
                table: "TicketAssignments");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "TicketAssignments");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "ProjectAssignments");

            migrationBuilder.AddColumn<string>(
                name: "AssignedById",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedById",
                table: "Tickets",
                column: "AssignedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_AssignedById",
                table: "Tickets",
                column: "AssignedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
