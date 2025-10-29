using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LabManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoUrlAndUserIdToReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__reports__generat__16CE6296",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "IX_reports_generated_by",
                table: "reports");

            migrationBuilder.DeleteData(
                table: "activity_types",
                keyColumn: "activity_type_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "activity_types",
                keyColumn: "activity_type_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "activity_types",
                keyColumn: "activity_type_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "activity_types",
                keyColumn: "activity_type_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "activity_types",
                keyColumn: "activity_type_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "lab_zones",
                keyColumn: "zone_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "lab_zones",
                keyColumn: "zone_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "lab_zones",
                keyColumn: "zone_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "lab_zones",
                keyColumn: "zone_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "lab_zones",
                keyColumn: "zone_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "labs",
                keyColumn: "lab_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "labs",
                keyColumn: "lab_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "labs",
                keyColumn: "lab_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "generated_by",
                table: "reports");

            migrationBuilder.AddColumn<string>(
                name: "photo_url",
                table: "reports",
                type: "varchar(1000)",
                unicode: false,
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "reports",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_read",
                table: "notifications",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_reports_user_id",
                table: "reports",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_reports_user_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "photo_url",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "reports");

            migrationBuilder.AddColumn<int>(
                name: "generated_by",
                table: "reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "is_read",
                table: "notifications",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.InsertData(
                table: "activity_types",
                columns: new[] { "activity_type_id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Hands-on training session", "Workshop" },
                    { 2, "Educational seminar or lecture", "Seminar" },
                    { 3, "Research activity", "Research" },
                    { 4, "Laboratory experiment", "Experiment" },
                    { 5, "Group meeting or discussion", "Meeting" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "created_at", "email", "name", "password_hash", "role" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@lab.com", "Admin User", "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", 0 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "schoolmanager@lab.com", "School Manager", "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", 1 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager@lab.com", "Lab Manager", "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", 2 },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "security@lab.com", "Security Staff", "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", 3 },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "member@lab.com", "Member", "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", 4 }
                });

            migrationBuilder.InsertData(
                table: "labs",
                columns: new[] { "lab_id", "description", "location", "manager_id", "name" },
                values: new object[,]
                {
                    { 1, "Laboratory for biology experiments and research", "Building A - Floor 2", 2, "Biology Lab" },
                    { 2, "Computer lab with 30 workstations", "Building B - Floor 3", 2, "Computer Lab" },
                    { 3, "Chemistry laboratory with safety equipment", "Building A - Floor 1", 3, "Chemistry Lab" }
                });

            migrationBuilder.InsertData(
                table: "equipment",
                columns: new[] { "equipment_id", "code", "description", "lab_id", "name", "status" },
                values: new object[,]
                {
                    { 1, "EQ-001", "Digital microscope with 1000x magnification", 1, "Microscope", 1 },
                    { 2, "EQ-002", "High-speed centrifuge", 1, "Centrifuge", 1 },
                    { 3, "EQ-003", "Workstation with development tools", 2, "Computer Station", 1 },
                    { 4, "EQ-004", "Network server infrastructure", 2, "Server Rack", 1 }
                });

            migrationBuilder.InsertData(
                table: "lab_zones",
                columns: new[] { "zone_id", "description", "lab_id", "name" },
                values: new object[,]
                {
                    { 1, "Main experiment area", 1, "Zone A" },
                    { 2, "Storage and preparation area", 1, "Zone B" },
                    { 3, "Development stations", 2, "Zone A" },
                    { 4, "Server room", 2, "Zone B" },
                    { 5, "Chemical storage", 3, "Zone A" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_reports_generated_by",
                table: "reports",
                column: "generated_by");

            migrationBuilder.AddForeignKey(
                name: "FK__reports__generat__16CE6296",
                table: "reports",
                column: "generated_by",
                principalTable: "users",
                principalColumn: "user_id");
        }
    }
}
