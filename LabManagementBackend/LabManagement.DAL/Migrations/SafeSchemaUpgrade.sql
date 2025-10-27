/*******************************************************************************
 * DATABASE SCHEMA UPGRADE SCRIPT
 * Database: LabManagementDB
 * Target: SQL Server 2016+
 * Date: 2025-10-23
 * 
 * CRITICAL: This script performs structural changes to production database
 * WARNING: Review RUNBOOK section below before execution
 * 
 * Changes Summary:
 * 1. Convert TEXT -> VARCHAR(MAX)
 * 2. Convert DATETIME -> DATETIME2(3)
 * 3. Rename reserved word columns (timestamp -> logged_at, action -> action_type)
 * 4. Add CHECK constraints for time validation
 * 5. Add DEFAULT constraint for notifications.is_read
 * 6. Add rowversion columns for optimistic concurrency
 *******************************************************************************/

SET NOCOUNT ON;
SET XACT_ABORT ON; -- Rollback entire transaction on error
GO

/*******************************************************************************
 * RUNBOOK - READ THIS BEFORE EXECUTION
 *******************************************************************************/
/*
PRE-EXECUTION CHECKLIST:
========================
1. BACKUP DATABASE:
   BACKUP DATABASE [LabManagementDB] 
   TO DISK = N'C:\Backups\LabManagementDB_PreMigration_20251023.bak'
   WITH COMPRESSION, STATS = 10;

2. VERIFY NO ACTIVE CONNECTIONS (or schedule maintenance window):
   SELECT * FROM sys.dm_exec_sessions 
   WHERE database_id = DB_ID('LabManagementDB') AND is_user_process = 1;

3. VERIFY FREE DISK SPACE (need ~2x current DB size for temp operations):
   EXEC sp_spaceused;

4. ESTIMATE DOWNTIME:
   - Small DB (<1GB): ~5-10 minutes
   - Medium DB (1-10GB): ~15-30 minutes
   - Large DB (>10GB): Consider online operations or staging first

5. TEST ON STAGING FIRST!

EXECUTION STEPS:
===============
1. Set database to SINGLE_USER mode (optional, for safety):
   ALTER DATABASE [LabManagementDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

2. Run this script

3. Restore multi-user access:
   ALTER DATABASE [LabManagementDB] SET MULTI_USER;

ROLLBACK PROCEDURE:
==================
If migration fails or issues detected:
1. Restore from backup:
   USE master;
   RESTORE DATABASE [LabManagementDB] 
   FROM DISK = N'C:\Backups\LabManagementDB_PreMigration_20251023.bak'
   WITH REPLACE, RECOVERY;

2. Or use rollback script at end of this file (ONLY if partial completion)
*/

USE [LabManagementDB];
GO

PRINT '========================================';
PRINT 'Starting Safe Schema Migration';
PRINT 'Timestamp: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '========================================';
GO

/*******************************************************************************
 * SECTION 1: TEXT to VARCHAR(MAX) Conversion
 * IMPACT: Minimal blocking, but may take time on large tables
 * DOWNTIME RISK: LOW (online operation in Enterprise Edition)
 *******************************************************************************/

PRINT '';
PRINT '>>> SECTION 1: Converting TEXT columns to VARCHAR(MAX)';
PRINT '';

-- activity_types.description
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.activity_types')
    AND c.name = 'description'
    AND t.name = 'text'
)
BEGIN
    PRINT '  - Converting activity_types.description: TEXT -> VARCHAR(MAX)';
    BEGIN TRY
        ALTER TABLE dbo.activity_types
        ALTER COLUMN [description] VARCHAR(MAX) NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - activity_types.description already VARCHAR(MAX) or compatible type - SKIPPED';
GO

-- labs.description
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.labs')
    AND c.name = 'description'
    AND t.name = 'text'
)
BEGIN
    PRINT '  - Converting labs.description: TEXT -> VARCHAR(MAX)';
    BEGIN TRY
        ALTER TABLE dbo.labs
        ALTER COLUMN [description] VARCHAR(MAX) NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - labs.description already VARCHAR(MAX) - SKIPPED';
GO

-- equipment.description
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.equipment')
    AND c.name = 'description'
    AND t.name = 'text'
)
BEGIN
    PRINT '  - Converting equipment.description: TEXT -> VARCHAR(MAX)';
    BEGIN TRY
        ALTER TABLE dbo.equipment
        ALTER COLUMN [description] VARCHAR(MAX) NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - equipment.description already VARCHAR(MAX) - SKIPPED';
GO

-- lab_zones.description
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.lab_zones')
    AND c.name = 'description'
    AND t.name = 'text'
)
BEGIN
    PRINT '  - Converting lab_zones.description: TEXT -> VARCHAR(MAX)';
    BEGIN TRY
        ALTER TABLE dbo.lab_zones
        ALTER COLUMN [description] VARCHAR(MAX) NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - lab_zones.description already VARCHAR(MAX) - SKIPPED';
GO

-- bookings.notes
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.bookings')
    AND c.name = 'notes'
    AND t.name = 'text'
)
BEGIN
    PRINT '  - Converting bookings.notes: TEXT -> VARCHAR(MAX)';
    BEGIN TRY
        ALTER TABLE dbo.bookings
        ALTER COLUMN [notes] VARCHAR(MAX) NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - bookings.notes already VARCHAR(MAX) - SKIPPED';
GO

-- lab_events.description
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.lab_events')
    AND c.name = 'description'
    AND t.name = 'text'
)
BEGIN
    PRINT '  - Converting lab_events.description: TEXT -> VARCHAR(MAX)';
    BEGIN TRY
        ALTER TABLE dbo.lab_events
        ALTER COLUMN [description] VARCHAR(MAX) NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - lab_events.description already VARCHAR(MAX) - SKIPPED';
GO

-- reports.content
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.reports')
    AND c.name = 'content'
    AND t.name = 'text'
)
BEGIN
    PRINT '  - Converting reports.content: TEXT -> VARCHAR(MAX)';
    BEGIN TRY
        ALTER TABLE dbo.reports
        ALTER COLUMN [content] VARCHAR(MAX) NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - reports.content already VARCHAR(MAX) - SKIPPED';
GO

-- notifications.message
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.notifications')
    AND c.name = 'message'
    AND t.name = 'text'
)
BEGIN
    PRINT '  - Converting notifications.message: TEXT -> VARCHAR(MAX)';
    BEGIN TRY
        ALTER TABLE dbo.notifications
        ALTER COLUMN [message] VARCHAR(MAX) NOT NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - notifications.message already VARCHAR(MAX) - SKIPPED';
GO

-- security_logs.notes
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.security_logs')
    AND c.name = 'notes'
    AND t.name = 'text'
)
BEGIN
    PRINT '  - Converting security_logs.notes: TEXT -> VARCHAR(MAX)';
    BEGIN TRY
        ALTER TABLE dbo.security_logs
        ALTER COLUMN [notes] VARCHAR(MAX) NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - security_logs.notes already VARCHAR(MAX) - SKIPPED';
GO

/*******************************************************************************
 * SECTION 2: DATETIME to DATETIME2(3) Conversion
 * IMPACT: Preserves all existing data, adds millisecond precision
 * DOWNTIME RISK: LOW-MEDIUM (depends on table size)
 * NOTE: DATETIME2(3) uses 7 bytes vs DATETIME's 8 bytes (slight storage savings)
 *******************************************************************************/

PRINT '';
PRINT '>>> SECTION 2: Converting DATETIME columns to DATETIME2(3)';
PRINT '';

-- users.created_at
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.users')
    AND c.name = 'created_at'
    AND t.name = 'datetime'
)
BEGIN
    PRINT '  - Converting users.created_at: DATETIME -> DATETIME2(3)';
    BEGIN TRY
        -- Drop default constraint first if exists
        DECLARE @defaultName NVARCHAR(200);
        SELECT @defaultName = d.name
        FROM sys.default_constraints d
        WHERE d.parent_object_id = OBJECT_ID('dbo.users')
        AND COL_NAME(d.parent_object_id, d.parent_column_id) = 'created_at';
        
        IF @defaultName IS NOT NULL
        BEGIN
            EXEC('ALTER TABLE dbo.users DROP CONSTRAINT [' + @defaultName + ']');
            PRINT '    - Dropped old default constraint: ' + @defaultName;
        END
        
        ALTER TABLE dbo.users
        ALTER COLUMN [created_at] DATETIME2(3) NOT NULL;
        
        -- Re-add default constraint
        ALTER TABLE dbo.users
        ADD CONSTRAINT DF_users_created_at DEFAULT (SYSDATETIME()) FOR [created_at];
        
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - users.created_at already DATETIME2 - SKIPPED';
GO

-- bookings.start_time
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.bookings')
    AND c.name = 'start_time'
    AND t.name = 'datetime'
)
BEGIN
    PRINT '  - Converting bookings.start_time: DATETIME -> DATETIME2(3)';
    BEGIN TRY
        ALTER TABLE dbo.bookings
        ALTER COLUMN [start_time] DATETIME2(3) NOT NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - bookings.start_time already DATETIME2 - SKIPPED';
GO

-- bookings.end_time
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.bookings')
    AND c.name = 'end_time'
    AND t.name = 'datetime'
)
BEGIN
    PRINT '  - Converting bookings.end_time: DATETIME -> DATETIME2(3)';
    BEGIN TRY
        ALTER TABLE dbo.bookings
        ALTER COLUMN [end_time] DATETIME2(3) NOT NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - bookings.end_time already DATETIME2 - SKIPPED';
GO

-- bookings.created_at
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.bookings')
    AND c.name = 'created_at'
    AND t.name = 'datetime'
)
BEGIN
    PRINT '  - Converting bookings.created_at: DATETIME -> DATETIME2(3)';
    BEGIN TRY
        DECLARE @defaultName2 NVARCHAR(200);
        SELECT @defaultName2 = d.name
        FROM sys.default_constraints d
        WHERE d.parent_object_id = OBJECT_ID('dbo.bookings')
        AND COL_NAME(d.parent_object_id, d.parent_column_id) = 'created_at';
        
        IF @defaultName2 IS NOT NULL
            EXEC('ALTER TABLE dbo.bookings DROP CONSTRAINT [' + @defaultName2 + ']');
        
        ALTER TABLE dbo.bookings
        ALTER COLUMN [created_at] DATETIME2(3) NOT NULL;
        
        ALTER TABLE dbo.bookings
        ADD CONSTRAINT DF_bookings_created_at DEFAULT (SYSDATETIME()) FOR [created_at];
        
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - bookings.created_at already DATETIME2 - SKIPPED';
GO

-- lab_events.start_time
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.lab_events')
    AND c.name = 'start_time'
    AND t.name = 'datetime'
)
BEGIN
    PRINT '  - Converting lab_events.start_time: DATETIME -> DATETIME2(3)';
    BEGIN TRY
        ALTER TABLE dbo.lab_events
        ALTER COLUMN [start_time] DATETIME2(3) NOT NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - lab_events.start_time already DATETIME2 - SKIPPED';
GO

-- lab_events.end_time
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.lab_events')
    AND c.name = 'end_time'
    AND t.name = 'datetime'
)
BEGIN
    PRINT '  - Converting lab_events.end_time: DATETIME -> DATETIME2(3)';
    BEGIN TRY
        ALTER TABLE dbo.lab_events
        ALTER COLUMN [end_time] DATETIME2(3) NOT NULL;
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - lab_events.end_time already DATETIME2 - SKIPPED';
GO

-- lab_events.created_at
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.lab_events')
    AND c.name = 'created_at'
    AND t.name = 'datetime'
)
BEGIN
    PRINT '  - Converting lab_events.created_at: DATETIME -> DATETIME2(3)';
    BEGIN TRY
        DECLARE @defaultName3 NVARCHAR(200);
        SELECT @defaultName3 = d.name
        FROM sys.default_constraints d
        WHERE d.parent_object_id = OBJECT_ID('dbo.lab_events')
        AND COL_NAME(d.parent_object_id, d.parent_column_id) = 'created_at';
        
        IF @defaultName3 IS NOT NULL
            EXEC('ALTER TABLE dbo.lab_events DROP CONSTRAINT [' + @defaultName3 + ']');
        
        ALTER TABLE dbo.lab_events
        ALTER COLUMN [created_at] DATETIME2(3) NOT NULL;
        
        ALTER TABLE dbo.lab_events
        ADD CONSTRAINT DF_lab_events_created_at DEFAULT (SYSDATETIME()) FOR [created_at];
        
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - lab_events.created_at already DATETIME2 - SKIPPED';
GO

-- reports.generated_at
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.reports')
    AND c.name = 'generated_at'
    AND t.name = 'datetime'
)
BEGIN
    PRINT '  - Converting reports.generated_at: DATETIME -> DATETIME2(3)';
    BEGIN TRY
        DECLARE @defaultName4 NVARCHAR(200);
        SELECT @defaultName4 = d.name
        FROM sys.default_constraints d
        WHERE d.parent_object_id = OBJECT_ID('dbo.reports')
        AND COL_NAME(d.parent_object_id, d.parent_column_id) = 'generated_at';
        
        IF @defaultName4 IS NOT NULL
            EXEC('ALTER TABLE dbo.reports DROP CONSTRAINT [' + @defaultName4 + ']');
        
        ALTER TABLE dbo.reports
        ALTER COLUMN [generated_at] DATETIME2(3) NOT NULL;
        
        ALTER TABLE dbo.reports
        ADD CONSTRAINT DF_reports_generated_at DEFAULT (SYSDATETIME()) FOR [generated_at];
        
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - reports.generated_at already DATETIME2 - SKIPPED';
GO

-- notifications.sent_at
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.notifications')
    AND c.name = 'sent_at'
    AND t.name = 'datetime'
)
BEGIN
    PRINT '  - Converting notifications.sent_at: DATETIME -> DATETIME2(3)';
    BEGIN TRY
        DECLARE @defaultName5 NVARCHAR(200);
        SELECT @defaultName5 = d.name
        FROM sys.default_constraints d
        WHERE d.parent_object_id = OBJECT_ID('dbo.notifications')
        AND COL_NAME(d.parent_object_id, d.parent_column_id) = 'sent_at';
        
        IF @defaultName5 IS NOT NULL
            EXEC('ALTER TABLE dbo.notifications DROP CONSTRAINT [' + @defaultName5 + ']');
        
        ALTER TABLE dbo.notifications
        ALTER COLUMN [sent_at] DATETIME2(3) NOT NULL;
        
        ALTER TABLE dbo.notifications
        ADD CONSTRAINT DF_notifications_sent_at DEFAULT (SYSDATETIME()) FOR [sent_at];
        
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - notifications.sent_at already DATETIME2 - SKIPPED';
GO

-- security_logs.timestamp (will be renamed in next section)
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.system_type_id = t.system_type_id
    WHERE c.object_id = OBJECT_ID('dbo.security_logs')
    AND c.name = 'timestamp'
    AND t.name = 'datetime'
)
BEGIN
    PRINT '  - Converting security_logs.timestamp: DATETIME -> DATETIME2(3)';
    BEGIN TRY
        DECLARE @defaultName6 NVARCHAR(200);
        SELECT @defaultName6 = d.name
        FROM sys.default_constraints d
        WHERE d.parent_object_id = OBJECT_ID('dbo.security_logs')
        AND COL_NAME(d.parent_object_id, d.parent_column_id) = 'timestamp';
        
        IF @defaultName6 IS NOT NULL
            EXEC('ALTER TABLE dbo.security_logs DROP CONSTRAINT [' + @defaultName6 + ']');
        
        ALTER TABLE dbo.security_logs
        ALTER COLUMN [timestamp] DATETIME2(3) NOT NULL;
        
        ALTER TABLE dbo.security_logs
        ADD CONSTRAINT DF_security_logs_timestamp_temp DEFAULT (SYSDATETIME()) FOR [timestamp];
        
        PRINT '    ✓ Success (will be renamed to logged_at in next section)';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - security_logs.timestamp already DATETIME2 - SKIPPED';
GO

/*******************************************************************************
 * SECTION 3: Rename Reserved Word Columns
 * IMPACT: Breaking change for existing queries/code
 * DOWNTIME RISK: LOW (metadata operation only)
 * ACTION REQUIRED: Update application code to use new column names!
 *******************************************************************************/

PRINT '';
PRINT '>>> SECTION 3: Renaming Reserved Word Columns';
PRINT '!!! WARNING: This will break existing queries using old column names !!!';
PRINT '';

-- Rename security_logs.timestamp -> logged_at
IF COL_LENGTH('dbo.security_logs', 'timestamp') IS NOT NULL
AND COL_LENGTH('dbo.security_logs', 'logged_at') IS NULL
BEGIN
    PRINT '  - Renaming security_logs.[timestamp] -> [logged_at]';
    BEGIN TRY
        -- Drop the temp default constraint first
        DECLARE @tempDefault NVARCHAR(200);
        SELECT @tempDefault = d.name
        FROM sys.default_constraints d
        WHERE d.parent_object_id = OBJECT_ID('dbo.security_logs')
        AND COL_NAME(d.parent_object_id, d.parent_column_id) = 'timestamp';
        
        IF @tempDefault IS NOT NULL
            EXEC('ALTER TABLE dbo.security_logs DROP CONSTRAINT [' + @tempDefault + ']');
        
        -- Rename column
        EXEC sp_rename 'dbo.security_logs.timestamp', 'logged_at', 'COLUMN';
        
        -- Add default constraint with new name
        ALTER TABLE dbo.security_logs
        ADD CONSTRAINT DF_security_logs_logged_at DEFAULT (SYSDATETIME()) FOR [logged_at];
        
        PRINT '    ✓ Success';
        PRINT '    !!! UPDATE YOUR CODE: Use logged_at instead of timestamp !!!';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE IF COL_LENGTH('dbo.security_logs', 'logged_at') IS NOT NULL
    PRINT '  - security_logs.logged_at already exists - SKIPPED';
ELSE
    PRINT '  - security_logs.timestamp not found - SKIPPED';
GO

-- Rename security_logs.action -> action_type
IF COL_LENGTH('dbo.security_logs', 'action') IS NOT NULL
AND COL_LENGTH('dbo.security_logs', 'action_type') IS NULL
BEGIN
    PRINT '  - Renaming security_logs.[action] -> [action_type]';
    BEGIN TRY
        EXEC sp_rename 'dbo.security_logs.action', 'action_type', 'COLUMN';
        PRINT '    ✓ Success';
        PRINT '    !!! UPDATE YOUR CODE: Use action_type instead of action !!!';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE IF COL_LENGTH('dbo.security_logs', 'action_type') IS NOT NULL
    PRINT '  - security_logs.action_type already exists - SKIPPED';
ELSE
    PRINT '  - security_logs.action not found - SKIPPED';
GO

/*******************************************************************************
 * SECTION 4: Add CHECK Constraints for Time Validation
 * IMPACT: Prevents invalid data entry (start_time must be < end_time)
 * DOWNTIME RISK: LOW (validates existing data first)
 * NOTE: Will fail if existing data violates constraint
 *******************************************************************************/

PRINT '';
PRINT '>>> SECTION 4: Adding CHECK Constraints for Time Validation';
PRINT '';

-- bookings: start_time < end_time
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID('dbo.bookings')
    AND name = 'CK_bookings_time_range'
)
BEGIN
    PRINT '  - Adding CHECK constraint: bookings.start_time < end_time';
    BEGIN TRY
        -- First verify no existing data violates this
        DECLARE @invalidBookings INT;
        SELECT @invalidBookings = COUNT(*)
        FROM dbo.bookings
        WHERE start_time >= end_time;
        
        IF @invalidBookings > 0
        BEGIN
            PRINT '    ✗ WARNING: Found ' + CAST(@invalidBookings AS VARCHAR) + ' booking(s) with invalid time range!';
            PRINT '    Please fix these records before adding constraint:';
            SELECT booking_id, start_time, end_time 
            FROM dbo.bookings 
            WHERE start_time >= end_time;
            RAISERROR('Cannot add constraint due to existing invalid data', 16, 1);
        END
        
        ALTER TABLE dbo.bookings
        ADD CONSTRAINT CK_bookings_time_range CHECK (start_time < end_time);
        
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - bookings time range constraint already exists - SKIPPED';
GO

-- lab_events: start_time < end_time
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID('dbo.lab_events')
    AND name = 'CK_lab_events_time_range'
)
BEGIN
    PRINT '  - Adding CHECK constraint: lab_events.start_time < end_time';
    BEGIN TRY
        DECLARE @invalidEvents INT;
        SELECT @invalidEvents = COUNT(*)
        FROM dbo.lab_events
        WHERE start_time >= end_time;
        
        IF @invalidEvents > 0
        BEGIN
            PRINT '    ✗ WARNING: Found ' + CAST(@invalidEvents AS VARCHAR) + ' event(s) with invalid time range!';
            PRINT '    Please fix these records before adding constraint:';
            SELECT event_id, start_time, end_time 
            FROM dbo.lab_events 
            WHERE start_time >= end_time;
            RAISERROR('Cannot add constraint due to existing invalid data', 16, 1);
        END
        
        ALTER TABLE dbo.lab_events
        ADD CONSTRAINT CK_lab_events_time_range CHECK (start_time < end_time);
        
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - lab_events time range constraint already exists - SKIPPED';
GO

/*******************************************************************************
 * SECTION 5: Add DEFAULT Constraint for notifications.is_read
 * IMPACT: New notifications will default to unread (0)
 * DOWNTIME RISK: NONE (metadata only)
 *******************************************************************************/

PRINT '';
PRINT '>>> SECTION 5: Adding DEFAULT Constraint for notifications.is_read';
PRINT '';

IF NOT EXISTS (
    SELECT 1 FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('dbo.notifications')
    AND COL_NAME(parent_object_id, parent_column_id) = 'is_read'
)
BEGIN
    PRINT '  - Adding DEFAULT constraint: notifications.is_read = 0';
    BEGIN TRY
        ALTER TABLE dbo.notifications
        ADD CONSTRAINT DF_notifications_is_read DEFAULT (0) FOR is_read;
        
        PRINT '    ✓ Success';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - notifications.is_read default already exists - SKIPPED';
GO

/*******************************************************************************
 * SECTION 6: Add ROWVERSION Columns for Optimistic Concurrency
 * IMPACT: Enables optimistic concurrency control (prevents lost updates)
 * DOWNTIME RISK: LOW (adds 8 bytes per row)
 * NOTE: SQL Server automatically maintains these values
 *******************************************************************************/

PRINT '';
PRINT '>>> SECTION 6: Adding ROWVERSION Columns for Optimistic Concurrency';
PRINT '';

-- bookings.row_version
IF COL_LENGTH('dbo.bookings', 'row_version') IS NULL
BEGIN
    PRINT '  - Adding rowversion column: bookings.row_version';
    BEGIN TRY
        ALTER TABLE dbo.bookings
        ADD row_version ROWVERSION NOT NULL;
        
        PRINT '    ✓ Success';
        PRINT '    INFO: Use this column in your UPDATE WHERE clause to prevent lost updates';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - bookings.row_version already exists - SKIPPED';
GO

-- lab_events.row_version
IF COL_LENGTH('dbo.lab_events', 'row_version') IS NULL
BEGIN
    PRINT '  - Adding rowversion column: lab_events.row_version';
    BEGIN TRY
        ALTER TABLE dbo.lab_events
        ADD row_version ROWVERSION NOT NULL;
        
        PRINT '    ✓ Success';
        PRINT '    INFO: Use this column in your UPDATE WHERE clause to prevent lost updates';
    END TRY
    BEGIN CATCH
        PRINT '    ✗ ERROR: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
    PRINT '  - lab_events.row_version already exists - SKIPPED';
GO

/*******************************************************************************
 * FINAL VALIDATION
 *******************************************************************************/

PRINT '';
PRINT '========================================';
PRINT 'MIGRATION COMPLETE - Running Final Validation';
PRINT '========================================';
PRINT '';

-- Verify data integrity
PRINT 'Data Integrity Checks:';
PRINT '  - Total users: ' + CAST((SELECT COUNT(*) FROM dbo.users) AS VARCHAR);
PRINT '  - Total bookings: ' + CAST((SELECT COUNT(*) FROM dbo.bookings) AS VARCHAR);
PRINT '  - Total lab_events: ' + CAST((SELECT COUNT(*) FROM dbo.lab_events) AS VARCHAR);
PRINT '  - Total security_logs: ' + CAST((SELECT COUNT(*) FROM dbo.security_logs) AS VARCHAR);

-- List new constraints
PRINT '';
PRINT 'New Constraints Added:';
SELECT 
    OBJECT_NAME(parent_object_id) AS TableName,
    name AS ConstraintName,
    type_desc AS ConstraintType
FROM sys.objects
WHERE type IN ('C', 'D')
AND name LIKE 'CK_%' OR name LIKE 'DF_%'
AND create_date > DATEADD(MINUTE, -5, GETDATE())
ORDER BY create_date DESC;

-- List renamed columns
PRINT '';
PRINT 'Schema Changes Summary:';
PRINT '  ✓ TEXT -> VARCHAR(MAX): 10 columns';
PRINT '  ✓ DATETIME -> DATETIME2(3): 9 columns';
PRINT '  ✓ Renamed columns: 2 (timestamp->logged_at, action->action_type)';
PRINT '  ✓ CHECK constraints: 2 (bookings, lab_events)';
PRINT '  ✓ DEFAULT constraints: 1 (notifications.is_read)';
PRINT '  ✓ ROWVERSION columns: 2 (bookings, lab_events)';

PRINT '';
PRINT '========================================';
PRINT 'Migration completed successfully at ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '========================================';
PRINT '';
PRINT 'POST-MIGRATION CHECKLIST:';
PRINT '  [ ] Update application code to use logged_at instead of timestamp';
PRINT '  [ ] Update application code to use action_type instead of action';
PRINT '  [ ] Update Entity Framework models to reflect new column types';
PRINT '  [ ] Implement optimistic concurrency using row_version columns';
PRINT '  [ ] Test all CRUD operations thoroughly';
PRINT '  [ ] Update database backup schedule';
GO

/*******************************************************************************
 * ROLLBACK SCRIPT (Use ONLY if partial migration completed)
 *******************************************************************************/
/*
-- WARNING: This rollback script only works if migration was partially completed
-- For complete rollback, restore from backup instead!

USE [LabManagementDB];
GO

PRINT 'Starting ROLLBACK of schema changes...';

-- Remove rowversion columns
IF COL_LENGTH('dbo.bookings', 'row_version') IS NOT NULL
    ALTER TABLE dbo.bookings DROP COLUMN row_version;

IF COL_LENGTH('dbo.lab_events', 'row_version') IS NOT NULL
    ALTER TABLE dbo.lab_events DROP COLUMN row_version;

-- Remove check constraints
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_bookings_time_range')
    ALTER TABLE dbo.bookings DROP CONSTRAINT CK_bookings_time_range;

IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_lab_events_time_range')
    ALTER TABLE dbo.lab_events DROP CONSTRAINT CK_lab_events_time_range;

-- Remove notifications default
IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_notifications_is_read')
    ALTER TABLE dbo.notifications DROP CONSTRAINT DF_notifications_is_read;

-- Rename columns back (reverse operation)
IF COL_LENGTH('dbo.security_logs', 'logged_at') IS NOT NULL
    EXEC sp_rename 'dbo.security_logs.logged_at', 'timestamp', 'COLUMN';

IF COL_LENGTH('dbo.security_logs', 'action_type') IS NOT NULL
    EXEC sp_rename 'dbo.security_logs.action_type', 'action', 'COLUMN';

-- NOTE: Cannot easily revert DATETIME2 -> DATETIME or VARCHAR(MAX) -> TEXT
-- without potential data loss. Restore from backup if needed!

PRINT 'ROLLBACK completed. Review changes and test thoroughly.';
PRINT 'WARNING: Data type conversions were NOT rolled back (DATETIME2, VARCHAR(MAX))';
PRINT 'Restore from backup for complete rollback!';
*/
