# 🚀 Quick Start Guide

## Teammate Setup (First Time)

### Method 1: One-Command Setup (Easiest) ⚡

```bash
# Clone repo
git clone <your-repo-url>
cd lab-management-backend/LabManagementBackend

# Run setup script
./setup-database.sh

# Start API
dotnet run --project LabManagement.API
```

Xong! API sẽ chạy tại: http://localhost:5162

---

### Method 2: Using Docker Compose 🐳

```bash
# Start SQL Server
docker-compose up -d

# Wait 30 seconds for SQL Server to be ready
sleep 30

# Apply schema
cat LabManagement.DAL/schema_v2_optimized.sql | docker exec -i mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C

# Start API
dotnet run --project LabManagement.API
```

---

### Method 3: Manual Setup (Full Control) 🔧

See [DOCKER_SQL_SERVER_SETUP.md](./DOCKER_SQL_SERVER_SETUP.md) for detailed instructions.

---

## Database Credentials 🔐

```
Server: localhost,1433
Database: LabManagementDB
Username: sa
Password: Giabaongo123
```

**Connection String:**
```
Server=localhost,1433;Database=LabManagementDB;User Id=sa;Password=Giabaongo123;TrustServerCertificate=True;Encrypt=False;
```

---

## Test Users 👥

| Email | Password | Role |
|-------|----------|------|
| admin@lab.com | password123 | Admin |
| schoolmanager@lab.com | password123 | School Manager |
| manager@lab.com | password123 | Lab Manager |
| security@lab.com | password123 | Security Staff |
| member@lab.com | password123 | Member |

---

## Common Commands 📋

```bash
# Start SQL Server
docker start mssql2022

# Stop SQL Server
docker stop mssql2022

# View SQL Server logs
docker logs mssql2022

# Connect to SQL CLI
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C

# Apply latest migrations
dotnet ef database update --project LabManagement.DAL --startup-project LabManagement.API

# Run API
dotnet run --project LabManagement.API

# Run with watch (auto-reload)
dotnet watch run --project LabManagement.API
```

---

## Troubleshooting 🐛

### Port 1433 already in use?
```bash
# Check what's using the port
sudo lsof -i :1433

# Kill the process or use different port
docker run -p 1434:1433 ...
```

### Cannot connect to SQL Server?
```bash
# Check container is running
docker ps | grep mssql2022

# Check SQL Server logs
docker logs mssql2022

# Restart container
docker restart mssql2022
```

### Database schema outdated?
```bash
# Apply latest migrations
dotnet ef database update --project LabManagement.DAL --startup-project LabManagement.API
```

---

## Documentation 📚

- **[DOCKER_SQL_SERVER_SETUP.md](./DOCKER_SQL_SERVER_SETUP.md)** - Detailed SQL Server setup
- **[SCHEMA_VERSIONS.md](./LabManagement.DAL/SCHEMA_VERSIONS.md)** - Database version history
- **[Migrations README](./LabManagement.DAL/Migrations/README.md)** - How to work with migrations
- **[GIT_MIGRATION_GUIDE.md](./GIT_MIGRATION_GUIDE.md)** - Git workflow for database changes

---

## Project Structure 📁

```
LabManagementBackend/
├── LabManagement.API/          # REST API Controllers
├── LabManagement.BLL/          # Business Logic Layer
├── LabManagement.DAL/          # Data Access Layer
│   ├── Migrations/             # EF Core Migrations
│   ├── Models/                 # Entity Models
│   └── schema_v2_optimized.sql # Full database schema
├── LabManagement.Common/       # Shared utilities
├── docker-compose.yml          # Docker configuration
└── setup-database.sh           # Quick setup script
```

---

## Need Help? 💬

1. Check existing documentation files
2. Ask in team chat
3. Create an issue in the repo

---

**Last Updated:** October 24, 2025
