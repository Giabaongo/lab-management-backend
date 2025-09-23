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
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__activity__D2470C87450D1758", x => x.activity_type_id);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    department_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__departme__C223242226FD0D59", x => x.department_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    role = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    department_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__B9BE370FDD573D70", x => x.user_id);
                    table.ForeignKey(
                        name: "FK__users__departmen__3B75D760",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "department_id");
                });

            migrationBuilder.CreateTable(
                name: "labs",
                columns: table => new
                {
                    lab_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    department_id = table.Column<int>(type: "int", nullable: false),
                    manager_id = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__labs__66DE64DB348D68A0", x => x.lab_id);
                    table.ForeignKey(
                        name: "FK__labs__department__3E52440B",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "department_id");
                    table.ForeignKey(
                        name: "FK__labs__manager_id__3F466844",
                        column: x => x.manager_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "lab_zones",
                columns: table => new
                {
                    zone_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lab_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__lab_zone__80B401DFC554C85A", x => x.zone_id);
                    table.ForeignKey(
                        name: "FK__lab_zones__lab_i__4222D4EF",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
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
                    title = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__lab_even__2370F727FFDCB3A2", x => x.event_id);
                    table.ForeignKey(
                        name: "FK__lab_event__activ__49C3F6B7",
                        column: x => x.activity_type_id,
                        principalTable: "activity_types",
                        principalColumn: "activity_type_id");
                    table.ForeignKey(
                        name: "FK__lab_event__lab_i__47DBAE45",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
                    table.ForeignKey(
                        name: "FK__lab_event__organ__4AB81AF0",
                        column: x => x.organizer_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__lab_event__zone___48CFD27E",
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
                    generated_by = table.Column<int>(type: "int", nullable: false),
                    report_type = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    generated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__reports__779B7C587B55F42C", x => x.report_id);
                    table.ForeignKey(
                        name: "FK__reports__generat__5CD6CB2B",
                        column: x => x.generated_by,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__reports__lab_id__5DCAEF64",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "lab_id");
                    table.ForeignKey(
                        name: "FK__reports__zone_id__5EBF139D",
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
                    role = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__event_pa__C8EB14572030C1E5", x => new { x.event_id, x.user_id });
                    table.ForeignKey(
                        name: "FK__event_par__event__4D94879B",
                        column: x => x.event_id,
                        principalTable: "lab_events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK__event_par__user___4E88ABD4",
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
                    message = table.Column<string>(type: "text", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    is_read = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__notifica__E059842FC3175EFE", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK__notificat__event__59063A47",
                        column: x => x.event_id,
                        principalTable: "lab_events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK__notificat__recip__5812160E",
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
                    action = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    photo_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__security__9E2397E0D77F092D", x => x.log_id);
                    table.ForeignKey(
                        name: "FK__security___event__52593CB8",
                        column: x => x.event_id,
                        principalTable: "lab_events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK__security___secur__534D60F1",
                        column: x => x.security_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

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
                name: "IX_reports_generated_by",
                table: "reports",
                column: "generated_by");

            migrationBuilder.CreateIndex(
                name: "IX_reports_lab_id",
                table: "reports",
                column: "lab_id");

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
                name: "IX_users_department_id",
                table: "users",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E61642BBB0620",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_participants");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "security_logs");

            migrationBuilder.DropTable(
                name: "lab_events");

            migrationBuilder.DropTable(
                name: "activity_types");

            migrationBuilder.DropTable(
                name: "lab_zones");

            migrationBuilder.DropTable(
                name: "labs");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "departments");
        }
    }
}
