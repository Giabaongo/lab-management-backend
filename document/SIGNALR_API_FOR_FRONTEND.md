# 📡 SignalR API Reference - For Frontend Developers

**Base URL**: `http://localhost:5162` (Development)  
**Protocol**: WebSocket with fallback to Server-Sent Events

---

## 🔗 Hub Endpoints

### 1. Booking Hub

**WebSocket URL**: `http://localhost:5162/hubs/booking`

**Methods** (Client → Server):

| Method | Parameters | Type | Description |
|--------|-----------|------|-------------|
| `JoinManagerGroup` | `managerId` | `number` (int) | Join nhóm để nhận booking notifications của manager |
| `LeaveManagerGroup` | `managerId` | `number` (int) | Rời khỏi nhóm manager |

**Events** (Server → Client):

| Event | Data Structure | Trigger |
|-------|---------------|---------|
| `BookingCreated` | `BookingEventData` | Khi có booking mới được tạo |

**BookingEventData Structure**:
```typescript
interface BookingEventData {
  bookingId: number;
  userId: number;
  labId: number;
  zoneId: number;
  startTime: string; // ISO 8601
  endTime: string;   // ISO 8601
  status: number;
  notes: string | null;
  createdAt: string; // ISO 8601
  managerName: string | null;
  labName: string;
  zoneName: string;
}
```

**Example Usage**:
```typescript
const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/booking')
  .build();

connection.on('BookingCreated', (data: BookingEventData) => {
  console.log('New booking:', data);
  // Update UI
});

await connection.start();
await connection.invoke('JoinManagerGroup', 3); // managerId must be number!
```

---

### 2. Equipment Hub

**WebSocket URL**: `http://localhost:5162/hubs/equipment`

**Methods** (Client → Server):

| Method | Parameters | Type | Description |
|--------|-----------|------|-------------|
| `JoinAllManagersGroup` | - | - | Join nhóm tất cả managers (nhận tất cả equipment alerts) |
| `LeaveAllManagersGroup` | - | - | Rời khỏi nhóm all managers |
| `JoinLabGroup` | `labId` | `number` (int) | Join nhóm lab cụ thể |
| `LeaveLabGroup` | `labId` | `number` (int) | Rời khỏi nhóm lab |

**Events** (Server → Client):

| Event | Data Structure | Trigger |
|-------|---------------|---------|
| `EquipmentStatusChanged` | `EquipmentStatusEventData` | Khi equipment status = Broken (2) hoặc Maintenance (3) |

**EquipmentStatusEventData Structure**:
```typescript
interface EquipmentStatusEventData {
  equipmentId: number;
  name: string;
  code: string;
  labId: number;
  status: number;
  statusText: string; // "Broken" hoặc "Maintenance"
  timestamp: string;  // ISO 8601
}
```

**Example Usage**:
```typescript
const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/equipment')
  .build();

connection.on('EquipmentStatusChanged', (data: EquipmentStatusEventData) => {
  if (data.status === 2) {
    // Show urgent alert - equipment broken
    showNotification('Equipment Broken!', data.name, 'error');
  }
});

await connection.start();

// Option 1: Receive all equipment alerts
await connection.invoke('JoinAllManagersGroup');

// Option 2: Receive alerts for specific lab only
await connection.invoke('JoinLabGroup', 1); // labId = 1
```

---

### 3. Security Log Hub

**WebSocket URL**: `http://localhost:5162/hubs/security`

**Methods** (Client → Server):

| Method | Parameters | Type | Description |
|--------|-----------|------|-------------|
| `JoinSecurityTeamGroup` | - | - | Join nhóm security team (nhận tất cả alerts) |
| `LeaveSecurityTeamGroup` | - | - | Rời khỏi nhóm security team |
| `JoinLabSecurityGroup` | `labId` | `number` (int) | Join nhóm security của lab cụ thể |
| `LeaveLabSecurityGroup` | `labId` | `number` (int) | Rời khỏi nhóm security lab |

**Events** (Server → Client):

| Event | Data Structure | Trigger |
|-------|---------------|---------|
| `SecurityAlert` | `SecurityAlertEventData` | Khi có security log mới được tạo |

**SecurityAlertEventData Structure**:
```typescript
interface SecurityAlertEventData {
  logId: number;
  eventId: number | null;
  securityId: number | null;
  actionType: string;
  notes: string | null;
  photoUrl: string | null;
  loggedAt: string; // ISO 8601
  severity: string; // "High", "Medium", "Low"
}
```

**Example Usage**:
```typescript
const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/security')
  .build();

connection.on('SecurityAlert', (data: SecurityAlertEventData) => {
  if (data.severity === 'High') {
    // Show urgent security alert
    showUrgentAlert(data.actionType, data.notes);
  }
});

await connection.start();
await connection.invoke('JoinSecurityTeamGroup');
```

---

### 4. Lab Event Hub

**WebSocket URL**: `http://localhost:5162/hubs/lab-events`

**Methods** (Client → Server):

| Method | Parameters | Type | Description |
|--------|-----------|------|-------------|
| `JoinAllEventsGroup` | - | - | Join nhóm tất cả events |
| `LeaveAllEventsGroup` | - | - | Rời khỏi nhóm all events |
| `SubscribeToLabEvents` | `labId` | `number` (int) | Subscribe events của lab cụ thể |
| `UnsubscribeFromLabEvents` | `labId` | `number` (int) | Unsubscribe events của lab |
| `SubscribeToEvent` | `eventId` | `number` (int) | Subscribe một event cụ thể |
| `UnsubscribeFromEvent` | `eventId` | `number` (int) | Unsubscribe event cụ thể |

**Events** (Server → Client):

| Event | Data Structure | Trigger |
|-------|---------------|---------|
| `NewLabEvent` | `LabEventEventData` | Khi có lab event mới được tạo |

**LabEventEventData Structure**:
```typescript
interface LabEventEventData {
  eventId: number;
  labId: number;
  title: string;
  description: string | null;
  startTime: string; // ISO 8601
  endTime: string;   // ISO 8601
  activityTypeId: number;
}
```

**Example Usage**:
```typescript
const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/lab-events')
  .build();

connection.on('NewLabEvent', (data: LabEventEventData) => {
  // Show notification about new event
  showEventNotification(data.title, data.startTime);
});

await connection.start();

// Option 1: All events
await connection.invoke('JoinAllEventsGroup');

// Option 2: Specific lab events
await connection.invoke('SubscribeToLabEvents', 1);

// Option 3: Specific event updates
await connection.invoke('SubscribeToEvent', 5);
```

---

### 5. Notification Hub

**WebSocket URL**: `http://localhost:5162/hubs/notifications`

**Methods** (Client → Server):

| Method | Parameters | Type | Description |
|--------|-----------|------|-------------|
| `JoinUserGroup` | `userId` | `number` (int) | Join nhóm user để nhận notifications cá nhân |
| `LeaveUserGroup` | `userId` | `number` (int) | Rời khỏi nhóm user |
| `JoinRoleGroup` | `role` | `string` | Join nhóm role (Admin/Manager/Member) |
| `LeaveRoleGroup` | `role` | `string` | Rời khỏi nhóm role |

**Events** (Server → Client):

| Event | Data Structure | Trigger |
|-------|---------------|---------|
| `ReceiveNotification` | `NotificationEventData` | ⚠️ Chưa implement trong controller |

**NotificationEventData Structure** (Draft):
```typescript
interface NotificationEventData {
  notificationId: number;
  userId: number | null;
  role: string | null;
  title: string;
  message: string;
  type: string; // "Info", "Warning", "Error", "Success"
  createdAt: string; // ISO 8601
  isRead: boolean;
}
```

**Example Usage**:
```typescript
const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/notifications')
  .build();

connection.on('ReceiveNotification', (data: NotificationEventData) => {
  // Show toast notification
  showToast(data.title, data.message, data.type);
});

await connection.start();

// Join user-specific notifications
const userId = 123; // from auth context
await connection.invoke('JoinUserGroup', userId);

// Join role-based notifications
await connection.invoke('JoinRoleGroup', 'Manager');
```

---

## 🔐 Authentication (Future)

⚠️ **Hiện tại chưa có authentication** - Hubs đang public.

**Khi implement authentication**, gửi JWT token:

```typescript
const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/booking', {
    accessTokenFactory: () => {
      return localStorage.getItem('jwt_token') || '';
    }
  })
  .build();
```

---

## ⚙️ Connection Configuration

**Recommended Setup**:

```typescript
const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/booking')
  .withAutomaticReconnect() // Auto-reconnect on disconnect
  .configureLogging(signalR.LogLevel.Information)
  .build();

// Handle connection closed
connection.onclose((error) => {
  console.error('Connection closed:', error);
  // Update UI to show "Disconnected" status
});

// Handle reconnecting
connection.onreconnecting((error) => {
  console.warn('Reconnecting...', error);
  // Update UI to show "Reconnecting..." status
});

// Handle reconnected
connection.onreconnected((connectionId) => {
  console.log('Reconnected:', connectionId);
  // Re-join groups if needed
  rejoinGroups();
});
```

---

## 🚨 Important Rules

### ⚠️ Type Safety

**CRITICAL**: All ID parameters MUST be sent as **number** (int), NOT string!

```typescript
// ❌ WRONG - Causes InvalidDataException
await connection.invoke('JoinManagerGroup', '3');
await connection.invoke('JoinLabGroup', '1');

// ✅ CORRECT
await connection.invoke('JoinManagerGroup', 3);
await connection.invoke('JoinLabGroup', 1);

// If you have string, convert first:
const managerId = parseInt(managerIdString);
await connection.invoke('JoinManagerGroup', managerId);
```

### Error Codes

| Error | Cause | Solution |
|-------|-------|----------|
| `InvalidDataException` | Gửi string thay vì int | Convert sang number trước khi invoke |
| `HubException: Invalid ID` | ID <= 0 | Validate ID > 0 trước khi gửi |
| `Connection failed` | API không chạy hoặc CORS | Kiểm tra API đang chạy ở port 5162 |
| `Failed to invoke` | Connection chưa established | Đợi `connection.start()` hoàn thành |

---

## 📋 Quick Integration Checklist

- [ ] Install `@microsoft/signalr` package
- [ ] Create connection với correct hub URL
- [ ] Add `.withAutomaticReconnect()`
- [ ] Register event listeners BEFORE calling `.start()`
- [ ] Handle `onclose`, `onreconnecting`, `onreconnected`
- [ ] Convert all IDs to **number** before invoke
- [ ] Check `connection.state` before invoke methods
- [ ] Cleanup connection on component unmount

---

## 🧪 Testing Endpoints

**Test với Browser Console**:

```javascript
// 1. Create connection
const conn = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/booking')
  .build();

// 2. Listen for events
conn.on('BookingCreated', data => console.log('Event:', data));

// 3. Start
await conn.start();
console.log('State:', conn.state); // Should be "Connected"

// 4. Join group (NUMBER!)
await conn.invoke('JoinManagerGroup', 3);

// 5. Test by creating a booking via API
// Event should fire in console

// 6. Disconnect
await conn.stop();
```

---

## 📊 Hub Status Summary

| Hub | Status | Events Implemented | Controller Integration |
|-----|--------|-------------------|----------------------|
| Booking | ✅ Working | ✅ BookingCreated | ✅ BookingController |
| Equipment | ✅ Working | ✅ EquipmentStatusChanged | ✅ EquipmentController |
| Security | ✅ Working | ✅ SecurityAlert | ✅ SecurityLogController |
| Lab Event | ✅ Working | ✅ NewLabEvent | ✅ LabEventController |
| Notification | ✅ Working | ⚠️ Not implemented yet | ❌ No integration |

---

## 📞 Support

**Documentation**:
- Full guide: `SIGNALR_FRONTEND_INTEGRATION.md`
- Testing guide: `SIGNALR_TESTING_SUMMARY.md`
- Quick test: `SIGNALR_QUICK_TEST.md`

**Test Client**: 
- HTML test client available: `test-all-signalr-hubs.html`
- Open: `http://localhost:8080/test-all-signalr-hubs.html`

**API Logs**:
- Check terminal running API (port 5162)
- SignalR debug logging enabled
- Shows connection/disconnection events
- Shows method invocations

---

## 📅 Last Updated

**Date**: November 13, 2025  
**API Version**: Development  
**SignalR Version**: ASP.NET Core 8.0  
**Client Library**: @microsoft/signalr ^8.0.0
