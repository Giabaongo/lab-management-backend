# Database Migrations

This folder contains Entity Framework Core migrations for the Lab Management Database.

## 📋 Migration History

### Latest: **UpdateSchemaToDateTime2AndVarcharMax** (2025-10-23)
**File:** `20251023151315_UpdateSchemaToDateTime2AndVarcharMax.cs`

**Changes:**
- Converted TEXT columns to VARCHAR(MAX)
- Converted DATETIME columns to DATETIME2(3)
- Renamed `security_logs.timestamp` → `logged_at`
- Renamed `security_logs.action` → `action_type`
- Added `row_version` (ROWVERSION) to `bookings` and `lab_events`
- Added CHECK constraints for time validation
- Added DEFAULT constraint for `notifications.is_read`

**Status:** ✅ Applied

---

### **ConvertAllEnumColumnsToInt** (2025-10-20)
**File:** `20251020031626_ConvertAllEnumColumnsToInt.cs`

**Changes:**
- Converted all enum columns from DECIMAL to INT
- Aligned with C# enum types

**Status:** ✅ Applied

---

### **AlignUserRoleEnum** (2025-10-13)
**File:** `20251013163951_AlignUserRoleEnum.cs`

**Changes:**
- Aligned user role enum values with application constants

**Status:** ✅ Applied

---

### **Add_equiq_and_booking_table** (2025-10-06)
**File:** `20251006142242_Add_equiq_and_booking_table.cs`

**Changes:**
- Added `equipment` table
- Added `bookings` table
- Added foreign key relationships

**Status:** ✅ Applied

---

### **Remove_department_table** (2025-09-29)
**File:** `20250929040237_Remove_department_table.cs`

**Changes:**
- Removed `departments` table (no longer needed)

**Status:** ✅ Applied

---

### **InitialCreate** (2025-09-23)
**File:** `20250923111522_InitialCreate.cs`

**Changes:**
- Initial database schema creation
- Created all core tables: users, labs, lab_zones, lab_events, etc.
- Added seed data for initial setup

**Status:** ✅ Applied

---

## 🚀 How to Apply Migrations

### Method 1: Using EF Core CLI (Recommended)

```bash
# Apply all pending migrations
dotnet ef database update --project LabManagement.DAL --startup-project LabManagement.API

# Apply to specific migration
dotnet ef database update <MigrationName> --project LabManagement.DAL --startup-project LabManagement.API

# Rollback to previous migration
dotnet ef database update <PreviousMigrationName> --project LabManagement.DAL --startup-project LabManagement.API
```

### Method 2: Using SQL Script

```bash
# Generate SQL script for all migrations
dotnet ef migrations script --project LabManagement.DAL --startup-project LabManagement.API --output migration.sql --idempotent

# Apply using sqlcmd
sqlcmd -S localhost,1433 -U sa -P YourPassword -d LabManagementDB -i migration.sql
```

### Method 3: Manual Migration Script

For production environments, use the pre-generated safe migration script:

```bash
sqlcmd -S localhost,1433 -U sa -P YourPassword -d LabManagementDB -i SafeSchemaUpgrade.sql
```

---

## 🛠️ Creating New Migrations

```bash
# Add new migration
dotnet ef migrations add <MigrationName> --project LabManagement.DAL --startup-project LabManagement.API

# Example
dotnet ef migrations add AddNewTableOrColumn --project LabManagement.DAL --startup-project LabManagement.API
```

---

## ⚠️ Important Notes

### Before Applying Migrations:

1. **Backup Database:**
   ```sql
   BACKUP DATABASE [LabManagementDB] 
   TO DISK = N'C:\Backups\LabManagementDB_backup.bak'
   WITH COMPRESSION;
   ```

2. **Check Current Migration Status:**
   ```bash
   dotnet ef migrations list --project LabManagement.DAL --startup-project LabManagement.API
   ```

3. **Review Migration Code:**
   - Always review generated migration files before applying
   - Check for potential data loss warnings
   - Test on staging environment first

### After Applying Migrations:

1. **Verify Applied Migrations:**
   ```sql
   SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;
   ```

2. **Update Application Code:**
   - Check SCHEMA_VERSIONS.md for breaking changes
   - Update models, DTOs, and business logic accordingly

3. **Run Tests:**
   - Ensure all unit tests pass
   - Test API endpoints
   - Verify data integrity

---

## 🔄 Rollback Strategy

### Rollback Last Migration:

```bash
# Remove last migration (if not yet applied)
dotnet ef migrations remove --project LabManagement.DAL --startup-project LabManagement.API

# Rollback database to previous migration (if already applied)
dotnet ef database update <PreviousMigrationName> --project LabManagement.DAL --startup-project LabManagement.API
```

### Complete Rollback (Nuclear Option):

```sql
-- Restore from backup
USE master;
ALTER DATABASE [LabManagementDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [LabManagementDB] 
FROM DISK = N'C:\Backups\LabManagementDB_backup.bak'
WITH REPLACE, RECOVERY;
ALTER DATABASE [LabManagementDB] SET MULTI_USER;
```

---

## 📊 Migration Files Structure

```
Migrations/
├── 20250923111522_InitialCreate.cs                    # Migration code
├── 20250923111522_InitialCreate.Designer.cs           # Migration metadata
├── 20250929040237_Remove_department_table.cs
├── 20250929040237_Remove_department_table.Designer.cs
├── ...
├── 20251023151315_UpdateSchemaToDateTime2AndVarcharMax.cs
├── 20251023151315_UpdateSchemaToDateTime2AndVarcharMax.Designer.cs
├── LabManagementDbContextModelSnapshot.cs             # Current model state
├── SafeSchemaUpgrade.sql                              # Manual migration script
└── README.md                                          # This file
```

---

## 🐛 Troubleshooting

### Migration Already Applied Error:
```
Error: The migration '...' has already been applied to the database.
```
**Solution:** Migration is already in database, no action needed.

### Pending Model Changes Error:
```
Error: Your model changes do not match the pending migration.
```
**Solution:** 
```bash
dotnet ef migrations remove
dotnet ef migrations add <NewMigrationName>
```

### Connection String Issues:
**Solution:** Check `appsettings.json` in LabManagement.API project.

### Tool Not Found Error:
```bash
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
```

---

## 📚 Additional Resources

- [Entity Framework Core Migrations Documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [SQL Server Data Types Best Practices](https://learn.microsoft.com/en-us/sql/t-sql/data-types/)
- See `../SCHEMA_VERSIONS.md` for schema version history
- See `SafeSchemaUpgrade.sql` for production-safe manual migration

---

**Last Updated:** October 23, 2025  
**EF Core Version:** 8.0
