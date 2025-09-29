using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Remove_department_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__event_par__event__4D94879B",
                table: "event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK__event_par__user___4E88ABD4",
                table: "event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__activ__49C3F6B7",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__lab_i__47DBAE45",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__organ__4AB81AF0",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_event__zone___48CFD27E",
                table: "lab_events");

            migrationBuilder.DropForeignKey(
                name: "FK__lab_zones__lab_i__4222D4EF",
                table: "lab_zones");

            migrationBuilder.DropForeignKey(
                name: "FK__labs__department__3E52440B",
                table: "labs");

            migrationBuilder.DropForeignKey(
                name: "FK__labs__manager_id__3F466844",
                table: "labs");

            migrationBuilder.DropForeignKey(
                name: "FK__notificat__event__59063A47",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK__notificat__recip__5812160E",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK__reports__generat__5CD6CB2B",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK__reports__lab_id__5DCAEF64",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK__reports__zone_id__5EBF139D",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK__security___event__52593CB8",
                table: "security_logs");

            migrationBuilder.DropForeignKey(
                name: "FK__security___secur__534D60F1",
                table: "security_logs");

            migrationBuilder.DropForeignKey(
                name: "FK__users__departmen__3B75D760",
                table: "users");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK__users__B9BE370FDD573D70",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_department_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK__security__9E2397E0D77F092D",
                table: "security_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK__reports__779B7C587B55F42C",
                table: "reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK__notifica__E059842FC3175EFE",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK__labs__66DE64DB348D68A0",
                table: "labs");

            migrationBuilder.DropIndex(
                name: "IX_labs_department_id",
                table: "labs");

            migrationBuilder.DropPrimaryKey(
                name: "PK__lab_zone__80B401DFC554C85A",
                table: "lab_zones");

            migrationBuilder.DropPrimaryKey(
                name: "PK__lab_even__2370F727FFDCB3A2",
                table: "lab_events");

            migrationBuilder.DropPrimaryKey(
                name: "PK__event_pa__C8EB14572030C1E5",
                table: "event_participants");

            migrationBuilder.DropPrimaryKey(
                name: "PK__activity__D2470C87450D1758",
                table: "activity_types");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "labs");

            migrationBuilder.RenameIndex(
                name: "UQ__users__AB6E61642BBB0620",
                table: "users",
                newName: "UQ__users__AB6E6164DB54CFBB");

            migrationBuilder.AlterColumn<decimal>(
                name: "role",
                table: "users",
                type: "decimal(2,0)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "role",
                table: "event_participants",
                type: "decimal(2,0)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                newName: "UQ__users__AB6E61642BBB0620");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "users",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,0)");

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "labs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "event_participants",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,0)");

            migrationBuilder.AddPrimaryKey(
                name: "PK__users__B9BE370FDD573D70",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__security__9E2397E0D77F092D",
                table: "security_logs",
                column: "log_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__reports__779B7C587B55F42C",
                table: "reports",
                column: "report_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__notifica__E059842FC3175EFE",
                table: "notifications",
                column: "notification_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__labs__66DE64DB348D68A0",
                table: "labs",
                column: "lab_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__lab_zone__80B401DFC554C85A",
                table: "lab_zones",
                column: "zone_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__lab_even__2370F727FFDCB3A2",
                table: "lab_events",
                column: "event_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__event_pa__C8EB14572030C1E5",
                table: "event_participants",
                columns: new[] { "event_id", "user_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK__activity__D2470C87450D1758",
                table: "activity_types",
                column: "activity_type_id");

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    department_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__departme__C223242226FD0D59", x => x.department_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_department_id",
                table: "users",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_labs_department_id",
                table: "labs",
                column: "department_id");

            migrationBuilder.AddForeignKey(
                name: "FK__event_par__event__4D94879B",
                table: "event_participants",
                column: "event_id",
                principalTable: "lab_events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK__event_par__user___4E88ABD4",
                table: "event_participants",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__activ__49C3F6B7",
                table: "lab_events",
                column: "activity_type_id",
                principalTable: "activity_types",
                principalColumn: "activity_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__lab_i__47DBAE45",
                table: "lab_events",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "lab_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__organ__4AB81AF0",
                table: "lab_events",
                column: "organizer_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_event__zone___48CFD27E",
                table: "lab_events",
                column: "zone_id",
                principalTable: "lab_zones",
                principalColumn: "zone_id");

            migrationBuilder.AddForeignKey(
                name: "FK__lab_zones__lab_i__4222D4EF",
                table: "lab_zones",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "lab_id");

            migrationBuilder.AddForeignKey(
                name: "FK__labs__department__3E52440B",
                table: "labs",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "department_id");

            migrationBuilder.AddForeignKey(
                name: "FK__labs__manager_id__3F466844",
                table: "labs",
                column: "manager_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__notificat__event__59063A47",
                table: "notifications",
                column: "event_id",
                principalTable: "lab_events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK__notificat__recip__5812160E",
                table: "notifications",
                column: "recipient_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__reports__generat__5CD6CB2B",
                table: "reports",
                column: "generated_by",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__reports__lab_id__5DCAEF64",
                table: "reports",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "lab_id");

            migrationBuilder.AddForeignKey(
                name: "FK__reports__zone_id__5EBF139D",
                table: "reports",
                column: "zone_id",
                principalTable: "lab_zones",
                principalColumn: "zone_id");

            migrationBuilder.AddForeignKey(
                name: "FK__security___event__52593CB8",
                table: "security_logs",
                column: "event_id",
                principalTable: "lab_events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK__security___secur__534D60F1",
                table: "security_logs",
                column: "security_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__users__departmen__3B75D760",
                table: "users",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "department_id");
        }
    }
}
