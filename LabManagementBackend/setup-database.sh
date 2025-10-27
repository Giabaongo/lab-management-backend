#!/bin/bash

##############################################################################
# Lab Management Database Setup Script
# This script sets up SQL Server and database for development
##############################################################################

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
CONTAINER_NAME="mssql2022"
SA_PASSWORD="Giabaongo123"
DATABASE_NAME="LabManagementDB"

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Lab Management Database Setup${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo -e "${RED}❌ Docker is not installed!${NC}"
    echo "Please install Docker first: https://docs.docker.com/get-docker/"
    exit 1
fi

echo -e "${GREEN}✓ Docker is installed${NC}"

# Check if container already exists
if docker ps -a --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
    echo -e "${YELLOW}⚠ Container ${CONTAINER_NAME} already exists${NC}"
    read -p "Do you want to remove it and start fresh? (y/n) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo "Stopping and removing existing container..."
        docker stop ${CONTAINER_NAME} 2>/dev/null || true
        docker rm ${CONTAINER_NAME} 2>/dev/null || true
        echo -e "${GREEN}✓ Old container removed${NC}"
    else
        echo "Keeping existing container. Exiting..."
        exit 0
    fi
fi

# Check if port 1433 is available
if lsof -Pi :1433 -sTCP:LISTEN -t >/dev/null 2>&1; then
    echo -e "${RED}❌ Port 1433 is already in use!${NC}"
    echo "Please stop the service using port 1433 or use docker-compose with a different port"
    exit 1
fi

echo -e "${GREEN}✓ Port 1433 is available${NC}"

# Start SQL Server container
echo ""
echo -e "${BLUE}Starting SQL Server container...${NC}"
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=${SA_PASSWORD}" \
  -e "MSSQL_PID=Developer" \
  -p 1433:1433 \
  --name ${CONTAINER_NAME} \
  --hostname sql1 \
  -d mcr.microsoft.com/mssql/server:2022-latest

echo -e "${GREEN}✓ Container started${NC}"

# Wait for SQL Server to be ready
echo ""
echo -e "${BLUE}Waiting for SQL Server to be ready...${NC}"
sleep 10

MAX_ATTEMPTS=30
ATTEMPT=0
while [ $ATTEMPT -lt $MAX_ATTEMPTS ]; do
    if docker exec ${CONTAINER_NAME} /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -C -Q "SELECT 1" &> /dev/null; then
        echo -e "${GREEN}✓ SQL Server is ready!${NC}"
        break
    fi
    ATTEMPT=$((ATTEMPT+1))
    echo -n "."
    sleep 2
done

if [ $ATTEMPT -eq $MAX_ATTEMPTS ]; then
    echo -e "${RED}❌ SQL Server failed to start within timeout${NC}"
    echo "Check logs with: docker logs ${CONTAINER_NAME}"
    exit 1
fi

# Create database
echo ""
echo -e "${BLUE}Creating database ${DATABASE_NAME}...${NC}"
docker exec ${CONTAINER_NAME} /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -C -Q "CREATE DATABASE [${DATABASE_NAME}]" 2>/dev/null || echo "Database might already exist"

# Check if schema file exists
if [ -f "LabManagement.DAL/schema_v2_optimized.sql" ]; then
    echo ""
    echo -e "${BLUE}Applying database schema...${NC}"
    cat LabManagement.DAL/schema_v2_optimized.sql | docker exec -i ${CONTAINER_NAME} /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -d ${DATABASE_NAME} -C
    echo -e "${GREEN}✓ Schema applied successfully${NC}"
else
    echo -e "${YELLOW}⚠ Schema file not found. You'll need to apply it manually:${NC}"
    echo "   dotnet ef database update --project LabManagement.DAL --startup-project LabManagement.API"
fi

# Verify database
echo ""
echo -e "${BLUE}Verifying database setup...${NC}"
TABLE_COUNT=$(docker exec ${CONTAINER_NAME} /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -d ${DATABASE_NAME} -C -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h -1 | tr -d ' ')

echo -e "${GREEN}✓ Database created with ${TABLE_COUNT} tables${NC}"

# Display connection info
echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}✅ Setup Complete!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "${BLUE}Connection Information:${NC}"
echo "  Server: localhost,1433"
echo "  Database: ${DATABASE_NAME}"
echo "  Username: sa"
echo "  Password: ${SA_PASSWORD}"
echo ""
echo -e "${BLUE}Connection String:${NC}"
echo "  Server=localhost,1433;Database=${DATABASE_NAME};User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True;Encrypt=False;"
echo ""
echo -e "${BLUE}Useful Commands:${NC}"
echo "  Start container:  docker start ${CONTAINER_NAME}"
echo "  Stop container:   docker stop ${CONTAINER_NAME}"
echo "  View logs:        docker logs ${CONTAINER_NAME}"
echo "  SQL CLI:          docker exec -it ${CONTAINER_NAME} /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -C"
echo ""
echo -e "${BLUE}Next Steps:${NC}"
echo "  1. Run API: dotnet run --project LabManagement.API"
echo "  2. API will be available at: http://localhost:5162"
echo "  3. See DOCKER_SQL_SERVER_SETUP.md for more details"
echo ""
