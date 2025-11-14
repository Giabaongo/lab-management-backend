# Real-Time Notifications – Complete SignalR Guide

This document provides a comprehensive guide for integrating all SignalR hubs in the Lab Management System. The backend exposes 4 real-time hubs for different notification types.

## 📡 Available Hubs

| Hub | Endpoint | Purpose | Target Users |
|-----|----------|---------|--------------|
| **BookingHub** | `/hubs/booking` | Booking notifications | Lab Managers |
| **LabEventHub** | `/hubs/lab-events` | Lab event updates | All users, Lab-specific subscribers |
| **EquipmentHub** | `/hubs/equipment` | Equipment status changes | All Managers, Lab-specific managers |
| **SecurityLogHub** | `/hubs/security` | Security alerts | Security Team (Admin, SchoolManager, SecurityLab) |

---

## 1️⃣ BookingHub - Booking Notifications

### Overview
Notifies lab managers when new bookings are created for their labs.

### Endpoint
```
/hubs/booking
```

### Groups
- `manager:{managerId}` - Notifications for specific lab manager

### Client Methods to Listen

#### `BookingCreated`
Triggered when a member creates a new booking.

**Payload:**
```json
{
  "bookingId": 12,
  "labId": 3,
  "labName": "Chemistry Lab",
  "zoneId": 2,
  "startTime": "2025-11-20T08:00:00Z",
  "endTime": "2025-11-20T10:00:00Z",
  "requestedByUserId": 42
}
```

### Server Methods to Call

#### `JoinManagerGroup(managerId: number)`
Subscribe to booking notifications for a specific manager.

#### `LeaveManagerGroup(managerId: number)`
Unsubscribe from manager's booking notifications.

### React Example

```tsx
import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from 'react-hot-toast';

export function useBookingNotifications(
  apiBaseUrl: string, 
  token: string, 
  managerId: number
) {
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${apiBaseUrl}/hubs/booking`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.on('BookingCreated', (payload) => {
      toast.success(
        `New booking for ${payload.labName}!\n` +
        `Time: ${new Date(payload.startTime).toLocaleString()}`,
        { duration: 5000 }
      );
    });

    connection.start()
      .then(() => connection.invoke('JoinManagerGroup', managerId))
      .catch(console.error);

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [apiBaseUrl, token, managerId]);
}
```

### Vue Example

```ts
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { onMounted, onBeforeUnmount } from 'vue';
import { useToast } from 'vue-toastification';

export function useBookingNotifications(
  apiBaseUrl: string, 
  token: string, 
  managerId: number
) {
  const toast = useToast();
  const connection = new HubConnectionBuilder()
    .withUrl(`${apiBaseUrl}/hubs/booking`, {
      accessTokenFactory: () => token
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();

  onMounted(async () => {
    connection.on('BookingCreated', (payload) => {
      toast.success(
        `New booking for ${payload.labName}! Time: ${new Date(payload.startTime).toLocaleString()}`
      );
    });
    
    await connection.start();
    await connection.invoke('JoinManagerGroup', managerId);
  });

  onBeforeUnmount(() => {
    connection.stop();
  });
}
```

---

## 2️⃣ LabEventHub - Lab Event Updates

### Overview
Notifies users about new lab events created in the system.

### Endpoint
```
/hubs/lab-events
```

### Groups
- `all-events` - All lab events across the system
- `lab-events:{labId}` - Events for a specific lab
- `event:{eventId}` - Subscribers to a specific event

### Client Methods to Listen

#### `NewLabEvent`
Triggered when a new lab event is created.

**Payload:**
```json
{
  "eventId": 5,
  "labId": 3,
  "title": "AI Workshop",
  "description": "Introduction to Machine Learning",
  "startTime": "2025-12-01T14:00:00Z",
  "endTime": "2025-12-01T17:00:00Z",
  "activityTypeId": 2
}
```

### Server Methods to Call

#### `JoinAllEventsGroup()`
Subscribe to all lab events.

#### `LeaveAllEventsGroup()`
Unsubscribe from all events.

#### `SubscribeToLabEvents(labId: number)`
Subscribe to events for a specific lab.

#### `UnsubscribeFromLabEvents(labId: number)`
Unsubscribe from lab's events.

#### `SubscribeToEvent(eventId: number)`
Subscribe to a specific event (for participants).

#### `UnsubscribeFromEvent(eventId: number)`
Unsubscribe from specific event.

### React Example

```tsx
import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from 'react-hot-toast';

export function useLabEventNotifications(
  apiBaseUrl: string,
  token: string,
  labId?: number // Optional: subscribe to specific lab
) {
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${apiBaseUrl}/hubs/lab-events`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.on('NewLabEvent', (payload) => {
      toast.info(
        `📅 New Event: ${payload.title}\n` +
        `Lab: ${payload.labId} | ${new Date(payload.startTime).toLocaleDateString()}`,
        { duration: 6000 }
      );
    });

    connection.start()
      .then(() => {
        if (labId) {
          return connection.invoke('SubscribeToLabEvents', labId);
        } else {
          return connection.invoke('JoinAllEventsGroup');
        }
      })
      .catch(console.error);

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [apiBaseUrl, token, labId]);
}
```

### Vue Example

```ts
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { onMounted, onBeforeUnmount, ref } from 'vue';
import { useToast } from 'vue-toastification';

export function useLabEventNotifications(
  apiBaseUrl: string,
  token: string,
  labId?: number
) {
  const toast = useToast();
  const connection = new HubConnectionBuilder()
    .withUrl(`${apiBaseUrl}/hubs/lab-events`, {
      accessTokenFactory: () => token
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();

  onMounted(async () => {
    connection.on('NewLabEvent', (payload) => {
      toast.info(
        `📅 ${payload.title} - ${new Date(payload.startTime).toLocaleDateString()}`
      );
    });

    await connection.start();
    
    if (labId) {
      await connection.invoke('SubscribeToLabEvents', labId);
    } else {
      await connection.invoke('JoinAllEventsGroup');
    }
  });

  onBeforeUnmount(() => {
    connection.stop();
  });
}
```

---

## 3️⃣ EquipmentHub - Equipment Status Updates

### Overview
Notifies managers when equipment status changes (broken, under maintenance).

### Endpoint
```
/hubs/equipment
```

### Groups
- `all-managers` - All managers receive all equipment updates
- `lab-manager:{labId}` - Managers of a specific lab

### Client Methods to Listen

#### `EquipmentStatusChanged`
Triggered when equipment status changes to broken/maintenance.

**Payload:**
```json
{
  "equipmentId": 8,
  "equipmentName": "Oscilloscope",
  "equipmentCode": "OSC-001",
  "labId": 5,
  "status": 2,
  "statusText": "Broken",
  "timestamp": "2025-11-14T10:30:00Z"
}
```

**Status Values:**
- `1` = Available
- `2` = Broken
- `3` = Under Maintenance
- `4` = Inactive

### Server Methods to Call

#### `JoinAllManagersGroup()`
Subscribe to all equipment status changes (for admins/school managers).

#### `LeaveAllManagersGroup()`
Unsubscribe from all equipment updates.

#### `JoinLabManagerGroup(labId: number)`
Subscribe to equipment updates for a specific lab.

#### `LeaveLabManagerGroup(labId: number)`
Unsubscribe from lab's equipment updates.

### React Example

```tsx
import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from 'react-hot-toast';

export function useEquipmentNotifications(
  apiBaseUrl: string,
  token: string,
  isAllManager: boolean = false,
  labId?: number
) {
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${apiBaseUrl}/hubs/equipment`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.on('EquipmentStatusChanged', (payload) => {
      const statusColor = payload.status === 2 ? '🔴' : '⚠️';
      toast.error(
        `${statusColor} Equipment Alert!\n` +
        `${payload.equipmentName} (${payload.equipmentCode})\n` +
        `Status: ${payload.statusText}`,
        { duration: 8000 }
      );
    });

    connection.start()
      .then(() => {
        if (isAllManager) {
          return connection.invoke('JoinAllManagersGroup');
        } else if (labId) {
          return connection.invoke('JoinLabManagerGroup', labId);
        }
      })
      .catch(console.error);

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [apiBaseUrl, token, isAllManager, labId]);
}
```

### Vue Example

```ts
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { onMounted, onBeforeUnmount } from 'vue';
import { useToast } from 'vue-toastification';

export function useEquipmentNotifications(
  apiBaseUrl: string,
  token: string,
  isAllManager: boolean = false,
  labId?: number
) {
  const toast = useToast();
  const connection = new HubConnectionBuilder()
    .withUrl(`${apiBaseUrl}/hubs/equipment`, {
      accessTokenFactory: () => token
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();

  onMounted(async () => {
    connection.on('EquipmentStatusChanged', (payload) => {
      const statusIcon = payload.status === 2 ? '🔴' : '⚠️';
      toast.error(
        `${statusIcon} ${payload.equipmentName}: ${payload.statusText}`
      );
    });

    await connection.start();
    
    if (isAllManager) {
      await connection.invoke('JoinAllManagersGroup');
    } else if (labId) {
      await connection.invoke('JoinLabManagerGroup', labId);
    }
  });

  onBeforeUnmount(() => {
    connection.stop();
  });
}
```

---

## 4️⃣ SecurityLogHub - Security Alerts

### Overview
Notifies security team (Admin, SchoolManager, SecurityLab) about security events.

### Endpoint
```
/hubs/security
```

### Groups
- `security-team` - All security personnel
- `lab-security:{labId}` - Security alerts for a specific lab

### Client Methods to Listen

#### `SecurityAlert`
Triggered when a security log is created.

**Payload:**
```json
{
  "logId": 15,
  "eventId": 8,
  "securityId": 3,
  "actionType": 2,
  "notes": "Unauthorized access attempt",
  "photoUrl": "https://example.com/security/photo.jpg",
  "loggedAt": "2025-11-14T10:45:00Z",
  "severity": "Critical"
}
```

**Severity Levels:**
- `"Warning"` - Minor security events
- `"Critical"` - Serious security breaches

### Server Methods to Call

#### `JoinSecurityTeamGroup()`
Subscribe to all security alerts (for Admin/SchoolManager/SecurityLab).

#### `LeaveSecurityTeamGroup()`
Unsubscribe from security alerts.

#### `JoinLabSecurityGroup(labId: number)`
Subscribe to security alerts for a specific lab.

#### `LeaveLabSecurityGroup(labId: number)`
Unsubscribe from lab's security alerts.

### React Example

```tsx
import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from 'react-hot-toast';

export function useSecurityNotifications(
  apiBaseUrl: string,
  token: string,
  labId?: number
) {
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${apiBaseUrl}/hubs/security`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.on('SecurityAlert', (payload) => {
      const isCritical = payload.severity === 'Critical';
      
      const toastFn = isCritical ? toast.error : toast.warning;
      toastFn(
        `🚨 Security Alert!\n` +
        `${payload.notes}\n` +
        `Severity: ${payload.severity}`,
        { 
          duration: isCritical ? 10000 : 6000,
          style: {
            background: isCritical ? '#DC2626' : '#F59E0B',
            color: 'white'
          }
        }
      );
    });

    connection.start()
      .then(() => {
        if (labId) {
          return connection.invoke('JoinLabSecurityGroup', labId);
        } else {
          return connection.invoke('JoinSecurityTeamGroup');
        }
      })
      .catch(console.error);

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [apiBaseUrl, token, labId]);
}
```

### Vue Example

```ts
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { onMounted, onBeforeUnmount } from 'vue';
import { useToast } from 'vue-toastification';

export function useSecurityNotifications(
  apiBaseUrl: string,
  token: string,
  labId?: number
) {
  const toast = useToast();
  const connection = new HubConnectionBuilder()
    .withUrl(`${apiBaseUrl}/hubs/security`, {
      accessTokenFactory: () => token
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();

  onMounted(async () => {
    connection.on('SecurityAlert', (payload) => {
      const isCritical = payload.severity === 'Critical';
      
      if (isCritical) {
        toast.error(`🚨 ${payload.notes} - ${payload.severity}`);
      } else {
        toast.warning(`⚠️ ${payload.notes}`);
      }
    });

    await connection.start();
    
    if (labId) {
      await connection.invoke('JoinLabSecurityGroup', labId);
    } else {
      await connection.invoke('JoinSecurityTeamGroup');
    }
  });

  onBeforeUnmount(() => {
    connection.stop();
  });
}
```

---

## 🎯 Complete React Integration Example

Combine all hubs in a single component based on user role:

```tsx
import { useEffect } from 'react';
import { useBookingNotifications } from './hooks/useBookingNotifications';
import { useLabEventNotifications } from './hooks/useLabEventNotifications';
import { useEquipmentNotifications } from './hooks/useEquipmentNotifications';
import { useSecurityNotifications } from './hooks/useSecurityNotifications';

interface RealtimeProviderProps {
  apiBaseUrl: string;
  token: string;
  userRole: 'Admin' | 'SchoolManager' | 'LabManager' | 'SecurityLab' | 'Member';
  userId: number;
  managerId?: number;
  labId?: number;
}

export const RealtimeProvider: React.FC<RealtimeProviderProps> = ({
  apiBaseUrl,
  token,
  userRole,
  userId,
  managerId,
  labId
}) => {
  // Booking notifications (for Lab Managers)
  if (userRole === 'LabManager' && managerId) {
    useBookingNotifications(apiBaseUrl, token, managerId);
  }

  // Lab events (for all users)
  useLabEventNotifications(apiBaseUrl, token, labId);

  // Equipment notifications (for Managers)
  if (['Admin', 'SchoolManager'].includes(userRole)) {
    useEquipmentNotifications(apiBaseUrl, token, true); // All equipment
  } else if (userRole === 'LabManager' && labId) {
    useEquipmentNotifications(apiBaseUrl, token, false, labId); // Specific lab
  }

  // Security alerts (for security personnel)
  if (['Admin', 'SchoolManager', 'SecurityLab'].includes(userRole)) {
    useSecurityNotifications(apiBaseUrl, token, labId);
  }

  return null; // This component just manages connections
};
```

Usage:

```tsx
function App() {
  return (
    <>
      <RealtimeProvider
        apiBaseUrl="https://api.example.com"
        token={authToken}
        userRole="LabManager"
        userId={currentUser.id}
        managerId={currentUser.managerId}
        labId={currentUser.labId}
      />
      <YourAppContent />
    </>
  );
}
```

---

## 🎯 Complete Vue Integration Example

```vue
<script setup lang="ts">
import { computed } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useBookingNotifications } from '@/composables/useBookingNotifications';
import { useLabEventNotifications } from '@/composables/useLabEventNotifications';
import { useEquipmentNotifications } from '@/composables/useEquipmentNotifications';
import { useSecurityNotifications } from '@/composables/useSecurityNotifications';

const authStore = useAuthStore();
const API_URL = import.meta.env.VITE_API_URL;

const isLabManager = computed(() => authStore.user?.role === 'LabManager');
const isManager = computed(() => 
  ['Admin', 'SchoolManager'].includes(authStore.user?.role || '')
);
const isSecurityTeam = computed(() => 
  ['Admin', 'SchoolManager', 'SecurityLab'].includes(authStore.user?.role || '')
);

// Booking notifications (for Lab Managers)
if (isLabManager.value && authStore.user?.managerId) {
  useBookingNotifications(
    API_URL,
    authStore.token,
    authStore.user.managerId
  );
}

// Lab events (for all users)
useLabEventNotifications(API_URL, authStore.token, authStore.user?.labId);

// Equipment notifications (for Managers)
if (isManager.value) {
  useEquipmentNotifications(API_URL, authStore.token, true);
} else if (isLabManager.value && authStore.user?.labId) {
  useEquipmentNotifications(API_URL, authStore.token, false, authStore.user.labId);
}

// Security alerts (for security personnel)
if (isSecurityTeam.value) {
  useSecurityNotifications(API_URL, authStore.token, authStore.user?.labId);
}
</script>

<template>
  <!-- Your app content -->
</template>
```

---

## 🔐 Security & Best Practices

### 1. Authentication
All hubs require JWT authentication via `accessTokenFactory`:

```ts
.withUrl(`${apiBaseUrl}/hubs/booking`, {
  accessTokenFactory: () => token
})
```

### 2. Error Handling

```ts
connection.on('error', (error) => {
  console.error('SignalR error:', error);
  toast.error('Connection error. Please refresh the page.');
});
```

### 3. Reconnection Strategy

```ts
.withAutomaticReconnect({
  nextRetryDelayInMilliseconds: (retryContext) => {
    // Exponential backoff
    return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
  }
})
```

### 4. Connection State Management

```tsx
const [connectionState, setConnectionState] = useState<'Connected' | 'Disconnected' | 'Reconnecting'>('Disconnected');

connection.onreconnecting(() => setConnectionState('Reconnecting'));
connection.onreconnected(() => setConnectionState('Connected'));
connection.onclose(() => setConnectionState('Disconnected'));
```

### 5. Clean Up

Always stop connections when components unmount:

```ts
return () => {
  connection.stop();
};
```

---

## 🐛 Troubleshooting

### 401 Unauthorized
- Verify JWT token is not expired
- Check `accessTokenFactory` returns valid token
- Ensure user has correct role permissions

### No Messages Received
- Confirm you've called the correct `Join` method
- Check group name matches backend logic
- Verify connection state is `Connected`
- Check browser console for SignalR logs

### CORS Issues
- Backend must configure CORS for SignalR:
```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});
```

### Connection Drops
- Check network stability
- Verify `withAutomaticReconnect()` is configured
- Monitor server-side logs for exceptions

---

## 📊 Hub Summary Table

| Hub | Event | Group | Payload Fields |
|-----|-------|-------|----------------|
| **BookingHub** | `BookingCreated` | `manager:{id}` | bookingId, labId, labName, zoneId, startTime, endTime, requestedByUserId |
| **LabEventHub** | `NewLabEvent` | `all-events`, `lab-events:{id}` | eventId, labId, title, description, startTime, endTime, activityTypeId |
| **EquipmentHub** | `EquipmentStatusChanged` | `all-managers`, `lab-manager:{id}` | equipmentId, equipmentName, equipmentCode, labId, status, statusText, timestamp |
| **SecurityLogHub** | `SecurityAlert` | `security-team`, `lab-security:{id}` | logId, eventId, securityId, actionType, notes, photoUrl, loggedAt, severity |

---

## 🎨 Toast Styling Recommendations

### React (react-hot-toast)

```tsx
// Success - Booking
toast.success('Message', {
  icon: '📅',
  duration: 5000,
  style: { background: '#10B981', color: 'white' }
});

// Info - Lab Event
toast('Message', {
  icon: '📣',
  duration: 6000,
  style: { background: '#3B82F6', color: 'white' }
});

// Error - Equipment
toast.error('Message', {
  icon: '🔧',
  duration: 8000,
  style: { background: '#DC2626', color: 'white' }
});

// Warning - Security
toast('Message', {
  icon: '🚨',
  duration: 10000,
  style: { background: '#F59E0B', color: 'white' }
});
```

### Vue (vue-toastification)

```ts
// Install
npm install vue-toastification

// main.ts
import Toast from 'vue-toastification';
import 'vue-toastification/dist/index.css';

app.use(Toast, {
  timeout: 5000,
  position: 'top-right'
});

// Usage
toast.success('Message', { icon: '📅' });
toast.info('Message', { icon: '📣' });
toast.error('Message', { icon: '🔧' });
toast.warning('Message', { icon: '🚨' });
```

---

## 📚 Additional Resources

- [SignalR JavaScript Client Documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/javascript-client)
- [React Hot Toast](https://react-hot-toast.com/)
- [Vue Toastification](https://vue-toastification.maronato.dev/)

---

**Last Updated:** November 14, 2025  
**Backend Version:** .NET 8.0  
**SignalR Version:** ASP.NET Core SignalR
