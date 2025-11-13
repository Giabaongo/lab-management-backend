-- =============================================
-- Add Lab Status Fields
-- Quick SQL Script
-- =============================================

USE [LabManagementDB];
GO

-- Add is_open field
ALTER TABLE dbo.labs
ADD is_open BIT NOT NULL DEFAULT 1;

-- Add status field
ALTER TABLE dbo.labs
ADD status INT NOT NULL DEFAULT 1;

-- Add open_time field
ALTER TABLE dbo.labs
ADD open_time TIME NULL DEFAULT '07:00:00';

-- Add close_time field
ALTER TABLE dbo.labs
ADD close_time TIME NULL DEFAULT '19:00:00';

-- Add CHECK constraint for time range
ALTER TABLE dbo.labs
ADD CONSTRAINT CK_labs_operating_hours 
CHECK (
    (open_time IS NULL AND close_time IS NULL) OR
    (open_time IS NOT NULL AND close_time IS NOT NULL AND open_time < close_time)
);

-- Add CHECK constraint for status values
ALTER TABLE dbo.labs
ADD CONSTRAINT CK_labs_status 
CHECK (status IN (1, 2, 3, 4));

GO

-- Verify
SELECT 
    lab_id,
    name,
    is_open,
    status,
    CONVERT(VARCHAR(5), open_time, 108) AS open_time,
    CONVERT(VARCHAR(5), close_time, 108) AS close_time
FROM dbo.labs;

GO

/*
Status values:
  1 = Active (Normal operation)
  2 = Closed (Temporarily closed)
  3 = Maintenance (Under maintenance)
  4 = Inactive (Permanently closed)

is_open:
  1 (True) = Lab is open
  0 (False) = Lab is closed
*/
