# 🔬 Lab Management Backend

> Enterprise-grade ASP.NET Core 8.0 Web API for Laboratory Management System

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)]()
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)]()
[![License](https://img.shields.io/badge/license-MIT-blue)]()

---

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Quick Start](#-quick-start)
- [Documentation](#-documentation)
- [API Endpoints](#-api-endpoints)
- [Project Structure](#-project-structure)
- [Technologies](#-technologies)
- [Contributing](#-contributing)

---

## ✨ Features

- ✅ **Clean Architecture** - 4-layer separation (API, BLL, DAL, Common)
- ✅ **Repository Pattern** - Generic repository with type-safe operations
- ✅ **Global Exception Handling** - Automatic error handling with middleware
- ✅ **JWT Authentication** - Role-based authorization
- ✅ **BCrypt Password Hashing** - Secure credential storage
- ✅ **Standardized API Responses** - Consistent `ApiResponse<T>` wrapper
- ✅ **Dependency Injection** - Interface-based DI throughout
- ✅ **Docker Support** - Containerized deployment ready
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Azure SQL Database** - Cloud-ready database

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────┐
│                  LabManagement.API                   │
│          Controllers + Middleware + Swagger          │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│                  LabManagement.BLL                   │
│     Interfaces/ + Implementations/ + DTOs/           │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│                  LabManagement.DAL                   │
│     Interfaces/ + Implementations/ + Models/         │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│                  LabManagement.Common                │
│        Exceptions/ + Models/ + Constants/            │
└─────────────────────────────────────────────────────┘
```

**Design Patterns:**
- Repository Pattern
- Dependency Injection
- Middleware Pattern
- Factory Pattern
- Generic Programming

---

## 🚀 Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional)
- [SQL Server](https://www.microsoft.com/sql-server) or Azure SQL Database

### 1. Clone Repository
```bash
git clone https://github.com/Giabaongo/lab-management-backend.git
cd lab-management-backend/LabManagementBackend
```

### 2. Configure Database
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=LabManagementDB;..."
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_HERE",
    "Issuer": "LabManagementAPI",
    "Audience": "LabManagementClient"
  }
}
```

### 3. Run Migrations
```bash
dotnet ef database update --project LabManagement.DAL --startup-project LabManagement.API
```

### 4. Build & Run
```bash
dotnet build
dotnet run --project LabManagement.API
```

### 5. Access Swagger
Open browser: `http://localhost:5000/swagger`

---

## 📚 Documentation

| Document | Description |
|----------|-------------|
| [API Response Format](./API_RESPONSE_DOCUMENTATION.md) | Complete API response examples with cURL commands |
| [Restructuring Summary](./RESTRUCTURING_SUMMARY.md) | Detailed change log and architecture decisions |
| [Quick Reference](./QUICK_REFERENCE.md) | Cheat sheet for common tasks |
| [Commit Guide](./COMMIT_GUIDE.md) | Git commit best practices |

---

## 🌐 API Endpoints

### Authentication
```
POST   /api/Auth/login              Login with email/password
```

### User Management
```
GET    /api/User                    Get all users (Admin, SchoolManager)
GET    /api/User/{id}               Get user by ID
GET    /api/User/email/{email}      Get user by email
POST   /api/User                    Create new user (Admin only)
PUT    /api/User/{id}               Update user (Admin, SchoolManager)
DELETE /api/User/{id}               Delete user (Admin only)
GET    /api/User/{id}/exists        Check if user exists
GET    /api/User/email/{email}/exists  Check if email exists
```

### Example (Testing)
```
GET    /api/Example/success-example           Success response demo
GET    /api/Example/not-found-example/{id}    404 error demo
POST   /api/Example/bad-request-example       400 error demo
GET    /api/Example/unauthorized-example      401 error demo
GET    /api/Example/error-example             500 error demo
```

**Full API documentation:** [API_RESPONSE_DOCUMENTATION.md](./API_RESPONSE_DOCUMENTATION.md)

---

## 📁 Project Structure

```
LabManagementBackend/
├── LabManagement.API/              # 🌐 Presentation Layer
│   ├── Controllers/
│   │   ├── AuthController.cs      # Login endpoint
│   │   ├── UserController.cs      # User CRUD operations
│   │   └── ExampleController.cs   # Exception handling demos
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs # Global error handler
│   └── Program.cs                 # App configuration + DI
│
├── LabManagement.BLL/              # 🧠 Business Logic Layer
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IUserService.cs
│   │   └── IPasswordHasher.cs
│   ├── Implementations/
│   │   ├── AuthService.cs
│   │   ├── UserService.cs
│   │   └── PasswordHasher.cs
│   └── DTOs/                      # Data Transfer Objects
│
├── LabManagement.DAL/              # 💾 Data Access Layer
│   ├── Interfaces/
│   │   ├── IGenericRepository.cs  # Base CRUD interface
│   │   └── IUserRepository.cs
│   ├── Implementations/
│   │   ├── GenericRepository.cs   # Base implementation
│   │   └── UserRepository.cs
│   ├── Models/                    # EF Core entities
│   │   ├── User.cs
│   │   ├── Lab.cs
│   │   ├── LabEvent.cs
│   │   └── ...
│   └── Migrations/                # Database migrations
│
└── LabManagement.Common/           # 🔧 Shared Components
    ├── Constants/
    │   └── UserRole.cs            # Role enum + helpers
    ├── Exceptions/
    │   ├── NotFoundException.cs   # 404 errors
    │   ├── BadRequestException.cs # 400 errors
    │   └── UnauthorizedException.cs # 401 errors
    └── Models/
        └── ApiResponse.cs         # Standardized response wrapper
```

---

## 🛠️ Technologies

### Core
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core 8.0** - ORM
- **SQL Server / Azure SQL** - Database

### Authentication & Security
- **JWT Bearer Tokens** - Authentication
- **BCrypt.Net-Next 4.0.3** - Password hashing
- **Role-based Authorization** - Access control

### Tools & Libraries
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization
- **Azure** - Cloud deployment

### Design Patterns
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling
- **Middleware Pattern** - Request pipeline
- **Generic Programming** - Type-safe operations

---

## 👥 User Roles

| Role | Value | Description |
|------|-------|-------------|
| **Member** | 0 | Basic user |
| **LabSecurity** | 1 | Lab security guard |
| **LabManager** | 2 | Lab manager |
| **SchoolManager** | 3 | School manager |
| **Admin** | 4 | System administrator |

---

## 🧪 Testing

### Run All Tests
```bash
dotnet test
```

### Test API Endpoints
```bash
# Login
curl -X POST http://localhost:5000/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Admin123"}'

# Get Users (requires JWT token)
curl -X GET http://localhost:5000/api/User \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Test Exception Handling
curl http://localhost:5000/api/Example/not-found-example/999
```

---

## 🐳 Docker Deployment

### Build Image
```bash
docker-compose build
```

### Run Container
```bash
docker-compose up -d
```

### Access API
```
http://localhost:8080/swagger
```

**Docker guide:** See [DOCKER.md](./DOCKER.md) for detailed instructions

---

## 📊 API Response Format

All endpoints return standardized responses:

### Success (200/201)
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { /* your data here */ },
  "errors": []
}
```

### Error (400/401/404/500)
```json
{
  "success": false,
  "message": "Error description",
  "data": null,
  "errors": ["Detailed error message"]
}
```

---

## 🔐 Environment Variables

For deployment (Render, Azure, etc.), set these:

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=...;Database=...
Jwt__Key=YOUR_SECRET_KEY
Jwt__Issuer=LabManagementAPI
Jwt__Audience=LabManagementClient
```

---

## 🤝 Contributing

### Branching Strategy
- `main` - Production-ready code
- `develop` - Development branch
- `feature/*` - Feature branches
- `bugfix/*` - Bug fix branches

### Commit Convention
```
feat: Add new feature
fix: Fix bug
refactor: Refactor code
docs: Update documentation
test: Add tests
chore: Update dependencies
```

See [COMMIT_GUIDE.md](./COMMIT_GUIDE.md) for detailed guidelines

---

## 📝 License

This project is licensed under the MIT License.

---

## 👨‍💻 Authors

- **Giabaongo** - [GitHub](https://github.com/Giabaongo)

---

## 🙏 Acknowledgments

- ASP.NET Core Team
- Entity Framework Core Team
- BCrypt.Net Contributors
- Swagger/OpenAPI Community

---

## 📞 Support

For issues and questions:
- Create an issue on GitHub
- Email: [your-email@example.com]

---

**🎉 Built with ❤️ using ASP.NET Core 8.0**
