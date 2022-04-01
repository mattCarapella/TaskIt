using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    public partial class AddTicketCompletionDateAndAssignedByUsedId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedByUsedId",
                table: "TicketAssignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedByUsedId",
                table: "ProjectAssignments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AssignedByUsedId",
                table: "TicketAssignments");

            migrationBuilder.DropColumn(
                name: "AssignedByUsedId",
                table: "ProjectAssignments");
        }
    }
}
