# 📊 Database Structure & Data Location Guide

## 🗂️ **1. Table Definitions (Schema)**

### **Source Code - Entity Models**
Nơi định nghĩa cấu trúc tables trong code:

```
LabManagement.DAL/Models/
├── User.cs               → users table (6 columns)
├── Lab.cs                → labs table (5 columns)
├── Booking.cs            → bookings table (10 columns)
├── Equipment.cs          → equipment table (6 columns)
├── LabZone.cs            → lab_zones table (4 columns)
├── LabEvent.cs           → lab_events table (12 columns)
├── EventParticipant.cs   → event_participants table (3 columns)
├── Notification.cs       → notifications table (6 columns)
├── Report.cs             → reports table (7 columns)
├── SecurityLog.cs        → security_logs table (7 columns)
├── ActivityType.cs       → activity_types table (3 columns)
└── LabManagementDbContext.cs  → Relationships, Constraints, Seed Data
```

**Example - Xem cấu trúc User table:**
```bash
cat LabManagement.DAL/Models/User.cs
```

### **SQL Schema Files**
Nơi định nghĩa SQL thuần túy:

```
LabManagement.DAL/
├── schema_v2_optimized.sql       ✅ CURRENT (v2.0 - 2,209 lines)
│   └── Full CREATE TABLE statements
│   └── Indexes, Constraints, Foreign Keys
│   └── Seed data INSERT statements
│
├── schema.sql                    ❌ DEPRECATED (v1.0 - 294 lines)
│
└── Migrations/
    ├── SafeSchemaUpgrade.sql     Manual upgrade v1→v2
    └── *.cs                      EF Core migrations
```

**View current schema:**
```bash
# Xem full schema
cat LabManagement.DAL/schema_v2_optimized.sql

# Xem chỉ CREATE TABLE statements
grep -A 20 "CREATE TABLE" LabManagement.DAL/schema_v2_optimized.sql
```

---

## 💾 **2. Data Storage Locations**

### **A. Seed Data (Initial Sample Data)** 📦

**Location:** `LabManagement.DAL/Models/LabManagementDbContext.cs`

**Seed Data Summary:**
```csharp
// Line 74-82: ActivityTypes
5 records: Workshop, Seminar, Research, Experiment, Meeting

// Line 161-165: Equipment  
4 records: Microscope, Centrifuge, Computer Station, Server Rack

// Line 218-222: Labs
3 records: Biology Lab, Computer Lab, Chemistry Lab

// Line 305-309: LabZones
5 records: Zone A, Zone B in each lab

// Line 452-458: Users
5 users: Admin, SchoolManager, LabManager, SecurityStaff, Member
```

**View seed data:**
```bash
# Xem seed data cho Users
grep -A 10 "entity.HasData" LabManagement.DAL/Models/LabManagementDbContext.cs | grep "new User"

# Hoặc view trực tiếp trong file
cat LabManagement.DAL/Models/LabManagementDbContext.cs | grep -A 6 "UserId = 1"
```

### **B. Runtime Data (Actual Database Data)** 💿

**Physical Location:**

```bash
# Docker Volume (Persistent Storage)
Docker Volume Name: mssql_data
Host Path: /var/lib/docker/volumes/mssql_data/_data/

# Inside Container
Container Path: /var/opt/mssql/data/
```

**Database Files:**
```
/var/opt/mssql/data/
├── LabManagementDB.mdf          # 📄 Main data file (tables, indexes, data)
├── LabManagementDB_log.ldf      # 📝 Transaction log
├── master.mdf                   # System database
├── model.mdf                    # Template database
├── msdb.mdf                     # SQL Agent jobs
└── tempdb.mdf                   # Temporary tables
```

**Check volume location:**
```bash
docker volume inspect mssql_data
```

**Current Data (as of last check):**
```
users                  : 6 records  (5 seed + 1 runtime)
labs                   : 3 records  (seed data)
bookings               : 0 records  (empty)
equipment              : 4 records  (seed data)
lab_zones              : 5 records  (seed data)
lab_events             : 1 record   (1 runtime)
event_participants     : 0 records  (empty)
notifications          : 0 records  (empty)
reports                : 0 records  (empty)
security_logs          : 1 record   (1 runtime)
activity_types         : 5 records  (seed data)
```

---

## 🔍 **3. How to Query/View Data**

### **Method 1: SQL Command Line** (Fastest)

```bash
# Connect to SQL CLI
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -d LabManagementDB -C

# List all tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME;
GO

# Count records in each table
SELECT 'users' as TableName, COUNT(*) as RecordCount FROM users
UNION ALL SELECT 'labs', COUNT(*) FROM labs
UNION ALL SELECT 'bookings', COUNT(*) FROM bookings;
GO

# View users
SELECT * FROM users;
GO

# Exit
EXIT
```

### **Method 2: One-liner Queries**

```bash
# List all tables
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -d LabManagementDB -C -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"

# View users
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -d LabManagementDB -C -Q "SELECT user_id, name, email, role FROM users"

# Count all records
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -d LabManagementDB -C -Q "
SELECT 'users' as [Table], COUNT(*) as [Count] FROM users
UNION ALL SELECT 'labs', COUNT(*) FROM labs
UNION ALL SELECT 'bookings', COUNT(*) FROM bookings
UNION ALL SELECT 'lab_events', COUNT(*) FROM lab_events"
```

### **Method 3: Using EF Core in Code**

```csharp
// In any service or controller
public async Task<IEnumerable<User>> GetAllUsers()
{
    return await _context.Users.ToListAsync();
}

// With Include (JOIN)
var bookingsWithDetails = await _context.Bookings
    .Include(b => b.User)
    .Include(b => b.Lab)
    .Include(b => b.Zone)
    .ToListAsync();
```

### **Method 4: API Endpoints**

```bash
# Start API
dotnet run --project LabManagement.API

# Query via API
curl http://localhost:5162/api/user
curl http://localhost:5162/api/lab
curl http://localhost:5162/api/booking
```

---

## 📤 **4. Export/Backup Data**

### **Export to Backup File**

```bash
# Create backup
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C -Q "BACKUP DATABASE [LabManagementDB] TO DISK = N'/var/opt/mssql/data/LabManagementDB.bak' WITH COMPRESSION"

# Copy backup to host
docker cp mssql2022:/var/opt/mssql/data/LabManagementDB.bak ./backup_$(date +%Y%m%d).bak

# Compress
gzip backup_*.bak
```

### **Export to SQL Script**

```bash
# Generate insert scripts (using schema file)
cat LabManagement.DAL/schema_v2_optimized.sql | grep "INSERT INTO"

# Or use EF to generate full script
dotnet ef migrations script --project LabManagement.DAL --startup-project LabManagement.API --output full_schema.sql --idempotent
```

### **Export to CSV**

```bash
# Export users to CSV
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -d LabManagementDB -C -Q "SELECT * FROM users" -o users.csv -s"," -W
```

---

## 📥 **5. Import/Restore Data**

### **From Backup File**

```bash
# Copy backup to container
docker cp backup.bak mssql2022:/var/opt/mssql/data/

# Restore
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C -Q "RESTORE DATABASE [LabManagementDB] FROM DISK = N'/var/opt/mssql/data/backup.bak' WITH REPLACE"
```

### **From SQL Schema**

```bash
# Fresh install
cat LabManagement.DAL/schema_v2_optimized.sql | docker exec -i mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C
```

### **Using EF Migrations**

```bash
# Apply all migrations (creates tables + seed data)
dotnet ef database update --project LabManagement.DAL --startup-project LabManagement.API
```

---

## 🗺️ **6. Database Schema Map**

```
┌─────────────────────────────────────────────────────────────────┐
│                      LabManagementDB                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Core Tables:                                                   │
│  ├─ users (6 records)          → Authentication & roles        │
│  ├─ labs (3 records)           → Lab information               │
│  └─ activity_types (5 records) → Event categories              │
│                                                                 │
│  Lab Management:                                                │
│  ├─ lab_zones (5 records)      → Zones within labs             │
│  ├─ equipment (4 records)      → Lab equipment                 │
│  └─ bookings (0 records)       → Lab reservations              │
│                                                                 │
│  Events:                                                        │
│  ├─ lab_events (1 record)      → Scheduled events              │
│  ├─ event_participants (0)     → Event attendees               │
│  └─ security_logs (1 record)   → Security access logs          │
│                                                                 │
│  Communication:                                                 │
│  ├─ notifications (0 records)  → User notifications            │
│  └─ reports (0 records)        → Generated reports             │
│                                                                 │
│  Metadata:                                                      │
│  └─ __EFMigrationsHistory      → Migration tracking            │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔧 **7. Quick Reference Commands**

```bash
# Start database
docker start mssql2022

# Stop database
docker stop mssql2022

# View all data locations
docker volume ls
docker volume inspect mssql_data

# Access SQL prompt
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -d LabManagementDB -C

# Check database size
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -d LabManagementDB -C -Q "EXEC sp_spaceused"

# View migration history
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -d LabManagementDB -C -Q "SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId"
```

---

## 📚 **Related Documentation**

- **[SCHEMA_VERSIONS.md](./LabManagement.DAL/SCHEMA_VERSIONS.md)** - Database version history
- **[Migrations README](./LabManagement.DAL/Migrations/README.md)** - Migration management
- **[DOCKER_SQL_SERVER_SETUP.md](./DOCKER_SQL_SERVER_SETUP.md)** - Setup guide
- **[QUICKSTART.md](./QUICKSTART.md)** - Quick start for teammates

---

**Last Updated:** October 24, 2025  
**Database Version:** v2.0 (Optimized)  
**Total Tables:** 11 (+ 1 metadata table)
