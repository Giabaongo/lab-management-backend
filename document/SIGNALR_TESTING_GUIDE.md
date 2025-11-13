# SignalR Booking Hub - Testing Guide

## 📋 Overview
SignalR implementation for real-time booking notifications to lab managers.

## 🏗️ Architecture

### Hub Location
- **Path**: `/hubs/booking`
- **Full URL**: `http://localhost:5162/hubs/booking`
- **Class**: `BookingHub` in `LabManagement.API/Hubs/BookingHub.cs`

### Components

1. **BookingHub** (SignalR Hub)
   - Methods:
     - `JoinManagerGroup(int managerId)` - Subscribe to notifications for a manager
     - `LeaveManagerGroup(int managerId)` - Unsubscribe from notifications
   - Group naming: `manager:{managerId}`

2. **BookingController** 
   - Injects `IHubContext<BookingHub>`
   - Sends notification when booking is created
   - Event: `BookingCreated`

## 🧪 Testing Steps

### Step 1: Start the API
```bash
cd LabManagementBackend
dotnet run --project LabManagement.API
```
API should be running on: `http://localhost:5162`

### Step 2: Open Test Client
1. Open browser and navigate to: `http://localhost:8080/test-signalr.html`
2. Or open the HTML file directly in browser

### Step 3: Connect to Hub
1. Click **"🔌 Connect to Hub"** button
2. Wait for green status: **"✅ Connected"**
3. Check the log for connection success message

### Step 4: Join Manager Group
1. Enter a Manager ID (e.g., `1` - this should match a real manager ID in your database)
2. Click **"➕ Join Manager Group"**
3. Log should show: `✅ Joined manager group: 1`

### Step 5: Test Notification
Create a booking through the API that belongs to a lab managed by the manager ID you joined:

**Using curl:**
```bash
curl -X POST http://localhost:5162/api/bookings \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "labId": 1,
    "zoneId": 1,
    "startTime": "2025-11-14T10:00:00",
    "endTime": "2025-11-14T12:00:00",
    "activityTypeId": 1,
    "note": "SignalR test booking"
  }'
```

**Using Swagger:**
1. Navigate to: `http://localhost:5162/swagger`
2. Authorize with JWT token
3. POST to `/api/bookings`
4. Fill in the booking details

### Step 6: Verify Notification
In the test client, you should see:
```
🎉 BOOKING CREATED!
   📌 Booking ID: 123
   🏢 Lab: Computer Lab A (ID: 1)
   📍 Zone ID: 1
   🕐 Start: 11/14/2025, 10:00:00 AM
   🕐 End: 11/14/2025, 12:00:00 PM
   👤 Requested by User ID: 5
```

## 📡 Event Schema

### BookingCreated Event
```javascript
{
  bookingId: number,
  labId: number,
  labName: string,
  zoneId: number,
  startTime: string (ISO 8601),
  endTime: string (ISO 8601),
  requestedByUserId: number
}
```

## 🔧 Troubleshooting

### Connection Failed
**Problem**: Cannot connect to SignalR hub

**Solutions**:
1. Check API is running: `http://localhost:5162`
2. Verify CORS is configured correctly in `Program.cs`
3. Check browser console for errors
4. Ensure port 5162 is not blocked

### Not Receiving Notifications
**Problem**: Connected but no notifications appear

**Checklist**:
1. ✅ Joined the correct manager group?
2. ✅ Manager ID matches the lab's manager?
3. ✅ Created booking for a lab managed by that manager?
4. ✅ Booking creation was successful (check API response)?

**Verify Manager ID**:
```sql
-- Find which manager manages which lab
SELECT l.labId, l.labName, l.managerId, u.name as ManagerName
FROM labs l
JOIN users u ON l.managerId = u.userId
WHERE l.labId = 1;
```

### CORS Errors
**Problem**: CORS policy blocking SignalR

**Current Config in Program.cs**:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:3000",
                  "http://localhost:8080",
                  "http://127.0.0.1:8080"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();  // Required for SignalR
    });
});
```

If testing from different origin, add it to `WithOrigins()`.

## 🎯 Testing Scenarios

### Scenario 1: Single Manager
1. Connect to hub
2. Join manager group (e.g., Manager ID = 1)
3. Create booking for Lab 1 (managed by Manager 1)
4. ✅ Should receive notification

### Scenario 2: Multiple Managers
1. Open test client in 2 browser tabs
2. Tab 1: Join Manager ID = 1
3. Tab 2: Join Manager ID = 2
4. Create booking for Lab 1 (Manager 1)
5. ✅ Tab 1 should receive notification
6. ❌ Tab 2 should NOT receive notification

### Scenario 3: Reconnection
1. Connect and join manager group
2. Stop API server
3. Test client should show "Reconnecting..."
4. Restart API server
5. ✅ Should auto-reconnect
6. ⚠️ Need to rejoin group after reconnection

## 🚀 Frontend Integration Example

### JavaScript/TypeScript
```javascript
import * as signalR from '@microsoft/signalr';

// Create connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5162/hubs/booking')
    .withAutomaticReconnect()
    .build();

// Listen for booking events
connection.on('BookingCreated', (data) => {
    console.log('New booking:', data);
    // Update UI, show notification, etc.
    showNotification(`New booking for ${data.labName}`);
});

// Start connection
await connection.start();

// Join manager group (get managerId from logged-in user)
const managerId = getCurrentUser().managerId;
await connection.invoke('JoinManagerGroup', managerId);

// Later, when leaving page or logging out
await connection.invoke('LeaveManagerGroup', managerId);
await connection.stop();
```

### React Hook
```javascript
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export function useBookingNotifications(managerId) {
    const [notification, setNotification] = useState(null);

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:5162/hubs/booking')
            .withAutomaticReconnect()
            .build();

        connection.on('BookingCreated', (data) => {
            setNotification(data);
        });

        connection.start()
            .then(() => connection.invoke('JoinManagerGroup', managerId))
            .catch(err => console.error('SignalR error:', err));

        return () => {
            connection.invoke('LeaveManagerGroup', managerId);
            connection.stop();
        };
    }, [managerId]);

    return notification;
}
```

## 📝 Notes

1. **Authentication**: Current implementation doesn't require authentication for SignalR connection. Consider adding `[Authorize]` attribute to `BookingHub` in production.

2. **Group Management**: Clients must explicitly join groups. Consider auto-joining based on JWT claims.

3. **Notification Types**: Currently only `BookingCreated` is implemented. Can extend with:
   - `BookingUpdated`
   - `BookingCancelled`
   - `BookingApproved`

4. **Scalability**: For production with multiple servers, use SignalR backplane (Redis, Azure SignalR Service).

## ✅ Success Criteria

- [x] SignalR hub accessible at `/hubs/booking`
- [x] Clients can connect successfully
- [x] Clients can join/leave manager groups
- [x] Booking creation triggers notification
- [x] Only managers of the lab receive notification
- [x] Notification contains complete booking details
- [x] Auto-reconnection works
- [x] CORS configured for frontend

## 🔗 Resources

- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr)
- [JavaScript Client](https://www.npmjs.com/package/@microsoft/signalr)
- Test Client: `test-signalr.html`
