# Demo Playbook – Lab Management Backend

This guide lists the main flows you can showcase when demoing the Lab Management backend. Each section highlights the persona, endpoints, sample payloads, and the story to tell.

---

## 1. Authentication & Access Control

### 1.1 Email/Password Login
1. **Endpoint:** `POST /api/auth/login`
2. **Payload:**
   ```json
   {
     "email": "admin@lab.local",
     "password": "Pass@123"
   }
   ```
3. **Demo Points:** JWT issuance (`token`, `userId`, `role`), unified `ApiResponse<>` envelope, downstream controllers protected by `[Authorize]` with role filters defined in `LabManagementBackend/LabManagement.API/Controllers/*.cs`.

### 1.2 Google OAuth Login
1. **Start:** `GET /api/auth/google/login?returnUrl=http://localhost:3000/auth/callback`
2. **Flow:** Controller caches `state`, redirects to Google; callback exchanges `code` for tokens, invokes `IGoogleAuthService`, then redirects to the frontend with our JWT in the query string (`GoogleOAuthController.cs`).
3. **Demo Points:** Secure `state` handling, reuse of existing ID-token verification, ability to demo both redirect-based and token-only (`POST /api/auth/google-login`) flows.

---

## 2. User & Role Management (Admin persona)

### 2.1 Browse Users
- `GET /api/users/paged?searchTerm=&pageNumber=1&pageSize=10`
- Show the paged response (`PagedResult<UserDTO>`) and filtering options.

### 2.2 Create / Update Roles
- **Create:** `POST /api/users`
  ```json
  {
    "name": "New Lab Manager",
    "email": "labmanager@lab.local",
    "password": "Secret@123",
    "role": 2
  }
  ```
- **Promote:** `PUT /api/users/{id}/role`
  ```json
  { "role": 3 }
  ```
- Emphasize validation (unique email, role-based authorization via `[Authorize(Roles = ...)]`).

### 2.3 Clean-up
- `DELETE /api/users/{id}` keeps the story tidy for repeated demos.

---

## 3. Lab Discovery & Booking Flow (Member persona)

### 3.1 Discover Labs
- `GET /api/labs/paged?searchTerm=chemistry&pageNumber=1&pageSize=5`
- Shows role-aware filtering (`LabController.GetRequesterContext()` restricts by department/role).

### 3.2 Check Availability
- `GET /api/bookings/available-slots?LabZoneId=1&Date=2025-11-15`
- Returns `AvailableSlotDTO` list to illustrate scheduler logic.

### 3.3 Create Booking
- `POST /api/bookings`
  ```json
  {
    "labZoneId": 1,
    "startTime": "2025-11-15T08:00:00Z",
    "endTime": "2025-11-15T10:00:00Z",
    "purpose": "Undergrad lab session"
  }
  ```
- Highlight validation (`BookingService.CreateBookingAsync` ensures conflicts are prevented) and the consistent success envelope.

### 3.4 Update / Cancel
- `PUT /api/bookings/{id}` to adjust slots.
- `DELETE /api/bookings/{id}` (Admin/SchoolManager) to show governance.
- **Realtime demo:** Spin up a SignalR client connected to `/hubs/booking`, call `JoinManagerGroup(<managerId>)`, then submit a new booking as a member. The Lab Manager’s client receives the `BookingCreated` payload instantly.

---

## 4. Equipment Lifecycle (Lab Manager persona)

### 4.1 Inventory Overview
- `GET /api/equipments` (requires authenticated role).
- `GET /api/equipments/paged?status=Available&sortBy=Name`

### 4.2 Detail & Health Checks
- `GET /api/equipments/{id}`
- `GET /api/equipments/code-exist?code=EQ-001` to demonstrate live validation hooks used by the frontend.

### 4.3 Update / Retire
- `PUT /api/equipments/{id}`
  ```json
  {
    "name": "Spectrometer A",
    "status": 2,
    "location": "Lab 3",
    "conditionNotes": "Requires calibration"
  }
  ```
- `DELETE /api/equipments/{id}` to show audit-friendly deletion response (`ApiResponse<string>`).

### 4.4 Demo Story
1. Show a manager checking stock before a session.
2. Update condition after usage.
3. Run the `status-exist` check to illustrate how the UI prevents invalid states.

---

## 5. Incident Reporting & Resolution (Member → Admin hand-off)

### 5.1 Submit Report
- `POST /api/reports`
  ```json
  {
    "labId": 1,
    "zoneId": 3,
    "issueType": "Equipment Failure",
    "description": "Microscope lamp not working",
    "photoUrl": "https://..."
  }
  ```
- Mention automatic timestamp/user assignment inside `ReportService`.

### 5.2 Track Reports
- `GET /api/reports/paged?sortBy=createdAt&sortOrder=desc`
- `GET /api/reports/lab/{labId}` to show filtered dashboards.

### 5.3 Resolution
- `PUT /api/reports/{id}` with `status`, `resolutionNotes`.
- `DELETE /api/reports/{id}` (Admin/SchoolManager) for duplicates.

### 5.4 Optional Tie-ins
- After resolving, trigger `NotificationController` to inform staff (see next section).

---

## 6. Notifications & Security Visibility

### 6.1 Admin Notifications
- `POST /api/notifications`
  ```json
  {
    "title": "Maintenance Window",
    "message": "Lab 2 will be offline tomorrow 8-10AM",
    "targetRole": 4
  }
  ```
- `GET /api/notifications/paged` to show targeted broadcast history.
- `PATCH /api/notifications/{id}/mark-as-read` demonstrates acknowledgement tracking.

### 6.2 Security Logs (if needed)
- `GET /api/security-logs/paged` (controller exists in the project) to prove that high-risk operations are audited.

---

## 7. Putting It All Together (Suggested Demo Narrative)

1. **Login** as Admin via email/password, then show Google OAuth as an alternative.
2. **Create** a Lab Manager user, then switch context (using issued JWT) to perform member-facing tasks.
3. **Browse labs**, check availability, book a session, and update/cancel it to show lifecycle management.
4. **Adjust equipment** status to prepare the lab, using validation endpoints.
5. **Submit** an incident report during the session; **resolve** it as Admin and push a notification to stakeholders.
6. **Highlight logs/notifications** to close the loop and emphasize compliance + communication.

This sequence can be delivered in ~15 minutes while touching every core subsystem (auth, labs, bookings, equipment, reports, notifications). Customize payloads or IDs to match your seeded data.
