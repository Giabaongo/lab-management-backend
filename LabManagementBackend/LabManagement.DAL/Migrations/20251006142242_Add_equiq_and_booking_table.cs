using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LabManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Add_equiq_and_booking_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__event_par__event__151B244E",
                table: "event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK__event_par__user___160F4887",
                table: "event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__activ__114A936A",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__lab_i__0F624AF8",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__organ__123EB7A3",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__zone___10566F31",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_zones__lab_i__09A971A2",
                table: "lab_zones");

            migrationBuilder.DropForeignKey(
                name: "FK__labs__manager_id__06CD04F7",
                table: "labs");

            migrationBuilder.DropForeignKey(
                name: "FK__notificat__event__208CD6FA",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK__notificat__recip__1F98B2C1",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK__reports__generat__245D67DE",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK__reports__lab_id__25518C17",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK__reports__zone_id__2645B050",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK__security___event__19DFD96B",
                table: "security_logs");

            migrationBuilder.DropForeignKey(
                name: "FK__security___secur__1AD3FDA4",
                table: "security_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK__users__B9BE370F20E930E3",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK__security__9E2397E0DFE7202D",
                table: "security_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK__reports__779B7C58021353BD",
                table: "reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK__notifica__E059842F54DF105B",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK__labs__66DE64DB63EAC51C",
                table: "labs");

            migrationBuilder.DropPrimaryKey(
                name: "PK__lab_zone__80B401DF8DC1AAD7",
                table: "lab_zones");

            migrationBuilder.DropPrimaryKey(
                name: "PK__lab_even__2370F7270A85F029",
                table: "lab_events");

            migrationBuilder.DropPrimaryKey(
                name: "PK__event_pa__C8EB1457151547CC",
                table: "event_participants");

            migrationBuilder.DropPrimaryKey(
                name: "PK__activity__D2470C87E6BFD9E2",
                table: "activity_types");

            migrationBuilder.RenameIndex(
                name: "UQ__users__AB6E6164DB54CFBB",
                table: "users",
                newName: "UQ__users__AB6E6164598EED90");

            migrationBuilder.AlterColumn<decimal>(
                name: "action",
                table: "security_logs",
                type: "decimal(2,0)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "status",
                table: "lab_events",
                type: "decimal(2,0)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20);

            migrationBuilder.AddPrimaryKey(
                name: "PK__users__B9BE370FBC272336",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__security__9E2397E0366827BF",
                table: "security_logs",
                column: "log_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__reports__779B7C581E1EA1F5",
                table: "reports",
                column: "report_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__notifica__E059842F312DB8D7",
                table: "notifications",
                column: "notification_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__labs__66DE64DB381C94E8",
                table: "labs",
                column: "lab_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__lab_zone__80B401DFE18A342C",
                table: "lab_zones",
                column: "zone_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__lab_even__2370F7275CD0E23F",
                table: "lab_events",
                column: "event_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__event_pa__C8EB1457D657AA72",
                table: "event_participants",
                columns: new[] { "event_id", "user_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK__activity__D2470C8792B1F20E",
                table: "activity_types",
                column: "activity_type_id");

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    lab_id = table.Column<int>(type: "int", nullable: false),
                    zone_id = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<decimal>(type: "decimal(2,0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__bookings__5DE3A5B17836BDD3", x => x.booking_id);
                    table.ForeignKey(
                        name: "FK__bookings__lab_id__7D0E9093",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
                    table.ForeignKey(
                        name: "FK__bookings__user_i__7C1A6C5A",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__bookings__zone_i__7E02B4CC",
                        column: x => x.zone_id,
                        principalTable: "lab_zones",
                        principalColumn: "zone_id");
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    equipment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lab_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<decimal>(type: "decimal(2,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__equipmen__197068AFC616B45E", x => x.equipment_id);
                    table.ForeignKey(
                        name: "FK__equipment__lab_i__7849DB76",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
                });

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
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@lab.com", "Admin User", "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", 1m },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "schoolmanager@lab.com", "School Manager", "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", 2m },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager@lab.com", "Lab Manager", "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", 3m },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "security@lab.com", "Security Staff", "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", 4m },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "student@lab.com", "Student User", "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", 5m }
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
                    { 1, "EQ-001", "Digital microscope with 1000x magnification", 1, "Microscope", 1m },
                    { 2, "EQ-002", "High-speed centrifuge", 1, "Centrifuge", 1m },
                    { 3, "EQ-003", "Workstation with development tools", 2, "Computer Station", 1m },
                    { 4, "EQ-004", "Network server infrastructure", 2, "Server Rack", 1m }
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
                name: "IX_bookings_lab_id",
                table: "bookings",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_user_id",
                table: "bookings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_zone_id",
                table: "bookings",
                column: "zone_id");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_lab_id",
                table: "equipment",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "UQ__equipmen__357D4CF958F5304E",
                table: "equipment",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK__event_par__event__078C1F06",
                table: "event_participants",
                column: "event_id",
                principalTable: "lab_events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK__event_par__user___0880433F",
                table: "event_participants",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__activ__03BB8E22",
                table: "lab_events",
                column: "activity_type_id",
                principalTable: "activity_types",
                principalColumn: "activity_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__lab_i__01D345B0",
                table: "lab_events",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "lab_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__organ__04AFB25B",
                table: "lab_events",
                column: "organizer_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__zone___02C769E9",
                table: "lab_events",
                column: "zone_id",
                principalTable: "lab_zones",
                principalColumn: "zone_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_zones__lab_i__72910220",
                table: "lab_zones",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "lab_id");

            migrationBuilder.AddForeignKey(
                name: "FK__labs__manager_id__6FB49575",
                table: "labs",
                column: "manager_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__notificat__event__12FDD1B2",
                table: "notifications",
                column: "event_id",
                principalTable: "lab_events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK__notificat__recip__1209AD79",
                table: "notifications",
                column: "recipient_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__reports__generat__16CE6296",
                table: "reports",
                column: "generated_by",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__reports__lab_id__17C286CF",
                table: "reports",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "lab_id");

            migrationBuilder.AddForeignKey(
                name: "FK__reports__zone_id__18B6AB08",
                table: "reports",
                column: "zone_id",
                principalTable: "lab_zones",
                principalColumn: "zone_id");

            migrationBuilder.AddForeignKey(
                name: "FK__security___event__0C50D423",
                table: "security_logs",
                column: "event_id",
                principalTable: "lab_events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK__security___secur__0D44F85C",
                table: "security_logs",
                column: "security_id",
                principalTable: "users",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__event_par__event__078C1F06",
                table: "event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK__event_par__user___0880433F",
                table: "event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__activ__03BB8E22",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__lab_i__01D345B0",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__organ__04AFB25B",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__zone___02C769E9",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_zones__lab_i__72910220",
                table: "lab_zones");

            migrationBuilder.DropForeignKey(
                name: "FK__labs__manager_id__6FB49575",
                table: "labs");

            migrationBuilder.DropForeignKey(
                name: "FK__notificat__event__12FDD1B2",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK__notificat__recip__1209AD79",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK__reports__generat__16CE6296",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK__reports__lab_id__17C286CF",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK__reports__zone_id__18B6AB08",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK__security___event__0C50D423",
                table: "security_logs");

            migrationBuilder.DropForeignKey(
                name: "FK__security___secur__0D44F85C",
                table: "security_logs");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK__users__B9BE370FBC272336",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK__security__9E2397E0366827BF",
                table: "security_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK__reports__779B7C581E1EA1F5",
                table: "reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK__notifica__E059842F312DB8D7",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK__labs__66DE64DB381C94E8",
                table: "labs");

            migrationBuilder.DropPrimaryKey(
                name: "PK__lab_zone__80B401DFE18A342C",
                table: "lab_zones");

            migrationBuilder.DropPrimaryKey(
                name: "PK__lab_even__2370F7275CD0E23F",
                table: "lab_events");

            migrationBuilder.DropPrimaryKey(
                name: "PK__event_pa__C8EB1457D657AA72",
                table: "event_participants");

            migrationBuilder.DropPrimaryKey(
                name: "PK__activity__D2470C8792B1F20E",
                table: "activity_types");

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

            migrationBuilder.RenameIndex(
                name: "UQ__users__AB6E6164598EED90",
                table: "users",
                newName: "UQ__users__AB6E6164DB54CFBB");

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "security_logs",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,0)");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "lab_events",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,0)");

            migrationBuilder.AddPrimaryKey(
                name: "PK__users__B9BE370F20E930E3",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__security__9E2397E0DFE7202D",
                table: "security_logs",
                column: "log_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__reports__779B7C58021353BD",
                table: "reports",
                column: "report_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__notifica__E059842F54DF105B",
                table: "notifications",
                column: "notification_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__labs__66DE64DB63EAC51C",
                table: "labs",
                column: "lab_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__lab_zone__80B401DF8DC1AAD7",
                table: "lab_zones",
                column: "zone_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__lab_even__2370F7270A85F029",
                table: "lab_events",
                column: "event_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__event_pa__C8EB1457151547CC",
                table: "event_participants",
                columns: new[] { "event_id", "user_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK__activity__D2470C87E6BFD9E2",
                table: "activity_types",
                column: "activity_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK__event_par__event__151B244E",
                table: "event_participants",
                column: "event_id",
                principalTable: "lab_events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK__event_par__user___160F4887",
                table: "event_participants",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__activ__114A936A",
                table: "lab_events",
                column: "activity_type_id",
                principalTable: "activity_types",
                principalColumn: "activity_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__lab_i__0F624AF8",
                table: "lab_events",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "lab_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__organ__123EB7A3",
                table: "lab_events",
                column: "organizer_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__zone___10566F31",
                table: "lab_events",
                column: "zone_id",
                principalTable: "lab_zones",
                principalColumn: "zone_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_zones__lab_i__09A971A2",
                table: "lab_zones",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "lab_id");

            migrationBuilder.AddForeignKey(
                name: "FK__labs__manager_id__06CD04F7",
                table: "labs",
                column: "manager_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__notificat__event__208CD6FA",
                table: "notifications",
                column: "event_id",
                principalTable: "lab_events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK__notificat__recip__1F98B2C1",
                table: "notifications",
                column: "recipient_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__reports__generat__245D67DE",
                table: "reports",
                column: "generated_by",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__reports__lab_id__25518C17",
                table: "reports",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "lab_id");

            migrationBuilder.AddForeignKey(
                name: "FK__reports__zone_id__2645B050",
                table: "reports",
                column: "zone_id",
                principalTable: "lab_zones",
                principalColumn: "zone_id");

            migrationBuilder.AddForeignKey(
                name: "FK__security___event__19DFD96B",
                table: "security_logs",
                column: "event_id",
                principalTable: "lab_events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK__security___secur__1AD3FDA4",
                table: "security_logs",
                column: "security_id",
                principalTable: "users",
                principalColumn: "user_id");
        }
    }
}
