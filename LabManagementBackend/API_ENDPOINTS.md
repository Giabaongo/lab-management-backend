# API Endpoints Reference

## 🏥 Health Check
```
GET    /api/health                     → Check API health status
```

## 🔐 Authentication
```
POST   /api/auth/login                 → Login with email & password
```

## 🧪 Examples (Development/Testing Only)
```
GET    /api/examples/success           → Example success response
GET    /api/examples/not-found/{id}    → Example 404 NotFoundException
POST   /api/examples/bad-request       → Example 400 BadRequestException
GET    /api/examples/unauthorized      → Example 401 UnauthorizedException
GET    /api/examples/error             → Example 500 Internal Server Error
GET    /api/examples/manual-error      → Example manual error response
```

---

## 👥 Users (`/api/users`)
```
GET    /api/users                      → Get all users
GET    /api/users/{id}                 → Get user by ID
GET    /api/users/by-email             → Get user by email (query: ?email=xxx)
POST   /api/users                      → Create new user
PUT    /api/users/{id}                 → Update user
PUT    /api/users/{id}/role            → Update user role (Admin only)
DELETE /api/users/{id}                 → Delete user
HEAD   /api/users/{id}                 → Check if user exists
HEAD   /api/users/check-email          → Check if email exists (query: ?email=xxx)
```

---

## 🏢 Labs (`/api/labs`)
```
GET    /api/labs                       → Get all labs
GET    /api/labs/{id}                  → Get lab by ID
POST   /api/labs                       → Create new lab
PUT    /api/labs/{id}                  → Update lab
DELETE /api/labs/{id}                  → Delete lab
```

---

## 🗺️ Lab Zones (`/api/lab-zones`)
```
GET    /api/lab-zones                  → Get all lab zones
GET    /api/lab-zones/{id}             → Get lab zone by ID
POST   /api/lab-zones                  → Create new lab zone
PUT    /api/lab-zones/{id}             → Update lab zone
DELETE /api/lab-zones/{id}             → Delete lab zone
```

---

## 📅 Bookings (`/api/bookings`)
```
GET    /api/bookings                   → Get all bookings
GET    /api/bookings/{id}              → Get booking by ID
POST   /api/bookings                   → Create new booking
PUT    /api/bookings/{id}              → Update booking
DELETE /api/bookings/{id}              → Delete booking
```

---

## 🔬 Equipment (`/api/equipment`)
```
GET    /api/equipment                  → Get all equipment
GET    /api/equipment/{id}             → Get equipment by ID
PUT    /api/equipment/{id}             → Update equipment
DELETE /api/equipment/{id}             → Delete equipment
HEAD   /api/equipment/check-code       → Check if code exists (query: ?code=xxx)
```

---

## 🎯 Activity Types (`/api/activity-types`)
```
GET    /api/activity-types             → Get all activity types
GET    /api/activity-types/{id}        → Get activity type by ID
POST   /api/activity-types             → Create new activity type
PUT    /api/activity-types/{id}        → Update activity type
DELETE /api/activity-types/{id}        → Delete activity type
```

---

## 📆 Lab Events (`/api/lab-events`)
```
GET    /api/lab-events                 → Get all lab events
GET    /api/lab-events/{id}            → Get lab event by ID
POST   /api/lab-events                 → Create new lab event
PUT    /api/lab-events/{id}            → Update lab event
DELETE /api/lab-events/{id}            → Delete lab event
```

---

## 🔒 Security Logs (`/api/security-logs`)
```
GET    /api/security-logs              → Get all security logs
GET    /api/security-logs/{id}         → Get security log by ID
POST   /api/security-logs              → Create new security log
PUT    /api/security-logs/{id}         → Update security log
DELETE /api/security-logs/{id}         → Delete security log
```

---

## 📊 Summary

**Total Endpoints:** 50+

**Base URL (Development):** `http://localhost:5162`

**Base URL (Production):** `https://bao-sql-server.database.windows.net`

**Authentication:** Bearer Token (JWT)

**Content-Type:** `application/json`

**Response Format:** 
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... }
}
```

---

## 🔑 Authentication Example

```bash
# 1. Login
curl -X POST "http://localhost:5162/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@lab.com",
    "password": "your-password"
  }'

# Response:
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "user": {
      "userId": 1,
      "email": "admin@lab.com",
      "name": "Admin User",
      "role": 0
    }
  }
}

# 2. Use token in subsequent requests
curl -X GET "http://localhost:5162/api/users" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..."
```

---

## 📱 Quick Examples

### Get all users
```bash
GET /api/users
Authorization: Bearer {token}
```

### Create a booking
```bash
POST /api/bookings
Authorization: Bearer {token}
Content-Type: application/json

{
  "userId": 1,
  "labId": 2,
  "zoneId": 3,
  "startTime": "2025-10-27T10:00:00",
  "endTime": "2025-10-27T12:00:00",
  "status": 0,
  "notes": "Research session"
}
```

### Update user role
```bash
PUT /api/users/5/role
Authorization: Bearer {token}
Content-Type: application/json

{
  "role": 2
}
```

### Check if email exists
```bash
HEAD /api/users/check-email?email=test@lab.com
Authorization: Bearer {token}

# Returns: 200 OK if exists, 404 Not Found if not
```

---

**Last Updated:** October 27, 2025
