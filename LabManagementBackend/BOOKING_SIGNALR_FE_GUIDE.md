# Real-Time Booking Notifications – Frontend Guide

This document explains how React or Vue clients can subscribe to booking events exposed by `BookingHub` (`/hubs/booking`).

## 1. Overview

When a member creates a booking, the backend sends a `BookingCreated` message to the SignalR group `manager:{managerId}` for the lab’s manager. Frontend apps should connect to the hub after authenticating and join the correct manager group so that Lab Managers receive bookings instantly.

```
Member creates booking → BookingController → BookingHub
                               ↓
                        Broadcast (BookingCreated)
                               ↓
               Lab Manager client receives payload
```

## 2. API Recap

- **Hub endpoint:** `https://<api-host>/hubs/booking`
- **Group naming:** `manager:{managerId}`
- **Server payload on booking creation:**

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

## 3. React Example

Install SignalR client:

```bash
npm install @microsoft/signalr
```

Create a hook / service:

```tsx
import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

export function useBookingNotifications(apiBaseUrl: string, token: string, managerId: number, onBookingCreated: (payload: any) => void) {
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${apiBaseUrl}/hubs/booking`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.on('BookingCreated', onBookingCreated);

    connection.start()
      .then(() => connection.invoke('JoinManagerGroup', managerId))
      .catch(console.error);

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [apiBaseUrl, token, managerId, onBookingCreated]);
}
```

Usage:

```tsx
const MyComponent = () => {
  useBookingNotifications(API_URL, authToken, currentManagerId, payload => {
    toast.success(`New booking for ${payload.labName}!`);
  });

  return <div>…</div>;
};
```

## 4. Vue Example

Install:

```bash
npm install @microsoft/signalr
```

Composable (`useBookingNotifications.ts`):

```ts
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { onMounted, onBeforeUnmount } from 'vue';

export function useBookingNotifications(apiBaseUrl: string, token: string, managerId: number, onBookingCreated: (payload: any) => void) {
  const connection = new HubConnectionBuilder()
    .withUrl(`${apiBaseUrl}/hubs/booking`, {
      accessTokenFactory: () => token
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();

  onMounted(async () => {
    connection.on('BookingCreated', onBookingCreated);
    await connection.start();
    await connection.invoke('JoinManagerGroup', managerId);
  });

  onBeforeUnmount(() => {
    connection.stop();
  });
}
```

Usage in a component:

```ts
useBookingNotifications(API_URL, authToken, currentManagerId, payload => {
  notifications.push({
    title: 'New Booking',
    message: `${payload.labName} reserved ${payload.startTime}`
  });
});
```

## 5. Security Notes

1. **Authentication:** pass the same JWT you use for API calls via `accessTokenFactory`.
2. **Join / Leave:** when the lab manager switches context (e.g., different lab), call `JoinManagerGroup` with the new manager ID. Optionally call `LeaveManagerGroup` for the old one.
3. **Reconnect:** both snippets use `withAutomaticReconnect()`, but you should still handle UI states (e.g., show “Disconnected” banner).

## 6. Troubleshooting

- 401 errors: ensure JWT is attached and not expired.
- No messages: confirm the manager ID matches the `Lab.managerId` field in the backend.
- HTTPS mixed content: for dev, align frontend origin (`http://localhost:3000`) with backend CORS + SignalR configuration.

With these steps, your React or Vue client can showcase real-time booking notifications during demos.***
