# SecurityLog CRUD API - Implementation Summary

## Các file đã tạo

### 1. DTOs (LabManagement.BLL/DTOs/)
- **SecurityLogDTO.cs** - Response model
- **CreateSecurityLogDTO.cs** - Chứa CreateSecurityLogDTO và UpdateSecurityLogDTO

### 2. Service Layer (LabManagement.BLL/)
- **Interfaces/ISecurityLogService.cs** - Interface cho security log service
- **Implementations/SecurityLogService.cs** - Implementation với CRUD methods:
  - GetAllSecurityLogsAsync()
  - GetSecurityLogByIdAsync(int id)
  - CreateSecurityLogAsync(CreateSecurityLogDTO)
  - UpdateSecurityLogAsync(int id, UpdateSecurityLogDTO)
  - DeleteSecurityLogAsync(int id)
  - SecurityLogExistsAsync(int id)

### 3. Mapping (LabManagement.BLL/Mappings/)
- **SecurityLogProfile.cs** - AutoMapper profile (tự động set Timestamp khi tạo mới)

### 4. Repository Layer (LabManagement.DAL/)
- **Interfaces/ISecurityLogRepository.cs** - Interface
- **Implementations/SecurityLogRepository.cs** - Implementation
- **Interfaces/IUnitOfWork.cs** - Cập nhật thêm SecurityLogs property
- **Implementations/UnitOfWork.cs** - Cập nhật để khởi tạo SecurityLogRepository

### 5. API Controller (LabManagement.API/Controllers/)
- **SecurityLogController.cs** - REST API endpoints

### 6. Configuration
- **Program.cs** - Đã cập nhật để đăng ký:
  - ISecurityLogService -> SecurityLogService
  - SecurityLogProfile trong AutoMapper

## API Endpoints

### GET /api/securitylog
Lấy tất cả security logs
- **Authorization**: Required
- **Response**: `ApiResponse<IEnumerable<SecurityLogDTO>>`

### GET /api/securitylog/{id}
Lấy security log theo ID
- **Authorization**: Required
- **Parameters**: `id` (int)
- **Response**: `ApiResponse<SecurityLogDTO>`
- **Errors**: 404 Not Found

### POST /api/securitylog
Tạo security log mới
- **Authorization**: Required
- **Body**: 
  ```json
  {
    "eventId": 1,
    "securityId": 4,
    "action": 1,
    "photoUrl": "https://example.com/photo.jpg",
    "notes": "Security checkpoint passed"
  }
  ```
- **Response**: `ApiResponse<SecurityLogDTO>`
- **Errors**: 400 Bad Request

### PUT /api/securitylog/{id}
Cập nhật security log
- **Authorization**: Required
- **Parameters**: `id` (int)
- **Body**: (tất cả fields optional)
  ```json
  {
    "eventId": 1,
    "securityId": 4,
    "action": 2,
    "photoUrl": "https://example.com/new-photo.jpg",
    "notes": "Updated notes"
  }
  ```
- **Response**: `ApiResponse<SecurityLogDTO>`
- **Errors**: 404 Not Found

### DELETE /api/securitylog/{id}
Xóa security log
- **Authorization**: Required
- **Parameters**: `id` (int)
- **Response**: `ApiResponse<object>`
- **Errors**: 404 Not Found

## Model Structure

### SecurityLog Entity Properties
- `LogId` - Primary key (int)
- `EventId` - Foreign key tới LabEvent (int, required)
- `SecurityId` - Foreign key tới User (int, required)
- `Action` - Hành động thực hiện (int, required)
- `Timestamp` - Thời gian log được tạo (DateTime, tự động set)
- `PhotoUrl` - URL của photo/screenshot (string, optional)
- `Notes` - Ghi chú (string, optional, max 500 characters)
- `Event` - Related LabEvent navigation property
- `Security` - Related User navigation property

## Features
- ✅ Full CRUD operations
- ✅ JWT Authentication on all endpoints
- ✅ Validation with Required và StringLength attributes
- ✅ AutoMapper for DTO mapping
- ✅ Exception handling (NotFoundException, BadRequestException)
- ✅ ApiResponse wrapper for all responses
- ✅ UnitOfWork pattern for data access
- ✅ Timestamp được tự động set khi tạo log mới
