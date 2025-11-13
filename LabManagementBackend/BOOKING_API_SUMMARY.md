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
- **Implementations/BookingService.cs** - Implementation với các methods đã được context-aware:
  - `GetAllBookingsAsync(requesterId, role)` *(lọc kết quả theo quyền: Member chỉ thấy booking của họ, LabManager thấy lab mình quản lý, Admin/Super được xem tất cả)*
  - `GetBookingByIdAsync(id, requesterId, role)`
  - `CreateBookingAsync(CreateBookingDTO, requesterId, role)` *(kiểm tra quyền lab, quota department, và zone thuộc lab)*
  - `UpdateBookingAsync(id, UpdateBookingDTO, requesterId, role)` *(chặn thay đổi userId nếu không phải admin, kiểm tra lab/zone mới và quota)*
  - `DeleteBookingAsync(id, requesterId, role)` *(chỉ Admin/SchoolManager mới thực thi được)*
  - `GetAvailableSlotsAsync(AvailableSlotQueryDTO, requesterId, role)` *(chỉ trả slot khi user có quyền lab, zone hợp lệ và chưa vượt quota department)*

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
- Tất cả endpoints đều sử dụng JWT authentication; response được bọc trong `ApiResponse<>`
- DELETE endpoint chỉ cho phép Admin và SchoolManager; **đọc/update** tự động lọc/kiểm tra quyền theo role
- Member chỉ nhìn/giao dịch với booking của chính họ và không thể chỉnh `userId`
- LabManager xem/điều chỉnh được booking thuộc lab mình quản lý
- Khi lấy slot hoặc tạo/đổi booking, hệ thống kiểm tra:
  1. Lab thuộc department public hoặc department mà người dùng đã đăng ký (hoặc do họ quản lý)
  2. ZoneId nằm trong lab tương ứng
  3. Thành viên chưa vượt quá `Constant.MaxDepartmentsPerMember` departments đang tham gia (kết hợp đăng ký + booking tương lai)
- `CreatedAt` được tự động set khi tạo booking mới
- Exception handling được xử lý qua `ExceptionMiddleware`
