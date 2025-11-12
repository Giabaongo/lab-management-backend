using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentAccessControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('reports','generated_by') IS NOT NULL
BEGIN
    DECLARE @dropFkSql NVARCHAR(MAX) = N'';
    SELECT @dropFkSql = STRING_AGG('ALTER TABLE [reports] DROP CONSTRAINT [' + fk.name + '];', '')
    FROM sys.foreign_keys fk
    WHERE fk.parent_object_id = OBJECT_ID(N'reports')
      AND fk.referenced_object_id = OBJECT_ID(N'users')
      AND fk.name LIKE 'FK__reports__generat__%';
    IF (@dropFkSql IS NOT NULL AND LEN(@dropFkSql) > 0)
        EXEC sp_executesql @dropFkSql;

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_reports_generated_by' AND object_id = OBJECT_ID(N'reports'))
        DROP INDEX [IX_reports_generated_by] ON [reports];

    ALTER TABLE [reports] DROP COLUMN [generated_by];
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('reports','photo_url') IS NULL
BEGIN
    ALTER TABLE [reports] ADD [photo_url] varchar(1000) NULL;
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('reports','user_id') IS NULL
BEGIN
    ALTER TABLE [reports] ADD [user_id] int NULL;
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_reports_user_id' AND object_id = OBJECT_ID(N'reports'))
BEGIN
    CREATE INDEX [IX_reports_user_id] ON [reports]([user_id]);
END");

            migrationBuilder.Sql(@"
DECLARE @constraintName NVARCHAR(128);
SELECT @constraintName = df.name
FROM sys.default_constraints df
JOIN sys.columns c ON c.default_object_id = df.object_id
WHERE df.parent_object_id = OBJECT_ID(N'notifications') AND c.name = 'is_read';
IF @constraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [notifications] DROP CONSTRAINT [' + @constraintName + ']');
END");

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    department_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_public = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__departme__C2232422", x => x.department_id);
                });

            migrationBuilder.InsertData(
                table: "departments",
                columns: new[] { "department_id", "name", "description", "is_public" },
                values: new object[] { 1, "General", "Default department for existing labs", true });

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "labs",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "user_departments",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    department_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "(sysdatetime())")
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

            migrationBuilder.CreateIndex(
                name: "IX_labs_department_id",
                table: "labs",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_departments_department_id",
                table: "user_departments",
                column: "department_id");

            migrationBuilder.AddForeignKey(
                name: "FK__labs__departmen__6EC0713C",
                table: "labs",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "department_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__labs__departmen__6EC0713C",
                table: "labs");

            migrationBuilder.DropTable(
                name: "user_departments");

            migrationBuilder.DropIndex(
                name: "IX_labs_department_id",
                table: "labs");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "labs");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_reports_user_id' AND object_id = OBJECT_ID(N'reports'))
BEGIN
    DROP INDEX [IX_reports_user_id] ON [reports];
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('reports','user_id') IS NOT NULL
BEGIN
    ALTER TABLE [reports] DROP COLUMN [user_id];
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('reports','photo_url') IS NOT NULL
BEGIN
    ALTER TABLE [reports] DROP COLUMN [photo_url];
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('reports','generated_by') IS NULL
BEGIN
    ALTER TABLE [reports] ADD [generated_by] int NOT NULL DEFAULT(0);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_reports_generated_by' AND object_id = OBJECT_ID(N'reports'))
BEGIN
    CREATE INDEX [IX_reports_generated_by] ON [reports]([generated_by]);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK__reports__generat__16CE6296' AND parent_object_id = OBJECT_ID(N'reports'))
BEGIN
    ALTER TABLE [reports]  WITH NOCHECK ADD  CONSTRAINT [FK__reports__generat__16CE6296] FOREIGN KEY([generated_by])
    REFERENCES [users] ([user_id]);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints df
    JOIN sys.columns c ON c.default_object_id = df.object_id
    WHERE df.parent_object_id = OBJECT_ID(N'notifications')
      AND c.name = 'is_read')
BEGIN
    ALTER TABLE [notifications] ADD CONSTRAINT [DF_notifications_is_read] DEFAULT(0) FOR [is_read];
END");
        }
    }
}
