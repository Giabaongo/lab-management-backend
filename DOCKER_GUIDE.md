# 🐳 Docker Deployment Guide

## 📋 Tổng quan

Dự án có 2 môi trường Docker:

1. **Production** (Root folder) - API + Azure SQL Database
2. **Development** (LabManagementBackend folder) - API + SQL Server Local

---

## 🚀 PRODUCTION - Azure SQL Database

### Cấu hình
- **Database**: Azure SQL - `LabManagementDB_v2`
- **API Port**: 8080
- **Environment**: Production

### Chạy Production

```bash
# Build và chạy
docker-compose up -d --build

# Hoặc chỉ chạy (nếu đã build)
docker-compose up -d

# Xem logs
docker-compose logs -f api

# Dừng
docker-compose down
```

### Test API Production

```bash
# Health check
curl http://localhost:8080/api/test-connect

# Hoặc mở browser
open http://localhost:8080
```

---

## 🛠️ DEVELOPMENT - Local SQL Server

### Cấu hình
- **Database**: SQL Server 2022 trong Docker
- **SQL Port**: 1433
- **API Port**: 5162 (khi uncomment service API)
- **Environment**: Development

### Option 1: Chỉ chạy Database (Recommended)

```bash
cd LabManagementBackend

# Start SQL Server
docker-compose up -d sqlserver

# Check status
docker-compose ps

# Chạy API bằng dotnet CLI (linh hoạt hơn cho dev)
cd ..
dotnet run --project LabManagementBackend/LabManagement.API/LabManagement.API.csproj
```

### Option 2: Chạy cả Database + API

```bash
cd LabManagementBackend

# Uncomment service 'api' trong docker-compose.yml trước

# Start tất cả services
docker-compose up -d --build

# Xem logs
docker-compose logs -f
```

### Setup Database lần đầu

```bash
cd LabManagementBackend

# Cách 1: Dùng setup script (Recommended)
chmod +x setup-database.sh
./setup-database.sh

# Cách 2: Manual setup
docker-compose up -d sqlserver

# Đợi SQL Server ready
sleep 20

# Apply schema
cat LabManagement.DAL/schema_v2_optimized.sql | docker exec -i mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -d LabManagementDB -C
```

### Connection Strings

**Development (Local SQL Server):**
```
Server=localhost,1433;Database=LabManagementDB;User Id=sa;Password=Giabaongo123;TrustServerCertificate=True;Encrypt=False;
```

**Production (Azure SQL):**
```
server=bao-sql-server.database.windows.net;database=LabManagementDB_v2;uid=giabaongo;pwd=abcd1234@;TrustServerCertificate=False;Trusted_Connection=False;
```

---

## 🔧 Useful Commands

### Docker Compose

```bash
# Build lại images
docker-compose build

# Build không dùng cache
docker-compose build --no-cache

# Chạy và rebuild nếu có thay đổi
docker-compose up -d --build

# Xem logs real-time
docker-compose logs -f

# Xem logs của 1 service
docker-compose logs -f api

# Stop services
docker-compose stop

# Stop và xóa containers (giữ volumes)
docker-compose down

# Stop và xóa containers + volumes (XÓA DATA!)
docker-compose down -v

# List running services
docker-compose ps
```

### Docker CLI

```bash
# List containers
docker ps -a

# Exec vào SQL Server
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C

# Exec vào API container
docker exec -it lab-management-api-prod bash

# Xem logs
docker logs lab-management-api-prod -f

# Restart container
docker restart mssql2022

# Stop container
docker stop mssql2022

# Remove container
docker rm mssql2022

# Remove image
docker rmi lab-management-backend-api
```

---

## 📊 Kiểm tra Database

### SQL Server Local

```bash
# Vào SQL CLI
docker exec -it mssql2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Giabaongo123 -C

# Trong SQL CLI
USE LabManagementDB;
GO

SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
GO

SELECT COUNT(*) FROM Users;
GO

EXIT
```

---

## 🐛 Troubleshooting

### API không connect được database

```bash
# Check connection string trong container
docker exec lab-management-api-prod printenv | grep Connection

# Check API logs
docker logs lab-management-api-prod --tail 100
```

### SQL Server không start

```bash
# Check logs
docker logs mssql2022

# Restart SQL Server
docker restart mssql2022

# Check port
netstat -an | grep 1433
# hoặc
lsof -i :1433
```

### Build bị lỗi

```bash
# Clean build
docker-compose down
docker system prune -a
docker-compose build --no-cache
docker-compose up -d
```

### Port bị chiếm

```bash
# Tìm process đang dùng port
lsof -i :8080  # hoặc :1433

# Kill process
kill -9 <PID>

# Hoặc đổi port trong docker-compose.yml
ports:
  - "8081:80"  # Thay vì 8080:80
```

---

## 🔐 Security Notes

**⚠️ QUAN TRỌNG:**

1. **Không commit passwords** vào Git
2. Sử dụng **environment variables** hoặc **Docker secrets** cho production
3. Đổi password mặc định `Giabaongo123` trong production
4. Sử dụng **Azure Key Vault** hoặc **HashiCorp Vault** cho secrets management

### Cách sử dụng .env file

```bash
# Tạo file .env
cat > .env << EOF
SQL_SA_PASSWORD=YourStrongPassword123!
AZURE_SQL_PASSWORD=YourAzurePassword!
JWT_KEY=YourSuperSecretJwtKey
EOF

# Cập nhật docker-compose.yml để dùng .env
environment:
  - MSSQL_SA_PASSWORD=${SQL_SA_PASSWORD}
```

---

## 📈 Production Deployment Best Practices

1. **Sử dụng multi-stage builds** (đã có trong Dockerfile)
2. **Minimize image size** với .dockerignore
3. **Health checks** cho tất cả services
4. **Restart policies** = unless-stopped
5. **Volumes cho data persistence**
6. **Networks để isolate services**
7. **Logging và monitoring**
8. **Security scanning**: `docker scan <image-name>`

---

## 🎯 Workflows

### Developer mới join team

```bash
# 1. Clone repo
git clone <repo-url>
cd lab-management-backend

# 2. Setup database
cd LabManagementBackend
./setup-database.sh

# 3. Run API
cd ..
dotnet run --project LabManagementBackend/LabManagement.API/LabManagement.API.csproj
```

### Deploy lên server

```bash
# 1. Build Docker image
docker-compose build

# 2. Test local
docker-compose up -d
curl http://localhost:8080/api/test-connect

# 3. Push to registry (Docker Hub, Azure ACR, etc.)
docker tag lab-management-backend-api:latest <registry>/lab-management-api:latest
docker push <registry>/lab-management-api:latest

# 4. Deploy trên server
docker-compose -f docker-compose.yml up -d
```

---

## 📚 Tài liệu liên quan

- [DOCKER_SQL_SERVER_SETUP.md](LabManagementBackend/DOCKER_SQL_SERVER_SETUP.md)
- [QUICKSTART.md](LabManagementBackend/QUICKSTART.md)
- [API_RESPONSE_DOCUMENTATION.md](document/API_RESPONSE_DOCUMENTATION.md)

---

## 💡 Tips

1. **Luôn dùng volumes** để persist data
2. **Uncomment API service** trong dev khi cần test full stack
3. **Dùng setup-database.sh** cho first-time setup
4. **Dùng docker-compose** cho daily development
5. **Check logs thường xuyên** khi có lỗi

---

**Last Updated**: 2025-10-27
**Maintained by**: Lab Management Team
