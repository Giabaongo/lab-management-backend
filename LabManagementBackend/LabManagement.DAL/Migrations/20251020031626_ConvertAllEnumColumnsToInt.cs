using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ConvertAllEnumColumnsToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "action",
                table: "security_logs",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,0)");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "lab_events",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,0)");

            migrationBuilder.AlterColumn<int>(
                name: "role",
                table: "event_participants",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,0)");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "equipment",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,0)");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "bookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,0)");

            migrationBuilder.UpdateData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 1,
                column: "status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 2,
                column: "status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 3,
                column: "status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 4,
                column: "status",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "action",
                table: "security_logs",
                type: "decimal(2,0)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "status",
                table: "lab_events",
                type: "decimal(2,0)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "role",
                table: "event_participants",
                type: "decimal(2,0)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "status",
                table: "equipment",
                type: "decimal(2,0)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "status",
                table: "bookings",
                type: "decimal(2,0)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 1,
                column: "status",
                value: 1m);

            migrationBuilder.UpdateData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 2,
                column: "status",
                value: 1m);

            migrationBuilder.UpdateData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 3,
                column: "status",
                value: 1m);

            migrationBuilder.UpdateData(
                table: "equipment",
                keyColumn: "equipment_id",
                keyValue: 4,
                column: "status",
                value: 1m);
        }
    }
}
