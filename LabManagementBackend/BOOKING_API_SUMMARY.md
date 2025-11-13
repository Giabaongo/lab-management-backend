# Booking API - CRUD Implementation Summary

## Files Created/Modified

### 1. DTOs (LabManagement.BLL/DTOs/)
- **BookingDTO.cs** - Response model cho booking
- **CreateBookingDTO.cs** - Model để tạo booking mới
- **UpdateBookingDTO.cs** - Model để cập nhật booking

### 2. Mapping (LabManagement.BLL/Mappings/)
- **BookingProfile.cs** - AutoMapper profile cho Booking

### 3. Service Layer (LabManagement.BLL/)
- **Interfaces/IBookingService.cs** - Interface cho booking service
- **Implementations/BookingService.cs** - Implementation của booking service với các methods:
  - GetAllBookingsAsync()
  - GetBookingByIdAsync(int id)
  - CreateBookingAsync(CreateBookingDTO, requesterId, requesterRole) *(kiểm tra quyền truy cập lab + buộc member đặt cho chính họ)*
  - UpdateBookingAsync(int id, UpdateBookingDTO)
  - DeleteBookingAsync(int id)
  - BookingExistsAsync(int id)
  - GetAvailableSlotsAsync(AvailableSlotQueryDTO, requesterId, requesterRole) *(chỉ trả slot cho lab mà người dùng có quyền xem)*

### 4. Repository Layer (LabManagement.DAL/)
- **Interfaces/IBookingRepository.cs** - Interface cho booking repository
- **Implementations/BookingRepository.cs** - Implementation của booking repository
- **Interfaces/IUnitOfWork.cs** - Đã cập nhật để thêm IBookingRepository
- **Implementations/UnitOfWork.cs** - Đã cập nhật để khởi tạo BookingRepository

### 5. API Controller (LabManagement.API/Controllers/)
- **BookingController.cs** - REST API endpoints:
  - GET /api/booking - Lấy tất cả bookings (yêu cầu authorization)
  - GET /api/booking/{id} - Lấy booking theo ID (yêu cầu authorization)
  - POST /api/booking - Tạo booking mới (yêu cầu authorization)
  - PUT /api/booking/{id} - Cập nhật booking (yêu cầu authorization)
  - DELETE /api/booking/{id} - Xóa booking (chỉ Admin và SchoolManager)

### 6. Configuration
- **Program.cs** - Đã cập nhật để đăng ký:
  - IBookingService -> BookingService
  - BookingProfile trong AutoMapper

## API Endpoints

### GET /api/booking
Lấy danh sách tất cả bookings
- **Authorization**: Required (any authenticated user)
- **Response**: `ApiResponse<IEnumerable<BookingDTO>>`

### GET /api/booking/{id}
Lấy thông tin chi tiết một booking
- **Authorization**: Required (any authenticated user)
- **Parameters**: `id` (int) - Booking ID
- **Response**: `ApiResponse<BookingDTO>`
- **Errors**: 404 Not Found nếu không tìm thấy

### POST /api/booking
Tạo booking mới
- **Authorization**: Required (any authenticated user)
- **Body**: `CreateBookingDTO`
  ```json
  {
    "userId": 1,
    "labId": 1,
    "zoneId": 1,
    "startTime": "2025-10-20T10:00:00Z",
    "endTime": "2025-10-20T12:00:00Z",
    "status": 1,
    "notes": "Optional notes"
  }
  ```
- **Response**: `ApiResponse<BookingDTO>`
- **Errors**: 400 Bad Request nếu data không hợp lệ

### PUT /api/booking/{id}
Cập nhật booking
- **Authorization**: Required (any authenticated user)
- **Parameters**: `id` (int) - Booking ID
- **Body**: `UpdateBookingDTO` (tất cả fields đều optional)
  ```json
  {
    "userId": 1,
    "labId": 1,
    "zoneId": 1,
    "startTime": "2025-10-20T10:00:00Z",
    "endTime": "2025-10-20T12:00:00Z",
    "status": 2,
    "notes": "Updated notes"
  }
  ```
- **Response**: `ApiResponse<BookingDTO>`
- **Errors**: 404 Not Found nếu không tìm thấy

### DELETE /api/booking/{id}
Xóa booking
- **Authorization**: Required (Admin hoặc SchoolManager only)
- **Parameters**: `id` (int) - Booking ID
- **Response**: `ApiResponse<object>`
- **Errors**: 404 Not Found nếu không tìm thấy

## Model Structure

### Booking Entity Properties
- `BookingId` - Primary key
- `UserId` - Foreign key tới User
- `LabId` - Foreign key tới Lab
- `ZoneId` - Foreign key tới LabZone
- `StartTime` - Thời gian bắt đầu
- `EndTime` - Thời gian kết thúc
- `Status` - Trạng thái booking
- `CreatedAt` - Thời gian tạo
- `Notes` - Ghi chú (optional)

## Notes
- Tất cả endpoints đều sử dụng JWT authentication
- DELETE endpoint chỉ cho phép Admin và SchoolManager
- Các endpoints khác cho phép mọi authenticated user
- Khi lấy slot hoặc tạo booking, hệ thống kiểm tra lab thuộc department public hoặc department mà người dùng đã đăng ký (hoặc do họ quản lý). Member chỉ được tạo booking cho chính mình.
- CreatedAt được tự động set khi tạo booking mới
- Sử dụng ApiResponse wrapper cho tất cả responses
- Exception handling được xử lý qua ExceptionMiddleware
