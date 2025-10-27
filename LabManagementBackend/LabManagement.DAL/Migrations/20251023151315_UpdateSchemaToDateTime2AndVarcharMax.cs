using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaToDateTime2AndVarcharMax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "security_logs");

            migrationBuilder.RenameColumn(
                name: "action",
                table: "security_logs",
                newName: "action_type");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2(3)",
                nullable: false,
                defaultValueSql: "(sysdatetime())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<string>(
                name: "notes",
                table: "security_logs",
                type: "varchar(max)",
                unicode: false,
                maxLength: -1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "logged_at",
                table: "security_logs",
                type: "datetime2(3)",
                nullable: false,
                defaultValueSql: "(sysdatetime())");

            migrationBuilder.AddColumn<byte[]>(
                name: "row_version",
                table: "security_logs",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<DateTime>(
                name: "generated_at",
                table: "reports",
                type: "datetime2(3)",
                nullable: false,
                defaultValueSql: "(sysdatetime())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "reports",
                type: "varchar(max)",
                unicode: false,
                maxLength: -1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "sent_at",
                table: "notifications",
                type: "datetime2(3)",
                nullable: false,
                defaultValueSql: "(sysdatetime())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "notifications",
                type: "varchar(max)",
                unicode: false,
                maxLength: -1,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "is_read",
                table: "notifications",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "labs",
                type: "varchar(max)",
                unicode: false,
                maxLength: -1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "lab_zones",
                type: "varchar(max)",
                unicode: false,
                maxLength: -1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "lab_events",
                type: "datetime2(3)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "lab_events",
                type: "datetime2(3)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "lab_events",
                type: "varchar(max)",
                unicode: false,
                maxLength: -1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "lab_events",
                type: "datetime2(3)",
                nullable: false,
                defaultValueSql: "(sysdatetime())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<byte[]>(
                name: "row_version",
                table: "lab_events",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "equipment",
                type: "varchar(max)",
                unicode: false,
                maxLength: -1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "bookings",
                type: "datetime2(3)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "notes",
                table: "bookings",
                type: "varchar(max)",
                unicode: false,
                maxLength: -1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "bookings",
                type: "datetime2(3)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "bookings",
                type: "datetime2(3)",
                nullable: false,
                defaultValueSql: "(sysdatetime())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<byte[]>(
                name: "row_version",
                table: "bookings",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "activity_types",
                type: "varchar(max)",
                unicode: false,
                maxLength: -1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "logged_at",
                table: "security_logs");

            migrationBuilder.DropColumn(
                name: "row_version",
                table: "security_logs");

            migrationBuilder.DropColumn(
                name: "row_version",
                table: "lab_events");

            migrationBuilder.DropColumn(
                name: "row_version",
                table: "bookings");

            migrationBuilder.RenameColumn(
                name: "action_type",
                table: "security_logs",
                newName: "action");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)",
                oldDefaultValueSql: "(sysdatetime())");

            migrationBuilder.AlterColumn<string>(
                name: "notes",
                table: "security_logs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldMaxLength: -1,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "timestamp",
                table: "security_logs",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "generated_at",
                table: "reports",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)",
                oldDefaultValueSql: "(sysdatetime())");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "reports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldMaxLength: -1,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "sent_at",
                table: "notifications",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)",
                oldDefaultValueSql: "(sysdatetime())");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "notifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldMaxLength: -1);

            migrationBuilder.AlterColumn<bool>(
                name: "is_read",
                table: "notifications",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "labs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldMaxLength: -1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "lab_zones",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldMaxLength: -1,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "lab_events",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "lab_events",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "lab_events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldMaxLength: -1,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "lab_events",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)",
                oldDefaultValueSql: "(sysdatetime())");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "equipment",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldMaxLength: -1,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "bookings",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)");

            migrationBuilder.AlterColumn<string>(
                name: "notes",
                table: "bookings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldMaxLength: -1,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "bookings",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "bookings",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)",
                oldDefaultValueSql: "(sysdatetime())");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "activity_types",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldMaxLength: -1,
                oldNullable: true);
        }
    }
}
