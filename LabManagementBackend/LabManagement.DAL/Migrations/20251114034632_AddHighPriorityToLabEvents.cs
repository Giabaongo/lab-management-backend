using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddHighPriorityToLabEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_high_priority",
                table: "lab_events",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_high_priority",
                table: "lab_events");
        }
    }
}
