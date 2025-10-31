/*******************************************************************************
 * LAB MANAGEMENT DATABASE SCHEMA - VERSION 2.0 (OPTIMIZED)
 * Generated: 2025-10-23
 * Database: LabManagementDB
 * Target: SQL Server 2016+
 * 
 * DESCRIPTION:
 * This is an optimized version of the database schema with performance 
 * improvements and best practices applied.
 * 
 * KEY IMPROVEMENTS FROM v1.0:
 * - TEXT → VARCHAR(MAX) for better indexing and performance
 * - DATETIME → DATETIME2(3) for millisecond precision and storage efficiency
 * - Renamed reserved word columns (timestamp → logged_at, action → action_type)
 * - Added CHECK constraints for data validation
 * - Added ROWVERSION for optimistic concurrency control
 * - Added proper DEFAULT constraints
 * 
 * MIGRATION STRATEGY:
 * This script is IDEMPOTENT - safe to run multiple times.
 * It checks __EFMigrationsHistory to skip already-applied migrations.
 * 
 * USAGE:
 * New installation:
 *   sqlcmd -S server -U user -P password -d LabManagementDB -i schema_v2_optimized.sql
 * 
 * Upgrade from v1.0:
 *   dotnet ef database update --project LabManagement.DAL --startup-project LabManagement.API
 * 
 * BREAKING CHANGES:
 * - SecurityLog.Timestamp → SecurityLog.LoggedAt
 * - SecurityLog.Action → SecurityLog.ActionType
 * - Added RowVersion to Booking and LabEvent models
 * 
 * See SCHEMA_VERSIONS.md for full changelog and migration guide.
 *******************************************************************************/

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE TABLE [activity_types] (
        [activity_type_id] int NOT NULL IDENTITY,
        [name] varchar(100) NOT NULL,
        [description] text NULL,
        CONSTRAINT [PK__activity__D2470C87450D1758] PRIMARY KEY ([activity_type_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE TABLE [departments] (
        [department_id] int NOT NULL IDENTITY,
        [name] varchar(100) NOT NULL,
        [description] text NULL,
        CONSTRAINT [PK__departme__C223242226FD0D59] PRIMARY KEY ([department_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE TABLE [users] (
        [user_id] int NOT NULL IDENTITY,
        [name] varchar(100) NOT NULL,
        [email] varchar(100) NOT NULL,
        [password_hash] varchar(128) NOT NULL,
        [role] varchar(20) NOT NULL,
        [department_id] int NOT NULL,
        [created_at] datetime NOT NULL DEFAULT ((getdate())),
        CONSTRAINT [PK__users__B9BE370FDD573D70] PRIMARY KEY ([user_id]),
        CONSTRAINT [FK__users__departmen__3B75D760] FOREIGN KEY ([department_id]) REFERENCES [departments] ([department_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE TABLE [labs] (
        [lab_id] int NOT NULL IDENTITY,
        [name] varchar(100) NOT NULL,
        [department_id] int NOT NULL,
        [manager_id] int NOT NULL,
        [location] varchar(255) NULL,
        [description] text NULL,
        CONSTRAINT [PK__labs__66DE64DB348D68A0] PRIMARY KEY ([lab_id]),
        CONSTRAINT [FK__labs__department__3E52440B] FOREIGN KEY ([department_id]) REFERENCES [departments] ([department_id]),
        CONSTRAINT [FK__labs__manager_id__3F466844] FOREIGN KEY ([manager_id]) REFERENCES [users] ([user_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE TABLE [lab_zones] (
        [zone_id] int NOT NULL IDENTITY,
        [lab_id] int NOT NULL,
        [name] varchar(100) NOT NULL,
        [description] text NULL,
        CONSTRAINT [PK__lab_zone__80B401DFC554C85A] PRIMARY KEY ([zone_id]),
        CONSTRAINT [FK__lab_zones__lab_i__4222D4EF] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE TABLE [lab_events] (
        [event_id] int NOT NULL IDENTITY,
        [lab_id] int NOT NULL,
        [zone_id] int NOT NULL,
        [activity_type_id] int NOT NULL,
        [organizer_id] int NOT NULL,
        [title] varchar(200) NOT NULL,
        [description] text NULL,
        [start_time] datetime NOT NULL,
        [end_time] datetime NOT NULL,
        [status] varchar(20) NOT NULL,
        [created_at] datetime NOT NULL DEFAULT ((getdate())),
        CONSTRAINT [PK__lab_even__2370F727FFDCB3A2] PRIMARY KEY ([event_id]),
        CONSTRAINT [FK__lab_event__activ__49C3F6B7] FOREIGN KEY ([activity_type_id]) REFERENCES [activity_types] ([activity_type_id]),
        CONSTRAINT [FK__lab_event__lab_i__47DBAE45] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]),
        CONSTRAINT [FK__lab_event__organ__4AB81AF0] FOREIGN KEY ([organizer_id]) REFERENCES [users] ([user_id]),
        CONSTRAINT [FK__lab_event__zone___48CFD27E] FOREIGN KEY ([zone_id]) REFERENCES [lab_zones] ([zone_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE TABLE [reports] (
        [report_id] int NOT NULL IDENTITY,
        [zone_id] int NULL,
        [lab_id] int NULL,
        [generated_by] int NOT NULL,
        [report_type] varchar(100) NOT NULL,
        [content] text NULL,
        [generated_at] datetime NOT NULL DEFAULT ((getdate())),
        CONSTRAINT [PK__reports__779B7C587B55F42C] PRIMARY KEY ([report_id]),
        CONSTRAINT [FK__reports__generat__5CD6CB2B] FOREIGN KEY ([generated_by]) REFERENCES [users] ([user_id]),
        CONSTRAINT [FK__reports__lab_id__5DCAEF64] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]),
        CONSTRAINT [FK__reports__zone_id__5EBF139D] FOREIGN KEY ([zone_id]) REFERENCES [lab_zones] ([zone_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE TABLE [event_participants] (
        [event_id] int NOT NULL,
        [user_id] int NOT NULL,
        [role] varchar(20) NOT NULL,
        CONSTRAINT [PK__event_pa__C8EB14572030C1E5] PRIMARY KEY ([event_id], [user_id]),
        CONSTRAINT [FK__event_par__event__4D94879B] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]),
        CONSTRAINT [FK__event_par__user___4E88ABD4] FOREIGN KEY ([user_id]) REFERENCES [users] ([user_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE TABLE [notifications] (
        [notification_id] int NOT NULL IDENTITY,
        [recipient_id] int NOT NULL,
        [event_id] int NOT NULL,
        [message] text NOT NULL,
        [sent_at] datetime NOT NULL DEFAULT ((getdate())),
        [is_read] bit NOT NULL,
        CONSTRAINT [PK__notifica__E059842FC3175EFE] PRIMARY KEY ([notification_id]),
        CONSTRAINT [FK__notificat__event__59063A47] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]),
        CONSTRAINT [FK__notificat__recip__5812160E] FOREIGN KEY ([recipient_id]) REFERENCES [users] ([user_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE TABLE [security_logs] (
        [log_id] int NOT NULL IDENTITY,
        [event_id] int NOT NULL,
        [security_id] int NOT NULL,
        [action] varchar(20) NOT NULL,
        [timestamp] datetime NOT NULL DEFAULT ((getdate())),
        [photo_url] varchar(255) NULL,
        [notes] text NULL,
        CONSTRAINT [PK__security__9E2397E0D77F092D] PRIMARY KEY ([log_id]),
        CONSTRAINT [FK__security___event__52593CB8] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]),
        CONSTRAINT [FK__security___secur__534D60F1] FOREIGN KEY ([security_id]) REFERENCES [users] ([user_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_event_participants_user_id] ON [event_participants] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_lab_events_activity_type_id] ON [lab_events] ([activity_type_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_lab_events_lab_id] ON [lab_events] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_lab_events_organizer_id] ON [lab_events] ([organizer_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_lab_events_zone_id] ON [lab_events] ([zone_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_lab_zones_lab_id] ON [lab_zones] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_labs_department_id] ON [labs] ([department_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_labs_manager_id] ON [labs] ([manager_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_notifications_event_id] ON [notifications] ([event_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_notifications_recipient_id] ON [notifications] ([recipient_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_reports_generated_by] ON [reports] ([generated_by]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_reports_lab_id] ON [reports] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_reports_zone_id] ON [reports] ([zone_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_security_logs_event_id] ON [security_logs] ([event_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_security_logs_security_id] ON [security_logs] ([security_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_users_department_id] ON [users] ([department_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UQ__users__AB6E61642BBB0620] ON [users] ([email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250923111522_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250923111522_InitialCreate', N'8.0.2');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [event_participants] DROP CONSTRAINT [FK__event_par__event__4D94879B];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [event_participants] DROP CONSTRAINT [FK__event_par__user___4E88ABD4];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_events] DROP CONSTRAINT [FK__lab_event__activ__49C3F6B7];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_events] DROP CONSTRAINT [FK__lab_event__lab_i__47DBAE45];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_events] DROP CONSTRAINT [FK__lab_event__organ__4AB81AF0];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_events] DROP CONSTRAINT [FK__lab_event__zone___48CFD27E];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_zones] DROP CONSTRAINT [FK__lab_zones__lab_i__4222D4EF];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [labs] DROP CONSTRAINT [FK__labs__department__3E52440B];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [labs] DROP CONSTRAINT [FK__labs__manager_id__3F466844];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [notifications] DROP CONSTRAINT [FK__notificat__event__59063A47];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [notifications] DROP CONSTRAINT [FK__notificat__recip__5812160E];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [reports] DROP CONSTRAINT [FK__reports__generat__5CD6CB2B];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [reports] DROP CONSTRAINT [FK__reports__lab_id__5DCAEF64];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [reports] DROP CONSTRAINT [FK__reports__zone_id__5EBF139D];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [security_logs] DROP CONSTRAINT [FK__security___event__52593CB8];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [security_logs] DROP CONSTRAINT [FK__security___secur__534D60F1];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [users] DROP CONSTRAINT [FK__users__departmen__3B75D760];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    DROP TABLE [departments];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [users] DROP CONSTRAINT [PK__users__B9BE370FDD573D70];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    DROP INDEX [IX_users_department_id] ON [users];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [security_logs] DROP CONSTRAINT [PK__security__9E2397E0D77F092D];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [reports] DROP CONSTRAINT [PK__reports__779B7C587B55F42C];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [notifications] DROP CONSTRAINT [PK__notifica__E059842FC3175EFE];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [labs] DROP CONSTRAINT [PK__labs__66DE64DB348D68A0];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    DROP INDEX [IX_labs_department_id] ON [labs];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_zones] DROP CONSTRAINT [PK__lab_zone__80B401DFC554C85A];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_events] DROP CONSTRAINT [PK__lab_even__2370F727FFDCB3A2];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [event_participants] DROP CONSTRAINT [PK__event_pa__C8EB14572030C1E5];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [activity_types] DROP CONSTRAINT [PK__activity__D2470C87450D1758];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'department_id');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [users] DROP COLUMN [department_id];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[labs]') AND [c].[name] = N'department_id');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [labs] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [labs] DROP COLUMN [department_id];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    EXEC sp_rename N'[users].[UQ__users__AB6E61642BBB0620]', N'UQ__users__AB6E6164DB54CFBB', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'role');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [users] ALTER COLUMN [role] decimal(2,0) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[event_participants]') AND [c].[name] = N'role');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [event_participants] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [event_participants] ALTER COLUMN [role] decimal(2,0) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [users] ADD CONSTRAINT [PK__users__B9BE370F20E930E3] PRIMARY KEY ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [security_logs] ADD CONSTRAINT [PK__security__9E2397E0DFE7202D] PRIMARY KEY ([log_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [reports] ADD CONSTRAINT [PK__reports__779B7C58021353BD] PRIMARY KEY ([report_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [notifications] ADD CONSTRAINT [PK__notifica__E059842F54DF105B] PRIMARY KEY ([notification_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [labs] ADD CONSTRAINT [PK__labs__66DE64DB63EAC51C] PRIMARY KEY ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_zones] ADD CONSTRAINT [PK__lab_zone__80B401DF8DC1AAD7] PRIMARY KEY ([zone_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_events] ADD CONSTRAINT [PK__lab_even__2370F7270A85F029] PRIMARY KEY ([event_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [event_participants] ADD CONSTRAINT [PK__event_pa__C8EB1457151547CC] PRIMARY KEY ([event_id], [user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [activity_types] ADD CONSTRAINT [PK__activity__D2470C87E6BFD9E2] PRIMARY KEY ([activity_type_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [event_participants] ADD CONSTRAINT [FK__event_par__event__151B244E] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [event_participants] ADD CONSTRAINT [FK__event_par__user___160F4887] FOREIGN KEY ([user_id]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_events] ADD CONSTRAINT [FK__lab_event__activ__114A936A] FOREIGN KEY ([activity_type_id]) REFERENCES [activity_types] ([activity_type_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_events] ADD CONSTRAINT [FK__lab_event__lab_i__0F624AF8] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_events] ADD CONSTRAINT [FK__lab_event__organ__123EB7A3] FOREIGN KEY ([organizer_id]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_events] ADD CONSTRAINT [FK__lab_event__zone___10566F31] FOREIGN KEY ([zone_id]) REFERENCES [lab_zones] ([zone_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [lab_zones] ADD CONSTRAINT [FK__lab_zones__lab_i__09A971A2] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [labs] ADD CONSTRAINT [FK__labs__manager_id__06CD04F7] FOREIGN KEY ([manager_id]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [notifications] ADD CONSTRAINT [FK__notificat__event__208CD6FA] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [notifications] ADD CONSTRAINT [FK__notificat__recip__1F98B2C1] FOREIGN KEY ([recipient_id]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [reports] ADD CONSTRAINT [FK__reports__generat__245D67DE] FOREIGN KEY ([generated_by]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [reports] ADD CONSTRAINT [FK__reports__lab_id__25518C17] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [reports] ADD CONSTRAINT [FK__reports__zone_id__2645B050] FOREIGN KEY ([zone_id]) REFERENCES [lab_zones] ([zone_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [security_logs] ADD CONSTRAINT [FK__security___event__19DFD96B] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    ALTER TABLE [security_logs] ADD CONSTRAINT [FK__security___secur__1AD3FDA4] FOREIGN KEY ([security_id]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250929040237_Remove_department_table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250929040237_Remove_department_table', N'8.0.2');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [event_participants] DROP CONSTRAINT [FK__event_par__event__151B244E];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [event_participants] DROP CONSTRAINT [FK__event_par__user___160F4887];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_events] DROP CONSTRAINT [FK__lab_event__activ__114A936A];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_events] DROP CONSTRAINT [FK__lab_event__lab_i__0F624AF8];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_events] DROP CONSTRAINT [FK__lab_event__organ__123EB7A3];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_events] DROP CONSTRAINT [FK__lab_event__zone___10566F31];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_zones] DROP CONSTRAINT [FK__lab_zones__lab_i__09A971A2];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [labs] DROP CONSTRAINT [FK__labs__manager_id__06CD04F7];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [notifications] DROP CONSTRAINT [FK__notificat__event__208CD6FA];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [notifications] DROP CONSTRAINT [FK__notificat__recip__1F98B2C1];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [reports] DROP CONSTRAINT [FK__reports__generat__245D67DE];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [reports] DROP CONSTRAINT [FK__reports__lab_id__25518C17];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [reports] DROP CONSTRAINT [FK__reports__zone_id__2645B050];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [security_logs] DROP CONSTRAINT [FK__security___event__19DFD96B];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [security_logs] DROP CONSTRAINT [FK__security___secur__1AD3FDA4];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [users] DROP CONSTRAINT [PK__users__B9BE370F20E930E3];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [security_logs] DROP CONSTRAINT [PK__security__9E2397E0DFE7202D];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [reports] DROP CONSTRAINT [PK__reports__779B7C58021353BD];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [notifications] DROP CONSTRAINT [PK__notifica__E059842F54DF105B];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [labs] DROP CONSTRAINT [PK__labs__66DE64DB63EAC51C];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_zones] DROP CONSTRAINT [PK__lab_zone__80B401DF8DC1AAD7];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_events] DROP CONSTRAINT [PK__lab_even__2370F7270A85F029];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [event_participants] DROP CONSTRAINT [PK__event_pa__C8EB1457151547CC];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [activity_types] DROP CONSTRAINT [PK__activity__D2470C87E6BFD9E2];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    EXEC sp_rename N'[users].[UQ__users__AB6E6164DB54CFBB]', N'UQ__users__AB6E6164598EED90', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[security_logs]') AND [c].[name] = N'action');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [security_logs] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [security_logs] ALTER COLUMN [action] decimal(2,0) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[lab_events]') AND [c].[name] = N'status');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [lab_events] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [lab_events] ALTER COLUMN [status] decimal(2,0) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [users] ADD CONSTRAINT [PK__users__B9BE370FBC272336] PRIMARY KEY ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [security_logs] ADD CONSTRAINT [PK__security__9E2397E0366827BF] PRIMARY KEY ([log_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [reports] ADD CONSTRAINT [PK__reports__779B7C581E1EA1F5] PRIMARY KEY ([report_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [notifications] ADD CONSTRAINT [PK__notifica__E059842F312DB8D7] PRIMARY KEY ([notification_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [labs] ADD CONSTRAINT [PK__labs__66DE64DB381C94E8] PRIMARY KEY ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_zones] ADD CONSTRAINT [PK__lab_zone__80B401DFE18A342C] PRIMARY KEY ([zone_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_events] ADD CONSTRAINT [PK__lab_even__2370F7275CD0E23F] PRIMARY KEY ([event_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [event_participants] ADD CONSTRAINT [PK__event_pa__C8EB1457D657AA72] PRIMARY KEY ([event_id], [user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [activity_types] ADD CONSTRAINT [PK__activity__D2470C8792B1F20E] PRIMARY KEY ([activity_type_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    CREATE TABLE [bookings] (
        [booking_id] int NOT NULL IDENTITY,
        [user_id] int NOT NULL,
        [lab_id] int NOT NULL,
        [zone_id] int NOT NULL,
        [start_time] datetime NOT NULL,
        [end_time] datetime NOT NULL,
        [status] decimal(2,0) NOT NULL,
        [created_at] datetime NOT NULL DEFAULT ((getdate())),
        [notes] text NULL,
        CONSTRAINT [PK__bookings__5DE3A5B17836BDD3] PRIMARY KEY ([booking_id]),
        CONSTRAINT [FK__bookings__lab_id__7D0E9093] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]),
        CONSTRAINT [FK__bookings__user_i__7C1A6C5A] FOREIGN KEY ([user_id]) REFERENCES [users] ([user_id]),
        CONSTRAINT [FK__bookings__zone_i__7E02B4CC] FOREIGN KEY ([zone_id]) REFERENCES [lab_zones] ([zone_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    CREATE TABLE [equipment] (
        [equipment_id] int NOT NULL IDENTITY,
        [lab_id] int NOT NULL,
        [name] varchar(100) NOT NULL,
        [code] varchar(50) NOT NULL,
        [description] text NULL,
        [status] decimal(2,0) NOT NULL,
        CONSTRAINT [PK__equipmen__197068AFC616B45E] PRIMARY KEY ([equipment_id]),
        CONSTRAINT [FK__equipment__lab_i__7849DB76] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'activity_type_id', N'description', N'name') AND [object_id] = OBJECT_ID(N'[activity_types]'))
        SET IDENTITY_INSERT [activity_types] ON;
    EXEC(N'INSERT INTO [activity_types] ([activity_type_id], [description], [name])
    VALUES (1, ''Hands-on training session'', ''Workshop''),
    (2, ''Educational seminar or lecture'', ''Seminar''),
    (3, ''Research activity'', ''Research''),
    (4, ''Laboratory experiment'', ''Experiment''),
    (5, ''Group meeting or discussion'', ''Meeting'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'activity_type_id', N'description', N'name') AND [object_id] = OBJECT_ID(N'[activity_types]'))
        SET IDENTITY_INSERT [activity_types] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'user_id', N'created_at', N'email', N'name', N'password_hash', N'role') AND [object_id] = OBJECT_ID(N'[users]'))
        SET IDENTITY_INSERT [users] ON;
    EXEC(N'INSERT INTO [users] ([user_id], [created_at], [email], [name], [password_hash], [role])
    VALUES (1, ''2025-01-01T00:00:00.000'', ''admin@lab.com'', ''Admin User'', ''$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC'', 1.0),
    (2, ''2025-01-01T00:00:00.000'', ''schoolmanager@lab.com'', ''School Manager'', ''$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC'', 2.0),
    (3, ''2025-01-01T00:00:00.000'', ''manager@lab.com'', ''Lab Manager'', ''$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC'', 3.0),
    (4, ''2025-01-01T00:00:00.000'', ''security@lab.com'', ''Security Staff'', ''$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC'', 4.0),
    (5, ''2025-01-01T00:00:00.000'', ''student@lab.com'', ''Student User'', ''$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC'', 5.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'user_id', N'created_at', N'email', N'name', N'password_hash', N'role') AND [object_id] = OBJECT_ID(N'[users]'))
        SET IDENTITY_INSERT [users] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'lab_id', N'description', N'location', N'manager_id', N'name') AND [object_id] = OBJECT_ID(N'[labs]'))
        SET IDENTITY_INSERT [labs] ON;
    EXEC(N'INSERT INTO [labs] ([lab_id], [description], [location], [manager_id], [name])
    VALUES (1, ''Laboratory for biology experiments and research'', ''Building A - Floor 2'', 2, ''Biology Lab''),
    (2, ''Computer lab with 30 workstations'', ''Building B - Floor 3'', 2, ''Computer Lab''),
    (3, ''Chemistry laboratory with safety equipment'', ''Building A - Floor 1'', 3, ''Chemistry Lab'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'lab_id', N'description', N'location', N'manager_id', N'name') AND [object_id] = OBJECT_ID(N'[labs]'))
        SET IDENTITY_INSERT [labs] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'equipment_id', N'code', N'description', N'lab_id', N'name', N'status') AND [object_id] = OBJECT_ID(N'[equipment]'))
        SET IDENTITY_INSERT [equipment] ON;
    EXEC(N'INSERT INTO [equipment] ([equipment_id], [code], [description], [lab_id], [name], [status])
    VALUES (1, ''EQ-001'', ''Digital microscope with 1000x magnification'', 1, ''Microscope'', 1.0),
    (2, ''EQ-002'', ''High-speed centrifuge'', 1, ''Centrifuge'', 1.0),
    (3, ''EQ-003'', ''Workstation with development tools'', 2, ''Computer Station'', 1.0),
    (4, ''EQ-004'', ''Network server infrastructure'', 2, ''Server Rack'', 1.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'equipment_id', N'code', N'description', N'lab_id', N'name', N'status') AND [object_id] = OBJECT_ID(N'[equipment]'))
        SET IDENTITY_INSERT [equipment] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'zone_id', N'description', N'lab_id', N'name') AND [object_id] = OBJECT_ID(N'[lab_zones]'))
        SET IDENTITY_INSERT [lab_zones] ON;
    EXEC(N'INSERT INTO [lab_zones] ([zone_id], [description], [lab_id], [name])
    VALUES (1, ''Main experiment area'', 1, ''Zone A''),
    (2, ''Storage and preparation area'', 1, ''Zone B''),
    (3, ''Development stations'', 2, ''Zone A''),
    (4, ''Server room'', 2, ''Zone B''),
    (5, ''Chemical storage'', 3, ''Zone A'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'zone_id', N'description', N'lab_id', N'name') AND [object_id] = OBJECT_ID(N'[lab_zones]'))
        SET IDENTITY_INSERT [lab_zones] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    CREATE INDEX [IX_bookings_lab_id] ON [bookings] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    CREATE INDEX [IX_bookings_user_id] ON [bookings] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    CREATE INDEX [IX_bookings_zone_id] ON [bookings] ([zone_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    CREATE INDEX [IX_equipment_lab_id] ON [equipment] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    CREATE UNIQUE INDEX [UQ__equipmen__357D4CF958F5304E] ON [equipment] ([code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [event_participants] ADD CONSTRAINT [FK__event_par__event__078C1F06] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [event_participants] ADD CONSTRAINT [FK__event_par__user___0880433F] FOREIGN KEY ([user_id]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_events] ADD CONSTRAINT [FK__lab_event__activ__03BB8E22] FOREIGN KEY ([activity_type_id]) REFERENCES [activity_types] ([activity_type_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_events] ADD CONSTRAINT [FK__lab_event__lab_i__01D345B0] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_events] ADD CONSTRAINT [FK__lab_event__organ__04AFB25B] FOREIGN KEY ([organizer_id]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_events] ADD CONSTRAINT [FK__lab_event__zone___02C769E9] FOREIGN KEY ([zone_id]) REFERENCES [lab_zones] ([zone_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [lab_zones] ADD CONSTRAINT [FK__lab_zones__lab_i__72910220] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [labs] ADD CONSTRAINT [FK__labs__manager_id__6FB49575] FOREIGN KEY ([manager_id]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [notifications] ADD CONSTRAINT [FK__notificat__event__12FDD1B2] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [notifications] ADD CONSTRAINT [FK__notificat__recip__1209AD79] FOREIGN KEY ([recipient_id]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [reports] ADD CONSTRAINT [FK__reports__generat__16CE6296] FOREIGN KEY ([generated_by]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [reports] ADD CONSTRAINT [FK__reports__lab_id__17C286CF] FOREIGN KEY ([lab_id]) REFERENCES [labs] ([lab_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [reports] ADD CONSTRAINT [FK__reports__zone_id__18B6AB08] FOREIGN KEY ([zone_id]) REFERENCES [lab_zones] ([zone_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [security_logs] ADD CONSTRAINT [FK__security___event__0C50D423] FOREIGN KEY ([event_id]) REFERENCES [lab_events] ([event_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    ALTER TABLE [security_logs] ADD CONSTRAINT [FK__security___secur__0D44F85C] FOREIGN KEY ([security_id]) REFERENCES [users] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006142242_Add_equiq_and_booking_table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251006142242_Add_equiq_and_booking_table', N'8.0.2');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251013163951_AlignUserRoleEnum'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'role');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [users] ALTER COLUMN [role] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251013163951_AlignUserRoleEnum'
)
BEGIN
    EXEC(N'UPDATE [users] SET [role] = 0
    WHERE [user_id] = 1;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251013163951_AlignUserRoleEnum'
)
BEGIN
    EXEC(N'UPDATE [users] SET [role] = 1
    WHERE [user_id] = 2;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251013163951_AlignUserRoleEnum'
)
BEGIN
    EXEC(N'UPDATE [users] SET [role] = 2
    WHERE [user_id] = 3;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251013163951_AlignUserRoleEnum'
)
BEGIN
    EXEC(N'UPDATE [users] SET [role] = 3
    WHERE [user_id] = 4;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251013163951_AlignUserRoleEnum'
)
BEGIN
    EXEC(N'UPDATE [users] SET [email] = ''member@lab.com'', [name] = ''Member'', [role] = 4
    WHERE [user_id] = 5;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251013163951_AlignUserRoleEnum'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251013163951_AlignUserRoleEnum', N'8.0.2');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251020031626_ConvertAllEnumColumnsToInt'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[security_logs]') AND [c].[name] = N'action');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [security_logs] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [security_logs] ALTER COLUMN [action] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251020031626_ConvertAllEnumColumnsToInt'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[lab_events]') AND [c].[name] = N'status');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [lab_events] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [lab_events] ALTER COLUMN [status] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251020031626_ConvertAllEnumColumnsToInt'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[event_participants]') AND [c].[name] = N'role');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [event_participants] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [event_participants] ALTER COLUMN [role] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251020031626_ConvertAllEnumColumnsToInt'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[equipment]') AND [c].[name] = N'status');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [equipment] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [equipment] ALTER COLUMN [status] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251020031626_ConvertAllEnumColumnsToInt'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bookings]') AND [c].[name] = N'status');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [bookings] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [bookings] ALTER COLUMN [status] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251020031626_ConvertAllEnumColumnsToInt'
)
BEGIN
    EXEC(N'UPDATE [equipment] SET [status] = 1
    WHERE [equipment_id] = 1;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251020031626_ConvertAllEnumColumnsToInt'
)
BEGIN
    EXEC(N'UPDATE [equipment] SET [status] = 1
    WHERE [equipment_id] = 2;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251020031626_ConvertAllEnumColumnsToInt'
)
BEGIN
    EXEC(N'UPDATE [equipment] SET [status] = 1
    WHERE [equipment_id] = 3;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251020031626_ConvertAllEnumColumnsToInt'
)
BEGIN
    EXEC(N'UPDATE [equipment] SET [status] = 1
    WHERE [equipment_id] = 4;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251020031626_ConvertAllEnumColumnsToInt'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251020031626_ConvertAllEnumColumnsToInt', N'8.0.2');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[security_logs]') AND [c].[name] = N'timestamp');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [security_logs] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [security_logs] DROP COLUMN [timestamp];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    EXEC sp_rename N'[security_logs].[action]', N'action_type', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'created_at');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [users] ALTER COLUMN [created_at] datetime2(3) NOT NULL;
    ALTER TABLE [users] ADD DEFAULT ((sysdatetime())) FOR [created_at];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[security_logs]') AND [c].[name] = N'notes');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [security_logs] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [security_logs] ALTER COLUMN [notes] varchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    ALTER TABLE [security_logs] ADD [logged_at] datetime2(3) NOT NULL DEFAULT ((sysdatetime()));
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    ALTER TABLE [security_logs] ADD [row_version] rowversion NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[reports]') AND [c].[name] = N'generated_at');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [reports] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [reports] ALTER COLUMN [generated_at] datetime2(3) NOT NULL;
    ALTER TABLE [reports] ADD DEFAULT ((sysdatetime())) FOR [generated_at];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[reports]') AND [c].[name] = N'content');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [reports] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [reports] ALTER COLUMN [content] varchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[notifications]') AND [c].[name] = N'sent_at');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [notifications] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [notifications] ALTER COLUMN [sent_at] datetime2(3) NOT NULL;
    ALTER TABLE [notifications] ADD DEFAULT ((sysdatetime())) FOR [sent_at];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[notifications]') AND [c].[name] = N'message');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [notifications] DROP CONSTRAINT [' + @var18 + '];');
    ALTER TABLE [notifications] ALTER COLUMN [message] varchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[notifications]') AND [c].[name] = N'is_read');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [notifications] DROP CONSTRAINT [' + @var19 + '];');
    ALTER TABLE [notifications] ADD DEFAULT CAST(0 AS bit) FOR [is_read];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[labs]') AND [c].[name] = N'description');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [labs] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [labs] ALTER COLUMN [description] varchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[lab_zones]') AND [c].[name] = N'description');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [lab_zones] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [lab_zones] ALTER COLUMN [description] varchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[lab_events]') AND [c].[name] = N'start_time');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [lab_events] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [lab_events] ALTER COLUMN [start_time] datetime2(3) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[lab_events]') AND [c].[name] = N'end_time');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [lab_events] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [lab_events] ALTER COLUMN [end_time] datetime2(3) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[lab_events]') AND [c].[name] = N'description');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [lab_events] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [lab_events] ALTER COLUMN [description] varchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var25 sysname;
    SELECT @var25 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[lab_events]') AND [c].[name] = N'created_at');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [lab_events] DROP CONSTRAINT [' + @var25 + '];');
    ALTER TABLE [lab_events] ALTER COLUMN [created_at] datetime2(3) NOT NULL;
    ALTER TABLE [lab_events] ADD DEFAULT ((sysdatetime())) FOR [created_at];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    ALTER TABLE [lab_events] ADD [row_version] rowversion NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var26 sysname;
    SELECT @var26 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[equipment]') AND [c].[name] = N'description');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [equipment] DROP CONSTRAINT [' + @var26 + '];');
    ALTER TABLE [equipment] ALTER COLUMN [description] varchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var27 sysname;
    SELECT @var27 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bookings]') AND [c].[name] = N'start_time');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [bookings] DROP CONSTRAINT [' + @var27 + '];');
    ALTER TABLE [bookings] ALTER COLUMN [start_time] datetime2(3) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var28 sysname;
    SELECT @var28 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bookings]') AND [c].[name] = N'notes');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [bookings] DROP CONSTRAINT [' + @var28 + '];');
    ALTER TABLE [bookings] ALTER COLUMN [notes] varchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var29 sysname;
    SELECT @var29 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bookings]') AND [c].[name] = N'end_time');
    IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [bookings] DROP CONSTRAINT [' + @var29 + '];');
    ALTER TABLE [bookings] ALTER COLUMN [end_time] datetime2(3) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var30 sysname;
    SELECT @var30 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bookings]') AND [c].[name] = N'created_at');
    IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [bookings] DROP CONSTRAINT [' + @var30 + '];');
    ALTER TABLE [bookings] ALTER COLUMN [created_at] datetime2(3) NOT NULL;
    ALTER TABLE [bookings] ADD DEFAULT ((sysdatetime())) FOR [created_at];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    ALTER TABLE [bookings] ADD [row_version] rowversion NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    DECLARE @var31 sysname;
    SELECT @var31 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[activity_types]') AND [c].[name] = N'description');
    IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [activity_types] DROP CONSTRAINT [' + @var31 + '];');
    ALTER TABLE [activity_types] ALTER COLUMN [description] varchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251023151315_UpdateSchemaToDateTime2AndVarcharMax', N'8.0.2');
END;
GO

COMMIT;
GO

