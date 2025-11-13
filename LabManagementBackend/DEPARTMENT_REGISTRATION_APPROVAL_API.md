# Department Registration Approval Flow - API Documentation

## Overview

Starting from this version, when a Member registers to a department, the registration requires approval from Admin or SchoolManager. The registration goes through 3 states:

- **Pending (0)**: Registration request submitted, waiting for approval
- **Approved (1)**: Registration approved, user is now a member
- **Rejected (2)**: Registration rejected

## Registration Status Enum

```csharp
public enum RegistrationStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
```

---

## API Endpoints

### 1. Register to Department (Member)

**Endpoint:** `POST /api/departments/{id}/register`

**Authorization:** Member role required

**Description:** Submit a registration request to join a department. The request will be in "Pending" status.

**Request:**
```http
POST /api/departments/5/register
Authorization: Bearer <token>
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": {
    "departmentId": 5
  },
  "message": "Registration request submitted successfully. Waiting for approval."
}
```

**Response (Error - 400 Bad Request):**
```json
{
  "success": false,
  "message": "You already have a pending registration request for this department"
}
```

```json
{
  "success": false,
  "message": "You are already a member of this department"
}
```

```json
{
  "success": false,
  "message": "Your previous registration request was rejected. Please contact an administrator."
}
```

```json
{
  "success": false,
  "message": "Members can only be part of 2 departments"
}
```

---

### 2. Get Pending Registration Requests (Admin/SchoolManager)

**Endpoint:** `GET /api/departments/{id}/pending-registrations`

**Authorization:** Admin or SchoolManager role required

**Description:** Get all pending registration requests for a specific department.

**Request:**
```http
GET /api/departments/5/pending-registrations
Authorization: Bearer <token>
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "userId": 12,
      "userName": "John Doe",
      "email": "john.doe@university.edu",
      "departmentId": 5,
      "departmentName": "Computer Science",
      "createdAt": "2025-11-14T08:30:00Z",
      "status": 0,
      "statusText": "Pending"
    },
    {
      "userId": 15,
      "userName": "Jane Smith",
      "email": "jane.smith@university.edu",
      "departmentId": 5,
      "departmentName": "Computer Science",
      "createdAt": "2025-11-14T09:15:00Z",
      "status": 0,
      "statusText": "Pending"
    }
  ],
  "message": "Pending registrations retrieved successfully"
}
```

---

### 3. Approve or Reject Registration (Admin/SchoolManager)

**Endpoint:** `POST /api/departments/{id}/approve-registration`

**Authorization:** Admin or SchoolManager role required

**Description:** Approve or reject a pending registration request.

**Request Body:**
```json
{
  "userId": 12,
  "approve": true
}
```

- `userId` (int, required): The ID of the user whose registration to process
- `approve` (boolean, required): `true` to approve, `false` to reject

**Request (Approve):**
```http
POST /api/departments/5/approve-registration
Authorization: Bearer <token>
Content-Type: application/json

{
  "userId": 12,
  "approve": true
}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": {
    "departmentId": 5,
    "userId": 12,
    "approved": true
  },
  "message": "Registration approved successfully"
}
```

**Request (Reject):**
```http
POST /api/departments/5/approve-registration
Authorization: Bearer <token>
Content-Type: application/json

{
  "userId": 12,
  "approve": false
}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": {
    "departmentId": 5,
    "userId": 12,
    "approved": false
  },
  "message": "Registration rejected successfully"
}
```

**Response (Error - 400 Bad Request):**
```json
{
  "success": false,
  "message": "This registration request has already been processed"
}
```

```json
{
  "success": false,
  "message": "User already has 2 approved department memberships"
}
```

**Response (Error - 404 Not Found):**
```json
{
  "success": false,
  "message": "Registration request not found"
}
```

---

### 4. Get My Departments (Member)

**Endpoint:** `GET /api/departments/my`

**Authorization:** Any authenticated user

**Description:** Get all departments the current user is a **member** of (only returns approved memberships).

**Request:**
```http
GET /api/departments/my
Authorization: Bearer <token>
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "departmentId": 5,
      "name": "Computer Science",
      "description": "Faculty of Computer Science",
      "isPublic": true,
      "isUserMember": true
    }
  ],
  "message": "Member departments retrieved successfully"
}
```

**Note:** Only returns departments with `status = 1` (Approved).

---

### 5. Unregister from Department

**Endpoint:** `DELETE /api/departments/{id}/register`

**Authorization:** Member, Admin, or SchoolManager

**Description:** Remove membership from a department.

**Query Parameters:**
- `userIdOverride` (int, optional): Only Admin/SchoolManager can override to remove other users

**Request (Member removing self):**
```http
DELETE /api/departments/5/register
Authorization: Bearer <token>
```

**Request (Admin removing another user):**
```http
DELETE /api/departments/5/register?userIdOverride=12
Authorization: Bearer <token>
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": {
    "departmentId": 5,
    "userId": 12
  },
  "message": "Unregistered from department successfully"
}
```

---

## Frontend Implementation Examples

### React Example - Register to Department

```tsx
import { useState } from 'react';
import { toast } from 'react-hot-toast';

const RegisterButton = ({ departmentId }: { departmentId: number }) => {
  const [loading, setLoading] = useState(false);

  const handleRegister = async () => {
    setLoading(true);
    try {
      const response = await fetch(`/api/departments/${departmentId}/register`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      const data = await response.json();

      if (data.success) {
        toast.success(data.message); // "Registration request submitted successfully. Waiting for approval."
      } else {
        toast.error(data.message);
      }
    } catch (error) {
      toast.error('Registration failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <button onClick={handleRegister} disabled={loading}>
      {loading ? 'Submitting...' : 'Request to Join'}
    </button>
  );
};
```

### React Example - View Pending Requests (Admin)

```tsx
import { useEffect, useState } from 'react';

interface PendingRegistration {
  userId: number;
  userName: string;
  email: string;
  departmentId: number;
  departmentName: string;
  createdAt: string;
  status: number;
  statusText: string;
}

const PendingRegistrations = ({ departmentId }: { departmentId: number }) => {
  const [requests, setRequests] = useState<PendingRegistration[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchPendingRequests();
  }, [departmentId]);

  const fetchPendingRequests = async () => {
    try {
      const response = await fetch(
        `/api/departments/${departmentId}/pending-registrations`,
        {
          headers: {
            'Authorization': `Bearer ${token}`,
          },
        }
      );

      const data = await response.json();
      if (data.success) {
        setRequests(data.data);
      }
    } catch (error) {
      console.error('Failed to fetch pending requests', error);
    } finally {
      setLoading(false);
    }
  };

  const handleApprove = async (userId: number) => {
    try {
      const response = await fetch(
        `/api/departments/${departmentId}/approve-registration`,
        {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            userId,
            approve: true,
          }),
        }
      );

      const data = await response.json();
      if (data.success) {
        toast.success(data.message);
        fetchPendingRequests(); // Refresh list
      } else {
        toast.error(data.message);
      }
    } catch (error) {
      toast.error('Failed to approve registration');
    }
  };

  const handleReject = async (userId: number) => {
    try {
      const response = await fetch(
        `/api/departments/${departmentId}/approve-registration`,
        {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            userId,
            approve: false,
          }),
        }
      );

      const data = await response.json();
      if (data.success) {
        toast.success(data.message);
        fetchPendingRequests(); // Refresh list
      } else {
        toast.error(data.message);
      }
    } catch (error) {
      toast.error('Failed to reject registration');
    }
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div>
      <h2>Pending Registrations</h2>
      {requests.length === 0 ? (
        <p>No pending registrations</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Requested At</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {requests.map((request) => (
              <tr key={request.userId}>
                <td>{request.userName}</td>
                <td>{request.email}</td>
                <td>{new Date(request.createdAt).toLocaleString()}</td>
                <td>
                  <button onClick={() => handleApprove(request.userId)}>
                    Approve
                  </button>
                  <button onClick={() => handleReject(request.userId)}>
                    Reject
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};
```

### Vue Example - Register to Department

```vue
<script setup lang="ts">
import { ref } from 'vue';
import { useToast } from 'vue-toastification';

const props = defineProps<{
  departmentId: number;
}>();

const toast = useToast();
const loading = ref(false);

const handleRegister = async () => {
  loading.value = true;
  try {
    const response = await fetch(`/api/departments/${props.departmentId}/register`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    const data = await response.json();

    if (data.success) {
      toast.success(data.message);
    } else {
      toast.error(data.message);
    }
  } catch (error) {
    toast.error('Registration failed');
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <button @click="handleRegister" :disabled="loading">
    {{ loading ? 'Submitting...' : 'Request to Join' }}
  </button>
</template>
```

---

## User Flow Diagram

```
[Member] --1. POST /departments/{id}/register--> [Backend]
                                                      |
                                                      v
                                            [Create Pending Request]
                                                      |
                                                      v
                                            [Notify Admin/SchoolManager]
                                                      |
                                                      v
[Admin] --2. GET /departments/{id}/pending-registrations--> [Backend]
                                                      |
                                                      v
                                            [Return Pending List]
                                                      |
                                                      v
[Admin] --3. POST /departments/{id}/approve-registration--> [Backend]
              (approve: true/false)                   |
                                                      v
                                      [Update Status to Approved/Rejected]
                                                      |
                                                      v
                                            [Notify Member]
```

---

## Important Notes

1. **Only approved memberships count** towards the 2-department limit
2. **Pending requests don't show** in "My Departments" list
3. **Rejected registrations** cannot be re-submitted (user must contact admin)
4. **Admin/SchoolManager** can view and manage all pending requests
5. **Members** can only see their approved departments

---

## Migration Required

Run this migration before using the new endpoints:

```bash
cd LabManagement.DAL
dotnet ef migrations add AddRegistrationStatusToUserDepartments --startup-project ../LabManagement.API
dotnet ef database update --startup-project ../LabManagement.API
```

---

**Last Updated:** November 14, 2025  
**API Version:** v1.0  
**Backend:** .NET 8.0
