# Public Department Validation - Feature Documentation

## Overview

Added validation to prevent registration to public departments and created a dedicated API endpoint for members to fetch only departments that require registration.

## Changes Made

### 1. Registration Validation for Public Departments

**File**: `LabManagement.BLL/Implementations/DepartmentService.cs`

**Method**: `RegisterUserToDepartmentAsync`

**Logic**:
- Before allowing registration, check if `Department.IsPublic == true`
- If true, throw `BadRequestException` with message: "Public departments are accessible to everyone. No registration required."
- This ensures members cannot register to departments that are already publicly accessible

**Code**:
```csharp
// Public departments don't require registration
if (department.IsPublic)
{
    throw new BadRequestException("Public departments are accessible to everyone. No registration required.");
}
```

### 2. New API Endpoint: Get Registerable Departments

**Endpoint**: `GET /api/departments/registerable`

**Authorization**: Members only

**Purpose**: Returns only departments that:
- Are NOT public (`IsPublic = false`)
- User has NOT already registered to (any status: Pending, Approved, or Rejected)
- Includes a `CanRegister` flag indicating if user has reached the department limit

**Response Example**:
```json
{
  "data": [
    {
      "departmentId": 3,
      "name": "Computer Science",
      "description": "CS Department",
      "isPublic": false,
      "isUserMember": false,
      "canRegister": true
    },
    {
      "departmentId": 5,
      "name": "Electrical Engineering",
      "description": "EE Department",
      "isPublic": false,
      "isUserMember": false,
      "canRegister": false  // User already has 2 approved departments
    }
  ],
  "message": "Registerable departments retrieved successfully",
  "success": true
}
```

### 3. DepartmentDTO Enhancement

**File**: `LabManagement.BLL/DTOs/DepartmentDTO.cs`

**New Property**: `CanRegister` (bool, default: true)

**Purpose**: Indicates whether the user can still register to departments based on the limit (`MaxDepartmentsPerMember = 2`)

## Implementation Details

### Service Method: `GetRegisterableDepartmentsAsync`

**File**: `LabManagement.BLL/Implementations/DepartmentService.cs`

**Interface**: `IDepartmentService`

**Logic**:
1. Validate user exists
2. Get all departments user has already registered to (any status)
3. Count how many approved memberships user has
4. Query departments where:
   - `IsPublic = false`
   - `DepartmentId NOT IN (user's registered departments)`
5. Map to DTOs and set `CanRegister` based on approved count vs limit

**Code**:
```csharp
public async Task<IEnumerable<DepartmentDTO>> GetRegisterableDepartmentsAsync(int userId)
{
    var user = await _unitOfWork.Users.GetByIdAsync(userId);
    if (user == null)
    {
        throw new NotFoundException("User", userId);
    }

    // Get all departments that user has already registered to (any status)
    var userDepartmentIds = await _unitOfWork.UserDepartments
        .GetUserDepartmentsQueryable()
        .Where(ud => ud.UserId == userId)
        .Select(ud => ud.DepartmentId)
        .ToListAsync();

    // Check if user has reached the limit
    var approvedCount = await _unitOfWork.UserDepartments
        .GetUserDepartmentsQueryable()
        .Where(ud => ud.UserId == userId && ud.Status == (int)Constant.RegistrationStatus.Approved)
        .CountAsync();

    // Get departments that:
    // - Are NOT public (IsPublic = false)
    // - User has NOT already registered to
    var departments = await _unitOfWork.Departments
        .GetDepartmentsQueryable()
        .Where(d => !d.IsPublic && !userDepartmentIds.Contains(d.DepartmentId))
        .AsNoTracking()
        .ToListAsync();

    var result = _mapper.Map<List<DepartmentDTO>>(departments);
    
    // Add a flag indicating if user can still register
    foreach (var dto in result)
    {
        dto.CanRegister = approvedCount < Constant.MaxDepartmentsPerMember;
    }

    return result;
}
```

### Controller Endpoint

**File**: `LabManagement.API/Controllers/DepartmentController.cs`

**Route**: `GET /api/departments/registerable`

**Authorization**: `[Authorize(Roles = nameof(Constant.UserRole.Member))]`

**Code**:
```csharp
/// <summary>
/// Get non-public departments that the member can register to (Member only)
/// </summary>
[HttpGet("registerable")]
[Authorize(Roles = nameof(Constant.UserRole.Member))]
public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDTO>>>> GetRegisterableDepartments()
{
    var (userId, _) = GetRequesterContext();
    var departments = await _departmentService.GetRegisterableDepartmentsAsync(userId);
    return Ok(ApiResponse<IEnumerable<DepartmentDTO>>.SuccessResponse(
        departments, 
        "Registerable departments retrieved successfully"));
}
```

## API Usage Examples

### 1. Attempt to Register to Public Department (Should Fail)

**Request**:
```http
POST /api/departments/1/register
Authorization: Bearer {member_token}
```

**Response** (400 Bad Request):
```json
{
  "success": false,
  "message": "Public departments are accessible to everyone. No registration required.",
  "errors": null
}
```

### 2. Get Registerable Departments

**Request**:
```http
GET /api/departments/registerable
Authorization: Bearer {member_token}
```

**Response** (200 OK):
```json
{
  "data": [
    {
      "departmentId": 3,
      "name": "Computer Science",
      "description": "CS Department - Advanced Computing",
      "isPublic": false,
      "isUserMember": false,
      "canRegister": true
    },
    {
      "departmentId": 5,
      "name": "Electrical Engineering",
      "description": "EE Department",
      "isPublic": false,
      "isUserMember": false,
      "canRegister": true
    }
  ],
  "message": "Registerable departments retrieved successfully",
  "success": true
}
```

### 3. Successfully Register to Non-Public Department

**Request**:
```http
POST /api/departments/3/register
Authorization: Bearer {member_token}
```

**Response** (200 OK):
```json
{
  "data": {
    "departmentId": 3,
    "userId": 123
  },
  "message": "Registration request submitted. Waiting for approval.",
  "success": true
}
```

## Business Rules

### Public Departments

1. **Automatic Access**: All members automatically have access to public departments
2. **No Registration**: Members cannot register to public departments
3. **Not Listed**: Public departments do NOT appear in the `/registerable` endpoint

### Private Departments (IsPublic = false)

1. **Registration Required**: Members must submit a registration request
2. **Approval Needed**: Lab Manager or Admin must approve the request
3. **Listed**: Only private departments appear in the `/registerable` endpoint
4. **Limit Applies**: Members can only have 2 approved department memberships

### Department Limit

- **Constant**: `MaxDepartmentsPerMember = 2`
- **Count**: Only APPROVED memberships count toward the limit
- **Pending/Rejected**: These statuses do not count toward the limit
- **CanRegister Flag**: Set to false if user already has 2 approved memberships

## Frontend Integration Guide

### Member Department Selection UI

**Scenario**: Member wants to register to a department

**Recommended Flow**:

1. **Fetch Registerable Departments**:
   ```javascript
   const response = await fetch('/api/departments/registerable', {
     headers: { 'Authorization': `Bearer ${token}` }
   });
   const { data: departments } = await response.json();
   ```

2. **Display List**:
   - Show only departments from the response (automatically excludes public ones)
   - Display `CanRegister` flag to indicate if user can still register

3. **Handle Registration Limit**:
   ```javascript
   departments.forEach(dept => {
     if (!dept.canRegister) {
       // Show message: "You have reached the maximum of 2 departments"
       // Disable registration button
     }
   });
   ```

4. **Submit Registration**:
   ```javascript
   const response = await fetch(`/api/departments/${deptId}/register`, {
     method: 'POST',
     headers: { 'Authorization': `Bearer ${token}` }
   });
   
   if (response.ok) {
     // Show success: "Registration submitted. Waiting for approval."
   } else {
     const error = await response.json();
     // Show error message (e.g., if department is public)
   }
   ```

### Department List for Viewing

**Scenario**: Display all departments (public + private)

Use existing endpoint: `GET /api/departments`

This includes both public and private departments with `IsUserMember` flag.

## Testing Scenarios

### Before Migration

⚠️ **Important**: The database must have the `status` column before testing!

**Run Migration**:
```bash
cd LabManagementBackend/LabManagement.DAL
dotnet ef database update --startup-project ../LabManagement.API
```

### Test Cases

1. **Test Public Department Rejection**:
   - Create a public department (`IsPublic = true`)
   - Attempt to register as Member
   - Should receive: "Public departments are accessible to everyone..."

2. **Test Registerable Endpoint**:
   - Create 2 public departments
   - Create 3 private departments
   - Member registers to 1 private department
   - Call `/api/departments/registerable`
   - Should return: 2 private departments (excluding the one already registered)

3. **Test Department Limit**:
   - Member has 2 approved memberships
   - Call `/api/departments/registerable`
   - All departments should have `CanRegister = false`

4. **Test Complete Flow**:
   - Member calls `/api/departments/registerable`
   - Selects a department
   - Submits registration via `/api/departments/{id}/register`
   - Manager approves via `/api/departments/{id}/approve-registration`
   - Member calls `/api/departments/my` to see approved membership

## Database Considerations

### IsPublic Field

**Table**: `departments`

**Column**: `is_public` (bit/boolean)

**Default**: `false` (private by default)

### Demo Data Suggestions

Consider updating demo data to include both public and private departments:

```sql
-- Public Department (accessible to all)
INSERT INTO departments (name, description, is_public) 
VALUES ('General Lab', 'Open to all students', 1);

-- Private Departments (require registration)
INSERT INTO departments (name, description, is_public) 
VALUES 
    ('Computer Science', 'CS Department', 0),
    ('Electrical Engineering', 'EE Department', 0),
    ('Mechanical Engineering', 'ME Department', 0);
```

## Summary

This enhancement provides a clear distinction between public and private departments:

✅ **Public departments**: No registration needed, accessible to all members

✅ **Private departments**: Require registration approval, limited to 2 per member

✅ **New API endpoint**: Returns only departments that require registration

✅ **Better UX**: Members only see departments they can actually register to

✅ **Validation**: Prevents incorrect registration attempts
