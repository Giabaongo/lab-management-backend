# 🚀 SignalR Quick Reference - Cheat Sheet

## 📡 Hub URLs (Copy & Paste)

```javascript
const SIGNALR_HUBS = {
  booking: 'http://localhost:5162/hubs/booking',
  equipment: 'http://localhost:5162/hubs/equipment',
  security: 'http://localhost:5162/hubs/security',
  labEvents: 'http://localhost:5162/hubs/lab-events',
  notifications: 'http://localhost:5162/hubs/notifications'
};
```

---

## 🎯 Booking Hub

**URL**: `http://localhost:5162/hubs/booking`

| Method | Params | Event | Data |
|--------|--------|-------|------|
| `JoinManagerGroup` | `managerId: number` | `BookingCreated` | bookingId, userId, labId, zoneId, startTime, endTime, status, notes, managerName, labName, zoneName |
| `LeaveManagerGroup` | `managerId: number` | - | - |

**Code**:
```javascript
connection.on('BookingCreated', data => console.log(data));
await connection.invoke('JoinManagerGroup', 3);
```

---

## 🔧 Equipment Hub

**URL**: `http://localhost:5162/hubs/equipment`

| Method | Params | Event | Trigger |
|--------|--------|-------|---------|
| `JoinAllManagersGroup` | - | `EquipmentStatusChanged` | Equipment status = Broken/Maintenance |
| `JoinLabGroup` | `labId: number` | `EquipmentStatusChanged` | Equipment status = Broken/Maintenance |

**Code**:
```javascript
connection.on('EquipmentStatusChanged', data => {
  // data: equipmentId, name, code, labId, status, statusText, timestamp
  if (data.status === 2) alert('Equipment Broken!');
});
await connection.invoke('JoinAllManagersGroup');
```

---

## 🔒 Security Hub

**URL**: `http://localhost:5162/hubs/security`

| Method | Params | Event | Trigger |
|--------|--------|-------|---------|
| `JoinSecurityTeamGroup` | - | `SecurityAlert` | New security log created |
| `JoinLabSecurityGroup` | `labId: number` | `SecurityAlert` | New security log created |

**Code**:
```javascript
connection.on('SecurityAlert', data => {
  // data: logId, eventId, securityId, actionType, notes, severity
  showAlert(data.actionType, data.severity);
});
await connection.invoke('JoinSecurityTeamGroup');
```

---

## 🎯 Lab Event Hub

**URL**: `http://localhost:5162/hubs/lab-events`

| Method | Params | Event | Trigger |
|--------|--------|-------|---------|
| `JoinAllEventsGroup` | - | `NewLabEvent` | New lab event created |
| `SubscribeToLabEvents` | `labId: number` | `NewLabEvent` | New lab event created |
| `SubscribeToEvent` | `eventId: number` | `NewLabEvent` | New lab event created |

**Code**:
```javascript
connection.on('NewLabEvent', data => {
  // data: eventId, labId, title, description, startTime, endTime
  showNotification(data.title, data.startTime);
});
await connection.invoke('JoinAllEventsGroup');
```

---

## 🔔 Notification Hub

**URL**: `http://localhost:5162/hubs/notifications`

| Method | Params | Event | Status |
|--------|--------|-------|--------|
| `JoinUserGroup` | `userId: number` | `ReceiveNotification` | ⚠️ Not implemented |
| `JoinRoleGroup` | `role: string` | `ReceiveNotification` | ⚠️ Not implemented |

**Code**:
```javascript
connection.on('ReceiveNotification', data => {
  // data: notificationId, title, message, type
  showToast(data.title, data.message);
});
await connection.invoke('JoinUserGroup', 123);
```

---

## ⚡ Setup Template

```javascript
import * as signalR from '@microsoft/signalr';

// 1. Create connection
const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/booking')
  .withAutomaticReconnect()
  .configureLogging(signalR.LogLevel.Information)
  .build();

// 2. Register events (BEFORE start!)
connection.on('BookingCreated', (data) => {
  console.log('Event received:', data);
});

// 3. Connection lifecycle
connection.onclose((error) => {
  console.error('Disconnected:', error);
});

connection.onreconnecting((error) => {
  console.warn('Reconnecting...', error);
});

connection.onreconnected((connectionId) => {
  console.log('Reconnected:', connectionId);
  // Re-join groups
});

// 4. Start connection
await connection.start();
console.log('Connected!');

// 5. Join groups (IDs must be NUMBER!)
await connection.invoke('JoinManagerGroup', 3);

// 6. Cleanup on unmount
// connection.stop();
```

---

## ⚠️ Critical Rules

### Type Safety

```javascript
// ❌ WRONG - Causes InvalidDataException
await connection.invoke('JoinManagerGroup', '3');
await connection.invoke('JoinLabGroup', '1');

// ✅ CORRECT - Send as number
await connection.invoke('JoinManagerGroup', 3);
await connection.invoke('JoinLabGroup', 1);

// If you have string, convert:
const id = parseInt(idString);
await connection.invoke('JoinManagerGroup', id);
```

### Check Connection State

```javascript
if (connection.state === signalR.HubConnectionState.Connected) {
  await connection.invoke('JoinManagerGroup', managerId);
} else {
  console.error('Not connected');
}
```

---

## 📦 TypeScript Interfaces

```typescript
interface BookingEventData {
  bookingId: number;
  userId: number;
  labId: number;
  zoneId: number;
  startTime: string;
  endTime: string;
  status: number;
  notes: string | null;
  createdAt: string;
  managerName: string | null;
  labName: string;
  zoneName: string;
}

interface EquipmentStatusEventData {
  equipmentId: number;
  name: string;
  code: string;
  labId: number;
  status: number; // 2=Broken, 3=Maintenance
  statusText: string;
  timestamp: string;
}

interface SecurityAlertEventData {
  logId: number;
  eventId: number | null;
  securityId: number | null;
  actionType: string;
  notes: string | null;
  photoUrl: string | null;
  loggedAt: string;
  severity: string; // "High", "Medium", "Low"
}

interface LabEventEventData {
  eventId: number;
  labId: number;
  title: string;
  description: string | null;
  startTime: string;
  endTime: string;
  activityTypeId: number;
}
```

---

## 🧪 Quick Test (Browser Console)

```javascript
// 1. Load SignalR
const script = document.createElement('script');
script.src = 'https://cdn.jsdelivr.net/npm/@microsoft/signalr@8.0.0/dist/browser/signalr.min.js';
document.head.appendChild(script);

// 2. Create connection (after script loads)
const conn = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/booking')
  .build();

// 3. Listen
conn.on('BookingCreated', d => console.log('📅', d));

// 4. Start
await conn.start();

// 5. Join
await conn.invoke('JoinManagerGroup', 3);

// Check state
conn.state // Should be "Connected"
```

---

## 🎯 Common Use Cases

### Manager Dashboard - Monitor All Bookings

```javascript
const bookingConn = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/booking')
  .withAutomaticReconnect()
  .build();

bookingConn.on('BookingCreated', (booking) => {
  // Add to dashboard
  addBookingToList(booking);
  showNotification(`New booking by ${booking.managerName}`);
});

await bookingConn.start();
await bookingConn.invoke('JoinManagerGroup', currentUser.id);
```

### Equipment Monitoring - All Labs

```javascript
const equipmentConn = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/equipment')
  .withAutomaticReconnect()
  .build();

equipmentConn.on('EquipmentStatusChanged', (equipment) => {
  if (equipment.status === 2) { // Broken
    showUrgentAlert(`${equipment.name} is BROKEN!`);
  } else if (equipment.status === 3) { // Maintenance
    showWarning(`${equipment.name} needs maintenance`);
  }
});

await equipmentConn.start();
await equipmentConn.invoke('JoinAllManagersGroup');
```

### Security Dashboard - Monitor All Alerts

```javascript
const securityConn = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/security')
  .withAutomaticReconnect()
  .build();

securityConn.on('SecurityAlert', (alert) => {
  if (alert.severity === 'High') {
    playAlertSound();
    showRedAlert(alert.actionType);
  }
  addToSecurityLog(alert);
});

await securityConn.start();
await securityConn.invoke('JoinSecurityTeamGroup');
```

### Event Calendar - Lab Specific

```javascript
const eventConn = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5162/hubs/lab-events')
  .withAutomaticReconnect()
  .build();

eventConn.on('NewLabEvent', (event) => {
  addEventToCalendar(event);
  showNotification(`New event: ${event.title}`);
});

await eventConn.start();
await eventConn.invoke('SubscribeToLabEvents', currentLabId);
```

---

## 📊 Hub Status

| Hub | Status | Events Working | Integration |
|-----|--------|---------------|-------------|
| Booking | ✅ | ✅ | ✅ |
| Equipment | ✅ | ✅ | ✅ |
| Security | ✅ | ✅ | ✅ |
| Lab Event | ✅ | ✅ | ✅ |
| Notification | ✅ | ⚠️ | ❌ |

---

## 📚 Resources

- **Full API Docs**: `SIGNALR_API_FOR_FRONTEND.md`
- **Integration Guide**: `SIGNALR_FRONTEND_INTEGRATION.md`
- **JSON Spec**: `signalr-api-spec.json`
- **Test Client**: `http://localhost:8080/test-all-signalr-hubs.html`

---

## 🆘 Common Errors

| Error | Fix |
|-------|-----|
| InvalidDataException | Send number, not string: `invoke('Join', 3)` not `'3'` |
| Connection failed | Check API running at port 5162 |
| Failed to invoke | Wait for `await connection.start()` |
| Hub not found | Check URL: `/hubs/booking` not `/hub/booking` |

---

**Last Updated**: November 13, 2025  
**API**: http://localhost:5162  
**SignalR Version**: 8.0.0
