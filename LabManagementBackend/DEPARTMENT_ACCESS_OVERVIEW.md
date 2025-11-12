# Department Access & Lab Visibility

This document summarizes the new department model, authorization rules, and request flow introduced to support department-scoped labs.

## 1. Data Model Changes

| Entity | Key Fields | Notes |
| --- | --- | --- |
| `departments` | `department_id`, `name`, `is_public` | Seeded with a default **General** department so existing labs remain accessible. `is_public` determines if everyone can see labs under this department. |
| `user_departments` | `(user_id, department_id)` | Join table that tracks up to 2 department memberships per member. Enforced in `DepartmentService`. |
| `labs` | `department_id` FK | Every lab must belong to exactly one department. Non-admin users only see labs they’re allowed to access (see §3). |

EF artifacts:
- `Department` & `UserDepartment` models under `LabManagement.DAL.Models`.
- DbContext registrations & Fluent config in `LabManagementDbContext`.
- New repositories (`IDepartmentRepository`, `IUserDepartmentRepository`) and wiring inside `UnitOfWork`.
- Migration `20251112043437_AddDepartmentAccessControl` creating tables, seed data, and FK.

## 2. Department APIs & Services

`DepartmentController` (`/api/departments`) exposes CRUD plus membership endpoints:

| Endpoint | Role(s) | Description |
| --- | --- | --- |
| `GET /api/departments` | Any authenticated user | Lists all departments with `IsUserMember` flag for the caller. |
| `GET /api/departments/my` | Any authenticated user | Lists only departments the caller belongs to. |
| `POST /api/departments` | Admin, SchoolManager | Creates a department. |
| `PUT /api/departments/{id}` | Admin, SchoolManager | Updates name/description/visibility. |
| `DELETE /api/departments/{id}` | Admin | Deletes a department (fails if labs still attached). |
| `POST /api/departments/{id}/register` | Member | Registers the caller; fails if over 2 departments or already joined. |
| `DELETE /api/departments/{id}/register?userIdOverride=` | Member (self), Admin/SchoolManager (self or override) | Unregisters a membership. |

Implementation highlights:
- `DepartmentService` enforces unique names, membership cap (`Constant.MaxDepartmentsPerMember = 2`), and ensures only members can self-register.
- AutoMapper profile + DTOs (`DepartmentDTO`, `CreateDepartmentDTO`, `UpdateDepartmentDTO`) map EF entities to transport objects.

## 3. Lab Visibility Logic

`LabService` now requires requester context for read methods:

```csharp
Task<IEnumerable<LabDTO>> GetAllLabsAsync(int requesterId, Constant.UserRole requesterRole);
Task<PagedResult<LabDTO>> GetLabsAsync(QueryParameters query, int requesterId, Constant.UserRole requesterRole);
```

Filtering rules (implemented in `ApplyVisibilityFilter`):
1. **Admin / SchoolManager**: see every lab (no filtering).
2. **LabManager**: see labs they manage + any where their department is public/member accessible.
3. **SecurityLab / Member**: see labs whose department is public *or* they are registered to the department.

`LabController` extracts `UserId` & `Role` claims and passes them to the service for `/api/labs` and `/api/labs/paged`.

## 4. Creating or Updating Labs

- `CreateLabDTO` and `UpdateLabDTO` now require `departmentId`.
- `LabProfile` maps department info into `LabDTO` so callers get `departmentId`, `departmentName`, and `isPublic`.
- When creating/updating, `LabService` validates the provided department exists before persisting changes.

## 5. Ops Checklist

1. Run `dotnet ef database update 20251112043437_AddDepartmentAccessControl`.
2. Seed any additional departments as needed.
3. Update clients to consume the new DTO fields and call department APIs for member registration.
4. Ensure JWT tokens include `UserId` and `Role` claims (already required by controllers).

With these changes in place, members can self-manage department access (up to two departments), and lab listings automatically respect visibility rules. Admins retain full control via the CRUD endpoints.
