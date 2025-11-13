# 🧪 SignalR Quick Testing Checklist

## ✅ Đã Hoàn Thành

### Infrastructure
- [x] 5 SignalR Hubs created (Booking, Equipment, Security, LabEvent, Notification)
- [x] All hubs configured in Program.cs
- [x] Debug logging enabled
- [x] CORS configured for localhost origins
- [x] Error handling added to all hubs

### Controller Integration
- [x] BookingController → triggers BookingCreated event
- [x] EquipmentController → triggers EquipmentStatusChanged event
- [x] SecurityLogController → triggers SecurityAlert event
- [x] LabEventController → triggers NewLabEvent event
- [ ] NotificationHub - chưa integrate vào controller nào

### Bug Fixes
- [x] Fixed method overloading crash (Exit Code 134)
- [x] Removed duplicate method definitions
- [x] Documented type requirements (int vs string)
- [x] BookingController userId handling fixed

### Testing Tools
- [x] `test-all-signalr-hubs.html` - Interactive test client
- [x] `test-signalr-events.sh` - Automated event triggers
- [x] HTTP server setup for testing

### Documentation
- [x] `SIGNALR_FRONTEND_INTEGRATION.md` - Complete guide
- [x] `SIGNALR_TESTING_SUMMARY.md` - Testing results
- [x] React/Vue/Angular examples
- [x] Common errors & solutions

---

## 🎯 Cách Test Ngay Bây Giờ

### Option 1: Test Client HTML (Recommended)

```bash
# Terminal 1: API đang chạy ở port 5162
cd LabManagementBackend/LabManagement.API
dotnet run

# Terminal 2: Start HTTP server
cd ../..
python3 -m http.server 8080

# Open browser:
http://localhost:8080/test-all-signalr-hubs.html

# Click "Test All Hubs Automatically" button
```

**Expected Results:**
- ✅ All 5 hubs show "Connected" status (green)
- ✅ Each hub joins its default group successfully
- ✅ Event log shows connection messages

---

### Option 2: Browser Console Testing

```javascript
// Open http://localhost:8080/test-all-signalr-hubs.html
// Press F12, go to Console, run:

// 1. Booking Hub - Check if connected
bookingConnection.state
// Should be: "Connected"

// 2. Try join manager group
await bookingConnection.invoke('JoinManagerGroup', 3)
// Check API logs for: "Client xxx successfully joined group: manager:3"

// 3. Equipment Hub
equipmentConnection.state
await equipmentConnection.invoke('JoinAllManagersGroup')
// Check API logs for join confirmation

// 4. Security Hub
securityConnection.state
await securityConnection.invoke('JoinSecurityTeamGroup')

// 5. Lab Event Hub
labEventConnection.state
await labEventConnection.invoke('JoinAllEventsGroup')

// 6. Notification Hub
notificationConnection.state
await notificationConnection.invoke('JoinUserGroup', 1)
```

---

### Option 3: Trigger Events via API

```bash
# Equipment Status Change (triggers EquipmentStatusChanged)
curl -X PUT "http://localhost:5162/api/equipment/1" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Equipment",
    "code": "EQ-001",
    "description": "Test",
    "status": 2,
    "labId": 1
  }'

# Security Log (triggers SecurityAlert)
curl -X POST "http://localhost:5162/api/securitylogs" \
  -H "Content-Type: application/json" \
  -d '{
    "eventId": 1,
    "securityId": 1,
    "actionType": "Test Alert",
    "notes": "Testing SignalR",
    "loggedAt": "'$(date -u +"%Y-%m-%dT%H:%M:%S")'"
  }'

# Lab Event (triggers NewLabEvent)
curl -X POST "http://localhost:5162/api/labevents" \
  -H "Content-Type: application/json" \
  -d '{
    "labId": 1,
    "activityTypeId": 1,
    "title": "Test Event",
    "description": "Testing SignalR",
    "startTime": "'$(date -u -d '+1 day' +"%Y-%m-%dT%H:%M:%S")'",
    "endTime": "'$(date -u -d '+1 day +2 hours' +"%Y-%m-%dT%H:%M:%S")'"
  }'
```

---

## 🔍 Kiểm Tra API Logs

Mở terminal đang chạy API (port 5162), check for:

### Successful Connection
```
info: LabManagement.API.Hubs.BookingHub[0]
      Client connected: [ConnectionId]
```

### Successful Group Join
```
info: LabManagement.API.Hubs.BookingHub[0]
      Client [ConnectionId] successfully joined group: manager:3
```

### Event Triggered
```
dbug: Microsoft.AspNetCore.SignalR.Internal.DefaultHubDispatcher[1]
      Sending hub method 'BookingCreated' to client [ConnectionId]
```

### Errors to Look For
```
❌ InvalidDataException: Error binding arguments
   → Frontend gửi string thay vì int

❌ NotSupportedException: Duplicate definitions
   → Method overloading (đã fix)

❌ HubException: Invalid manager ID
   → managerId <= 0
```

---

## ⚠️ Common Issues & Quick Fixes

### Issue 1: "Error binding arguments"
**Cause:** Frontend gửi `'3'` (string) thay vì `3` (number)

**Fix:**
```javascript
// ❌ Wrong
await connection.invoke('JoinManagerGroup', '3');

// ✅ Correct
await connection.invoke('JoinManagerGroup', 3);
```

---

### Issue 2: "Connection failed"
**Cause:** API không chạy hoặc CORS issue

**Fix:**
```bash
# Check API đang chạy
curl http://localhost:5162/api/labs

# Restart API nếu cần
cd LabManagementBackend/LabManagement.API
dotnet run
```

---

### Issue 3: "Failed to invoke method"
**Cause:** Connection chưa established

**Fix:**
```javascript
// Check connection state trước
if (connection.state === 'Connected') {
    await connection.invoke('JoinManagerGroup', 3);
}
```

---

## 📊 Expected Test Results

### All Hubs Connected
```
✅ Booking Hub: Connected
✅ Equipment Hub: Connected
✅ Security Hub: Connected
✅ Lab Event Hub: Connected
✅ Notification Hub: Connected
```

### Groups Joined
```
✅ manager:3 (Booking)
✅ all-managers (Equipment)
✅ security-team (Security)
✅ all-events (Lab Event)
✅ user:1 (Notification)
```

### Events Received (when triggered)
```
✅ BookingCreated - when create booking
✅ EquipmentStatusChanged - when update to Broken/Maintenance
✅ SecurityAlert - when create security log
✅ NewLabEvent - when create lab event
⚠️ ReceiveNotification - not implemented yet
```

---

## 🎯 Success Criteria

**Minimum**: 
- [ ] All 5 hubs connect successfully
- [ ] At least 1 join method works per hub
- [ ] No crashes or exceptions

**Ideal**:
- [x] All hubs connect ✅
- [x] All join/leave methods work ✅
- [x] 4/5 events triggering correctly ✅
- [x] Type safety validated ✅
- [x] Error handling works ✅
- [x] Logging clear and helpful ✅

---

## 📝 Frontend Integration Steps

1. **Install SignalR Package**
   ```bash
   npm install @microsoft/signalr
   ```

2. **Read Documentation**
   - `document/SIGNALR_FRONTEND_INTEGRATION.md`

3. **Use Test Client as Reference**
   - `test-all-signalr-hubs.html`

4. **Important Rules**
   - ⚠️ ALWAYS send IDs as **number** (int), NOT string
   - ✅ Check connection state before invoke
   - ✅ Use `.withAutomaticReconnect()`
   - ✅ Handle `onclose` event

5. **Example Code**
   ```typescript
   const connection = new signalR.HubConnectionBuilder()
       .withUrl('http://localhost:5162/hubs/booking')
       .withAutomaticReconnect()
       .build();

   connection.on('BookingCreated', (data) => {
       console.log('New booking:', data);
   });

   await connection.start();
   await connection.invoke('JoinManagerGroup', 3); // NUMBER!
   ```

---

## ✅ Final Status

**Overall**: 🎉 **EXCELLENT - Ready for Frontend Integration**

**Hubs**: 5/5 Working ✅
**Events**: 4/5 Implemented ✅
**Documentation**: Complete ✅
**Testing Tools**: Available ✅
**Type Safety**: Documented ✅

**Next Steps**:
1. Frontend tích hợp SignalR theo docs
2. Thêm authentication vào hubs (production)
3. Implement NotificationHub events
4. Create test users in database
