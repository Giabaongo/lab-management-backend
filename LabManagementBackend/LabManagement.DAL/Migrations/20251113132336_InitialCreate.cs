using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "activity_types",
                columns: table => new
                {
                    activity_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__activity__D2470C8792B1F20E", x => x.activity_type_id);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    department_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_public = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__departme__C2232422", x => x.department_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    role = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__B9BE370FBC272336", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "labs",
                columns: table => new
                {
                    lab_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    manager_id = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    is_open = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__labs__66DE64DB381C94E8", x => x.lab_id);
                    table.ForeignKey(
                        name: "FK__labs__departmen__6EC0713C",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "department_id");
                    table.ForeignKey(
                        name: "FK__labs__manager_id__6FB49575",
                        column: x => x.manager_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_departments",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    department_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user_dep__1EDFFB19", x => new { x.user_id, x.department_id });
                    table.ForeignKey(
                        name: "FK__user_depa__depar__74AE549C",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "department_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__user_depa__user___73BA3083",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    equipment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lab_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "lab_zones",
                columns: table => new
                {
                    zone_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lab_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__lab_zone__80B401DFE18A342C", x => x.zone_id);
                    table.ForeignKey(
                        name: "FK__lab_zones__lab_i__72910220",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    lab_id = table.Column<int>(type: "int", nullable: false),
                    zone_id = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())"),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
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
                name: "lab_events",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lab_id = table.Column<int>(type: "int", nullable: false),
                    zone_id = table.Column<int>(type: "int", nullable: false),
                    activity_type_id = table.Column<int>(type: "int", nullable: false),
                    organizer_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())"),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__lab_even__2370F7275CD0E23F", x => x.event_id);
                    table.ForeignKey(
                        name: "FK__lab_event__activ__03BB8E22",
                        column: x => x.activity_type_id,
                        principalTable: "activity_types",
                        principalColumn: "activity_type_id");
                    table.ForeignKey(
                        name: "FK__lab_event__lab_i__01D345B0",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
                    table.ForeignKey(
                        name: "FK__lab_event__organ__04AFB25B",
                        column: x => x.organizer_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__lab_event__zone___02C769E9",
                        column: x => x.zone_id,
                        principalTable: "lab_zones",
                        principalColumn: "zone_id");
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    report_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    zone_id = table.Column<int>(type: "int", nullable: true),
                    lab_id = table.Column<int>(type: "int", nullable: true),
                    report_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    generated_at = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    photo_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__reports__779B7C581E1EA1F5", x => x.report_id);
                    table.ForeignKey(
                        name: "FK__reports__lab_id__17C286CF",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
                    table.ForeignKey(
                        name: "FK__reports__zone_id__18B6AB08",
                        column: x => x.zone_id,
                        principalTable: "lab_zones",
                        principalColumn: "zone_id");
                });

            migrationBuilder.CreateTable(
                name: "event_participants",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__event_pa__C8EB1457D657AA72", x => new { x.event_id, x.user_id });
                    table.ForeignKey(
                        name: "FK__event_par__event__078C1F06",
                        column: x => x.event_id,
                        principalTable: "lab_events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK__event_par__user___0880433F",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recipient_id = table.Column<int>(type: "int", nullable: false),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())"),
                    is_read = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__notifica__E059842F312DB8D7", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK__notificat__event__12FDD1B2",
                        column: x => x.event_id,
                        principalTable: "lab_events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK__notificat__recip__1209AD79",
                        column: x => x.recipient_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "security_logs",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    security_id = table.Column<int>(type: "int", nullable: false),
                    action_type = table.Column<int>(type: "int", nullable: false),
                    photo_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    logged_at = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())"),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__security__9E2397E0366827BF", x => x.log_id);
                    table.ForeignKey(
                        name: "FK__security___event__0C50D423",
                        column: x => x.event_id,
                        principalTable: "lab_events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK__security___secur__0D44F85C",
                        column: x => x.security_id,
                        principalTable: "users",
                        principalColumn: "user_id");
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

            migrationBuilder.CreateIndex(
                name: "IX_event_participants_user_id",
                table: "event_participants",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_events_activity_type_id",
                table: "lab_events",
                column: "activity_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_events_lab_id",
                table: "lab_events",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_events_organizer_id",
                table: "lab_events",
                column: "organizer_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_events_zone_id",
                table: "lab_events",
                column: "zone_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_zones_lab_id",
                table: "lab_zones",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "IX_labs_department_id",
                table: "labs",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_labs_manager_id",
                table: "labs",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_event_id",
                table: "notifications",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_recipient_id",
                table: "notifications",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_lab_id",
                table: "reports",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_user_id",
                table: "reports",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_zone_id",
                table: "reports",
                column: "zone_id");

            migrationBuilder.CreateIndex(
                name: "IX_security_logs_event_id",
                table: "security_logs",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_security_logs_security_id",
                table: "security_logs",
                column: "security_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_departments_department_id",
                table: "user_departments",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E6164598EED90",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "event_participants");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "security_logs");

            migrationBuilder.DropTable(
                name: "user_departments");

            migrationBuilder.DropTable(
                name: "lab_events");

            migrationBuilder.DropTable(
                name: "activity_types");

            migrationBuilder.DropTable(
                name: "lab_zones");

            migrationBuilder.DropTable(
                name: "labs");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
