# 🚀 API Response Format Documentation

## 📋 Standardized Response Structure

Tất cả API endpoints giờ sử dụng `ApiResponse<T>` wrapper với cấu trúc thống nhất:

### ✅ Success Response Format
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { /* actual data here */ },
  "errors": []
}
```

### ❌ Error Response Format
```json
{
  "success": false,
  "message": "Error description",
  "data": null,
  "errors": [
    "Detailed error message 1",
    "Detailed error message 2"
  ]
}
```

---

## 🔐 Authentication Endpoints

### POST /api/Auth/login
**Description:** Login with email and password

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": 1,
    "name": "John Doe",
    "email": "user@example.com",
    "role": 4
  },
  "errors": []
}
```

**Error Responses:**
- **400 Bad Request:** Email or Password is required
- **401 Unauthorized:** Invalid email or password

---

## 👥 User Management Endpoints

### GET /api/User
**Description:** Get all users (Admin and SchoolManager only)

**Authorization:** Roles: `3` (SchoolManager), `4` (Admin)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Users retrieved successfully",
  "data": [
    {
      "userId": 1,
      "name": "John Doe",
      "email": "john@example.com",
      "role": 4,
      "createdAt": "2025-01-01T00:00:00Z"
    },
    {
      "userId": 2,
      "name": "Jane Smith",
      "email": "jane@example.com",
      "role": 2,
      "createdAt": "2025-01-02T00:00:00Z"
    }
  ],
  "errors": []
}
```

---

### GET /api/User/{id}
**Description:** Get user by ID

**Authorization:** Roles: `2` (LabManager), `3` (SchoolManager), `4` (Admin)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "User retrieved successfully",
  "data": {
    "userId": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "role": 4,
    "createdAt": "2025-01-01T00:00:00Z"
  },
  "errors": []
}
```

**Error Response (404 Not Found):**
```json
{
  "success": false,
  "message": "User with key '999' was not found.",
  "data": null,
  "errors": ["User with key '999' was not found."]
}
```

---

### GET /api/User/email/{email}
**Description:** Get user by email

**Authorization:** Roles: `2` (LabManager), `3` (SchoolManager), `4` (Admin)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "User retrieved successfully",
  "data": {
    "userId": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "role": 4,
    "createdAt": "2025-01-01T00:00:00Z"
  },
  "errors": []
}
```

**Error Responses:**
- **400 Bad Request:** Email cannot be empty
- **404 Not Found:** User with email 'xxx@example.com' was not found

---

### POST /api/User
**Description:** Create a new user (Admin only)

**Authorization:** Role: `4` (Admin)

**Request Body:**
```json
{
  "name": "New User",
  "email": "newuser@example.com",
  "password": "SecurePassword123",
  "role": 2
}
```

**Success Response (201 Created):**
```json
{
  "success": true,
  "message": "User created successfully",
  "data": {
    "userId": 3,
    "name": "New User",
    "email": "newuser@example.com",
    "role": 2,
    "createdAt": "2025-10-12T10:30:00Z"
  },
  "errors": []
}
```

**Error Responses:**
- **400 Bad Request:** 
  - Invalid user data
  - Email 'xxx@example.com' already exists
- **401 Unauthorized:** Missing or invalid JWT token
- **403 Forbidden:** User does not have Admin role

---

### PUT /api/User/{id}
**Description:** Update user by ID (Admin and SchoolManager only)

**Authorization:** Roles: `3` (SchoolManager), `4` (Admin)

**Request Body:**
```json
{
  "name": "Updated Name",
  "email": "updated@example.com",
  "password": "NewPassword123",
  "role": 3
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "User updated successfully",
  "data": {
    "userId": 1,
    "name": "Updated Name",
    "email": "updated@example.com",
    "role": 3,
    "createdAt": "2025-01-01T00:00:00Z"
  },
  "errors": []
}
```

**Error Responses:**
- **400 Bad Request:** 
  - Invalid user data
  - Email 'xxx@example.com' already exists
- **404 Not Found:** User with key '999' was not found

---

### DELETE /api/User/{id}
**Description:** Delete user by ID (Admin only)

**Authorization:** Role: `4` (Admin)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "User deleted successfully",
  "data": {
    "deletedUserId": 1
  },
  "errors": []
}
```

**Error Response (404 Not Found):**
```json
{
  "success": false,
  "message": "User with key '999' was not found.",
  "data": null,
  "errors": ["User with key '999' was not found."]
}
```

---

### GET /api/User/{id}/exists
**Description:** Check if user exists

**Authorization:** Roles: `2` (LabManager), `3` (SchoolManager), `4` (Admin)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "User exists",
  "data": {
    "userId": 1,
    "exists": true
  },
  "errors": []
}
```

---

### GET /api/User/email/{email}/exists
**Description:** Check if email exists

**Authorization:** Roles: `2` (LabManager), `3` (SchoolManager), `4` (Admin)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Email exists",
  "data": {
    "email": "john@example.com",
    "exists": true
  },
  "errors": []
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Email cannot be empty",
  "data": null,
  "errors": ["Email cannot be empty"]
}
```

---

## 🎯 Example Testing Endpoints

### GET /api/Example/success-example
**Description:** Example of success response

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": "This is a success message",
  "errors": []
}
```

---

### GET /api/Example/not-found-example/{id}
**Description:** Example of NotFoundException (404)

**Error Response (404 Not Found):**
```json
{
  "success": false,
  "message": "User with key '999' was not found.",
  "data": null,
  "errors": ["User with key '999' was not found."]
}
```

---

### POST /api/Example/bad-request-example
**Description:** Example of BadRequestException (400)

**Request Body:**
```json
"invalid-email"
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Invalid email format",
  "data": null,
  "errors": ["Invalid email format"]
}
```

---

### GET /api/Example/unauthorized-example
**Description:** Example of UnauthorizedException (401)

**Error Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "You are not authorized to access this resource",
  "data": null,
  "errors": ["You are not authorized to access this resource"]
}
```

---

### GET /api/Example/error-example
**Description:** Example of unhandled exception (500)

**Error Response (500 Internal Server Error):**

**Development Environment:**
```json
{
  "success": false,
  "message": "An internal server error occurred.",
  "data": null,
  "errors": [
    "Something went terribly wrong!",
    "   at LabManagement.API.Controllers.ExampleController.GetErrorExample() in ..."
  ]
}
```

**Production Environment:**
```json
{
  "success": false,
  "message": "An internal server error occurred.",
  "data": null,
  "errors": ["An internal server error occurred. Please contact support."]
}
```

---

## 📝 HTTP Status Codes

| Status Code | Description | When Used |
|-------------|-------------|-----------|
| **200 OK** | Success | Successful GET, PUT requests |
| **201 Created** | Resource created | Successful POST requests |
| **400 Bad Request** | Invalid input | Validation errors, bad data |
| **401 Unauthorized** | Authentication failed | Invalid credentials, missing token |
| **403 Forbidden** | Authorization failed | User lacks required permissions |
| **404 Not Found** | Resource not found | Entity doesn't exist in database |
| **500 Internal Server Error** | Server error | Unhandled exceptions |

---

## 🔑 User Roles Reference

| Role | Value | Description |
|------|-------|-------------|
| **Admin** | 0 | System administrator |
| **SchoolManager** | 1 | School manager |
| **LabManager** | 2 | Lab manager |
| **SecurityLab** | 3 | Lab security guard |
| **Member** | 4 | Basic user |

---

## 🛡️ Authorization Rules

| Endpoint | Required Roles |
|----------|----------------|
| `GET /api/User` | SchoolManager (1), Admin (0) |
| `GET /api/User/{id}` | LabManager (2), SchoolManager (1), Admin (0) |
| `GET /api/User/email/{email}` | LabManager (2), SchoolManager (1), Admin (0) |
| `POST /api/User` | Admin (0) only |
| `PUT /api/User/{id}` | SchoolManager (1), Admin (0) |
| `DELETE /api/User/{id}` | Admin (0) only |
| `GET /api/User/{id}/exists` | LabManager (2), SchoolManager (1), Admin (0) |
| `GET /api/User/email/{email}/exists` | LabManager (2), SchoolManager (1), Admin (0) |

---

## 🧪 Testing with cURL

### Login
```bash
curl -X POST http://localhost:5000/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@example.com",
    "password": "Admin123"
  }'
```

### Get All Users (with JWT token)
```bash
curl -X GET http://localhost:5000/api/User \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### Create User
```bash
curl -X POST http://localhost:5000/api/User \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "New User",
    "email": "newuser@example.com",
    "password": "SecurePass123",
    "role": 2
  }'
```

### Test Exception Handling
```bash
# Test 404 Not Found
curl -X GET http://localhost:5000/api/Example/not-found-example/999

# Test 400 Bad Request
curl -X POST http://localhost:5000/api/Example/bad-request-example \
  -H "Content-Type: application/json" \
  -d '"invalid-email"'

# Test 500 Internal Error
curl -X GET http://localhost:5000/api/Example/error-example
```

---

## ✨ Key Improvements

1. **✅ Consistent Response Format** - Tất cả endpoints trả về cùng structure
2. **✅ No Try-Catch in Controllers** - Clean code, exceptions được middleware handle
3. **✅ Proper HTTP Status Codes** - 200, 201, 400, 401, 404, 500
4. **✅ Meaningful Error Messages** - Clear, descriptive error descriptions
5. **✅ Environment-Aware Errors** - Dev shows stack trace, Production hides details
6. **✅ Type-Safe Responses** - `ApiResponse<T>` với generic type
7. **✅ Role-Based Authorization** - Proper access control trên từng endpoint
8. **✅ Auto Logging** - Tất cả errors được log tự động

---

## 🎓 Migration Summary

### Before (Old Pattern):
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<UserDTO>> GetUserById(int id)
{
    try
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });
        return Ok(user);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Internal server error", error = ex.Message });
    }
}
```

### After (New Pattern):
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ApiResponse<UserDTO>>> GetUserById(int id)
{
    var user = await _userService.GetUserByIdAsync(id);
    
    if (user == null)
        throw new NotFoundException("User", id);

    return Ok(ApiResponse<UserDTO>.SuccessResponse(user, "User retrieved successfully"));
}
```

**Benefits:**
- ✅ 10 lines → 6 lines (40% less code)
- ✅ No try-catch boilerplate
- ✅ Consistent error handling
- ✅ Better testability
- ✅ Cleaner, more readable

---

🎉 **All endpoints are now using standardized ApiResponse format!**
