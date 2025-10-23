# SQL Server Docker Setup Guide

## 🐳 Quick Start - Sử dụng Docker Image có sẵn

### **Bước 1: Pull và chạy SQL Server container**

```bash
# Pull SQL Server 2022 image
docker pull mcr.microsoft.com/mssql/server:2022-latest

# Chạy container
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Giabaongo123" \
  -p 1433:1433 \
  --name mssql2022 \
  --hostname sql1 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### **Bước 2: Verify container đang chạy**

```bash
# Check container status
docker ps -a | grep mssql2022

# Check logs
docker logs mssql2022

# Nên thấy: "SQL Server is now ready for client connections"
```

### **Bước 3: Apply database schema**

**Option A - Sử dụng schema mới nhất (Recommended):**
```bash
# Từ thư mục root của project
cat LabManagement.DAL/schema_v2_optimized.sql | docker exec -i mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C
```

**Option B - Sử dụng EF Migrations:**
```bash
# Apply tất cả migrations
dotnet ef database update --project LabManagement.DAL --startup-project LabManagement.API
```

### **Bước 4: Test connection**

```bash
# Test với sqlcmd (nếu có cài)
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C

# Hoặc test từ API
dotnet run --project LabManagement.API
# API sẽ listen tại http://localhost:5162
```

---

## 📦 **Cách 2: Import Database từ Backup File**

### **Tạo backup trên máy của bạn:**

```bash
# 1. Tạo backup file
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C -Q "BACKUP DATABASE [LabManagementDB] TO DISK = N'/var/opt/mssql/data/LabManagementDB.bak' WITH COMPRESSION, STATS = 10;"

# 2. Copy backup file ra ngoài
docker cp mssql2022:/var/opt/mssql/data/LabManagementDB.bak ./LabManagementDB.bak

# 3. Compress (optional)
gzip LabManagementDB.bak
```

### **Teammate restore backup:**

```bash
# 1. Chạy SQL Server container (như Bước 1 ở trên)
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Giabaongo123" \
  -p 1433:1433 \
  --name mssql2022 \
  -d mcr.microsoft.com/mssql/server:2022-latest

# 2. Copy backup file vào container
docker cp LabManagementDB.bak mssql2022:/var/opt/mssql/data/

# 3. Restore database
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C -Q "RESTORE DATABASE [LabManagementDB] FROM DISK = N'/var/opt/mssql/data/LabManagementDB.bak' WITH MOVE 'LabManagementDB' TO '/var/opt/mssql/data/LabManagementDB.mdf', MOVE 'LabManagementDB_log' TO '/var/opt/mssql/data/LabManagementDB_log.ldf', REPLACE, STATS = 10;"
```

---

## 🔧 **Cách 3: Docker Compose (Best Practice)**

Tạo file `docker-compose.yml` trong root project:

```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: mssql2022
    hostname: sql1
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=Giabaongo123
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
      - ./LabManagement.DAL:/sql-scripts:ro
    networks:
      - lab-network
    restart: unless-stopped
    healthcheck:
      test: /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 5s
      retries: 10

volumes:
  sqlserver_data:
    driver: local

networks:
  lab-network:
    driver: bridge
```

**Teammate chỉ cần:**

```bash
# 1. Clone repo
git clone <repo-url>
cd lab-management-backend/LabManagementBackend

# 2. Start SQL Server
docker-compose up -d

# 3. Wait for SQL Server to be ready
docker-compose logs -f sqlserver
# Đợi thấy: "SQL Server is now ready for client connections"

# 4. Apply schema
cat LabManagement.DAL/schema_v2_optimized.sql | docker exec -i mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C

# 5. Run API
dotnet run --project LabManagement.API
```

---

## 🔐 **Credentials**

**QUAN TRỌNG:** Đây là credentials cho development environment

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

> ⚠️ **LƯU Ý:** Password này CHỈ dùng cho development. Production phải dùng password khác và quản lý qua secrets/environment variables!

---

## 📋 **Database Schema Version**

Current version: **v2.0 (Optimized)**

Sample data included:
- 5 users (Admin, SchoolManager, LabManager, SecurityStaff, Member)
- 3 labs (Biology Lab, Computer Lab, Chemistry Lab)
- 5 lab zones
- 4 equipment items
- 5 activity types

**Login credentials cho testing:**
```
Admin: admin@lab.com / password123
School Manager: schoolmanager@lab.com / password123
Lab Manager: manager@lab.com / password123
Security: security@lab.com / password123
Member: member@lab.com / password123
```

---

## 🐛 **Troubleshooting**

### Port 1433 already in use:
```bash
# Check what's using port 1433
sudo lsof -i :1433

# Stop existing SQL Server
docker stop mssql2022
docker rm mssql2022

# Or use different port
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Giabaongo123" \
  -p 1434:1433 \
  --name mssql2022 \
  -d mcr.microsoft.com/mssql/server:2022-latest

# Update connection string to use port 1434
```

### Password doesn't meet complexity requirements:
```
Password must:
- At least 8 characters
- Contain uppercase, lowercase, numbers, and special characters
```

### Container exits immediately:
```bash
# Check logs
docker logs mssql2022

# Common issue: EULA not accepted
# Solution: Make sure ACCEPT_EULA=Y is set
```

### Cannot connect from API:
```bash
# 1. Check container is running
docker ps | grep mssql2022

# 2. Check SQL Server is ready
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C -Q "SELECT @@VERSION"

# 3. Check connection string in appsettings.json
cat LabManagement.API/appsettings.json | grep ConnectionStrings

# 4. Test from host
# Install sqlcmd if needed (Ubuntu)
sudo apt-get install mssql-tools18 -y
sqlcmd -S localhost,1433 -U sa -P Giabaongo123 -C
```

---

## 🔄 **Useful Docker Commands**

```bash
# Start container
docker start mssql2022

# Stop container
docker stop mssql2022

# Restart container
docker restart mssql2022

# View logs
docker logs -f mssql2022

# Execute SQL command
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C

# Access container shell
docker exec -it mssql2022 /bin/bash

# Remove container (data will be lost unless using volume)
docker stop mssql2022
docker rm mssql2022

# Backup database
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C -Q "BACKUP DATABASE [LabManagementDB] TO DISK = N'/var/opt/mssql/data/backup.bak'"
docker cp mssql2022:/var/opt/mssql/data/backup.bak ./
```

---

## 📚 **Additional Resources**

- [SQL Server Docker Documentation](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker)
- [EF Core Migrations Guide](./LabManagement.DAL/Migrations/README.md)
- [Schema Version History](./LabManagement.DAL/SCHEMA_VERSIONS.md)
- [Git Workflow for Database](./GIT_MIGRATION_GUIDE.md)

---

**Last Updated:** October 24, 2025  
**SQL Server Version:** 2022 (16.0)  
**Docker Image:** mcr.microsoft.com/mssql/server:2022-latest
