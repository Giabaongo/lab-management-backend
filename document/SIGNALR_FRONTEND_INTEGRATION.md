# SignalR Frontend Integration Guide

## ⚠️ QUAN TRỌNG: Type Matching

SignalR **KHÔNG HỖ TRỢ** method overloading và **NGHIÊM NGẶT** về kiểu dữ liệu!

### ❌ SAI - Frontend gửi String
```javascript
// LỖI: Gửi managerId dưới dạng string sẽ bị InvalidDataException
await connection.invoke('JoinManagerGroup', '3');
```

### ✅ ĐÚNG - Frontend gửi Number
```javascript
// ĐÚNG: Gửi managerId dưới dạng number
await connection.invoke('JoinManagerGroup', 3);

// Hoặc nếu có biến string, convert trước:
const managerIdString = '3';
const managerId = parseInt(managerIdString);
await connection.invoke('JoinManagerGroup', managerId);
```

---

## 📋 Danh Sách Hubs

### 1. 📅 Booking Hub
**URL:** `http://localhost:5162/hubs/booking`

**Methods:**
- `JoinManagerGroup(int managerId)` - Tham gia nhóm manager để nhận thông báo booking
- `LeaveManagerGroup(int managerId)` - Rời khỏi nhóm manager

**Events:**
- `BookingCreated` - Phát ra khi có booking mới được tạo

**Example:**
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5162/hubs/booking')
    .build();

// Listen for events
connection.on('BookingCreated', (data) => {
    console.log('New booking:', data);
    // data structure:
    // {
    //   bookingId, userId, labId, zoneId,
    //   startTime, endTime, status, notes,
    //   createdAt, managerName, labName, zoneName
    // }
});

// Start connection
await connection.start();
console.log('Connected to Booking Hub');

// Join manager group (MUST send as number!)
const managerId = 3; // Number, not string!
await connection.invoke('JoinManagerGroup', managerId);
```

---

### 2. 🔧 Equipment Hub
**URL:** `http://localhost:5162/hubs/equipment`

**Methods:**
- `JoinAllManagersGroup()` - Tham gia nhóm tất cả managers
- `LeaveAllManagersGroup()` - Rời khỏi nhóm tất cả managers
- `JoinLabGroup(int labId)` - Tham gia nhóm lab cụ thể
- `LeaveLabGroup(int labId)` - Rời khỏi nhóm lab

**Events:**
- `EquipmentStatusChanged` - Phát ra khi trạng thái thiết bị thay đổi (Broken/Maintenance)

**Example:**
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5162/hubs/equipment')
    .build();

connection.on('EquipmentStatusChanged', (data) => {
    console.log('Equipment status changed:', data);
    // data: { equipmentId, name, code, labId, status, statusText, timestamp }
});

await connection.start();

// Join all managers group
await connection.invoke('JoinAllManagersGroup');

// Or join specific lab group (MUST send labId as number!)
const labId = 1;
await connection.invoke('JoinLabGroup', labId);
```

---

### 3. 🔒 Security Log Hub
**URL:** `http://localhost:5162/hubs/security`

**Methods:**
- `JoinSecurityTeamGroup()` - Tham gia nhóm security team
- `LeaveSecurityTeamGroup()` - Rời khỏi nhóm security team
- `JoinLabSecurityGroup(int labId)` - Tham gia nhóm bảo vệ lab cụ thể
- `LeaveLabSecurityGroup(int labId)` - Rời khỏi nhóm bảo vệ lab

**Events:**
- `SecurityAlert` - Phát ra khi có security log mới

**Example:**
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5162/hubs/security')
    .build();

connection.on('SecurityAlert', (data) => {
    console.log('Security alert:', data);
    // data: { logId, eventId, securityId, actionType, notes, photoUrl, loggedAt, severity }
});

await connection.start();

// Join security team
await connection.invoke('JoinSecurityTeamGroup');

// Join specific lab security
const labId = 1;
await connection.invoke('JoinLabSecurityGroup', labId);
```

---

### 4. 🎯 Lab Event Hub
**URL:** `http://localhost:5162/hubs/lab-events`

**Methods:**
- `JoinAllEventsGroup()` - Tham gia nhóm tất cả events
- `LeaveAllEventsGroup()` - Rời khỏi nhóm tất cả events
- `SubscribeToLabEvents(int labId)` - Theo dõi events của lab cụ thể
- `UnsubscribeFromLabEvents(int labId)` - Hủy theo dõi events của lab
- `SubscribeToEvent(int eventId)` - Theo dõi event cụ thể
- `UnsubscribeFromEvent(int eventId)` - Hủy theo dõi event cụ thể

**Events:**
- `NewLabEvent` - Phát ra khi có lab event mới

**Example:**
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5162/hubs/lab-events')
    .build();

connection.on('NewLabEvent', (data) => {
    console.log('New lab event:', data);
    // data: { eventId, labId, title, description, startTime, endTime, activityTypeId }
});

await connection.start();

// Join all events
await connection.invoke('JoinAllEventsGroup');

// Subscribe to specific lab events
const labId = 1;
await connection.invoke('SubscribeToLabEvents', labId);

// Subscribe to specific event
const eventId = 5;
await connection.invoke('SubscribeToEvent', eventId);
```

---

### 5. 🔔 Notification Hub
**URL:** `http://localhost:5162/hubs/notifications`

**Methods:**
- `JoinUserGroup(int userId)` - Tham gia nhóm user để nhận thông báo cá nhân
- `LeaveUserGroup(int userId)` - Rời khỏi nhóm user
- `JoinRoleGroup(string role)` - Tham gia nhóm role
- `LeaveRoleGroup(string role)` - Rời khỏi nhóm role

**Events:**
- `ReceiveNotification` - Nhận thông báo (chưa implement trong controller)

**Example:**
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5162/hubs/notifications')
    .build();

connection.on('ReceiveNotification', (data) => {
    console.log('Notification:', data);
});

await connection.start();

// Join user group (MUST send userId as number!)
const userId = 123;
await connection.invoke('JoinUserGroup', userId);

// Join role group (string is OK here)
await connection.invoke('JoinRoleGroup', 'Manager');
```

---

## 🔧 Common Pattern - React/Vue/Angular

### React Hook Example
```typescript
import { useEffect, useRef, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export function useBookingHub(managerId: number) {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [connected, setConnected] = useState(false);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:5162/hubs/booking')
            .withAutomaticReconnect()
            .build();

        newConnection.on('BookingCreated', (data) => {
            console.log('New booking:', data);
            // Update your state here
        });

        newConnection.start()
            .then(() => {
                console.log('Connected to Booking Hub');
                setConnected(true);
                // IMPORTANT: Send managerId as number!
                return newConnection.invoke('JoinManagerGroup', managerId);
            })
            .then(() => {
                console.log('Joined manager group:', managerId);
            })
            .catch(err => console.error('Connection failed:', err));

        setConnection(newConnection);

        return () => {
            newConnection.stop();
        };
    }, [managerId]);

    return { connection, connected };
}
```

### Vue 3 Composable Example
```typescript
import { ref, onMounted, onUnmounted } from 'vue';
import * as signalR from '@microsoft/signalr';

export function useEquipmentHub(labId: number) {
    const connection = ref<signalR.HubConnection | null>(null);
    const connected = ref(false);

    onMounted(async () => {
        connection.value = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:5162/hubs/equipment')
            .withAutomaticReconnect()
            .build();

        connection.value.on('EquipmentStatusChanged', (data) => {
            console.log('Equipment status changed:', data);
            // Update your state
        });

        try {
            await connection.value.start();
            console.log('Connected to Equipment Hub');
            connected.value = true;

            // Join lab group (MUST be number!)
            await connection.value.invoke('JoinLabGroup', labId);
            console.log('Joined lab group:', labId);
        } catch (err) {
            console.error('Connection failed:', err);
        }
    });

    onUnmounted(() => {
        connection.value?.stop();
    });

    return { connection, connected };
}
```

---

## 🐛 Common Errors & Solutions

### Error 1: InvalidDataException
```
System.IO.InvalidDataException: Error binding arguments. 
Make sure that the types of the provided values match the types of the hub method being invoked.
```

**Cause:** Frontend gửi string `'3'` nhưng backend cần number `3`

**Solution:**
```javascript
// ❌ Wrong
await connection.invoke('JoinManagerGroup', '3');

// ✅ Correct
await connection.invoke('JoinManagerGroup', 3);
// Or
const id = parseInt('3');
await connection.invoke('JoinManagerGroup', id);
```

### Error 2: Failed to invoke 'JoinManagerGroup'
**Cause:** Connection chưa được establish hoặc đã disconnect

**Solution:**
```javascript
// Check connection state before invoke
if (connection.state === signalR.HubConnectionState.Connected) {
    await connection.invoke('JoinManagerGroup', managerId);
} else {
    console.error('Connection not established');
}
```

### Error 3: Connection stopped during negotiation
**Cause:** CORS issue hoặc API không chạy

**Solution:**
1. Kiểm tra API đã chạy tại `http://localhost:5162`
2. Kiểm tra CORS trong `Program.cs` đã allow origin của frontend
3. Kiểm tra browser console để xem chi tiết lỗi

---

## 📊 Testing với Browser Console

```javascript
// 1. Create connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5162/hubs/booking')
    .configureLogging(signalR.LogLevel.Debug)
    .build();

// 2. Listen for events
connection.on('BookingCreated', (data) => {
    console.log('Booking created:', data);
});

// 3. Start connection
await connection.start();
console.log('Connected!');

// 4. Join group (NUMBER only!)
await connection.invoke('JoinManagerGroup', 3);
console.log('Joined manager group 3');

// 5. Test by creating a booking via API
// The BookingCreated event should fire

// 6. Leave group
await connection.invoke('LeaveManagerGroup', 3);

// 7. Disconnect
await connection.stop();
```

---

## 🔐 Authentication (Future Enhancement)

Hiện tại các hubs **CHƯA CÓ** authentication. Để thêm authentication:

1. Thêm `[Authorize]` attribute vào hub class
2. Gửi JWT token khi kết nối:

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5162/hubs/booking', {
        accessTokenFactory: () => {
            return localStorage.getItem('jwt_token');
        }
    })
    .build();
```

3. Access user info trong hub:
```csharp
var userId = Context.User?.FindFirst("UserId")?.Value;
var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
```

---

## 📝 Summary Checklist

- [ ] **LUÔN** gửi ID dưới dạng **number** (int), KHÔNG phải string
- [ ] Kiểm tra `connection.state` trước khi invoke method
- [ ] Sử dụng `.withAutomaticReconnect()` để tự động reconnect
- [ ] Listen events **TRƯỚC KHI** start connection
- [ ] Handle `onclose` event để update UI state
- [ ] Test connection với browser console trước khi integrate
- [ ] Check API logs trong terminal để debug
- [ ] Cleanup connection trong `useEffect` return / `onUnmounted`

---

## 🚀 Quick Start Commands

```bash
# Start API
cd LabManagementBackend/LabManagement.API
dotnet run

# Start test client
cd ../..
python3 -m http.server 8080
# Open http://localhost:8080/test-all-signalr-hubs.html
```

---

## 📞 Support

Nếu gặp lỗi, check:
1. Browser console (F12)
2. API terminal logs
3. Network tab trong DevTools
4. Kiểm tra type của tham số (number vs string)
