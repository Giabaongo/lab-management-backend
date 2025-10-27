# Sử dụng SDK image để build application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy solution file và project files
COPY LabManagementBackend/*.sln ./
COPY LabManagementBackend/LabManagement.API/*.csproj ./LabManagement.API/
COPY LabManagementBackend/LabManagement.BLL/*.csproj ./LabManagement.BLL/
COPY LabManagementBackend/LabManagement.DAL/*.csproj ./LabManagement.DAL/
COPY LabManagementBackend/LabManagement.Common/*.csproj ./LabManagement.Common/

# Restore dependencies
RUN dotnet restore

# Copy toàn bộ source code
COPY LabManagementBackend/ ./

# Build application
RUN dotnet publish LabManagement.API/LabManagement.API.csproj -c Release -o out

# Sử dụng runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published app từ build stage
COPY --from=build-env /app/out .

# Render sets PORT env variable dynamically
# Default to 80 if PORT is not set (for local testing)
ENV ASPNETCORE_URLS=http://+:${PORT:-80}

# Expose port (Render will override this)
EXPOSE ${PORT:-80}

# Entry point
ENTRYPOINT ["dotnet", "LabManagement.API.dll"]