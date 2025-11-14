# High Priority Lab Event API - Frontend Integration Guide

## Overview

High Priority Lab Events tự động cancel các bookings và low-priority events trùng thời gian. API sẽ trả về thông tin về những gì đã bị cancelled.

## API Endpoints

### 1. Create Lab Event

**Endpoint**: `POST /api/labevents`

**Authorization**: Required (Bearer token)

**Request Body**:

```json
{
  "labId": 5,
  "zoneId": 10,
  "activityTypeId": 1,
  "organizerId": 123,
  "title": "Important Workshop",
  "description": "Mandatory training session",
  "startTime": "2025-11-15T10:00:00Z",
  "endTime": "2025-11-15T16:00:00Z",
  "status": 1,
  "isHighPriority": true
}
```

**Fields**:
- `isHighPriority`: (optional, default: `false`)
  - `true`: High priority event, sẽ auto-cancel conflicts
  - `false` hoặc không gửi: Normal event, sẽ bị reject nếu conflict với high priority

**Response Success** (High Priority Event):

```json
{
  "data": {
    "event": {
      "eventId": 456,
      "labId": 5,
      "zoneId": 10,
      "activityTypeId": 1,
      "organizerId": 123,
      "title": "Important Workshop",
      "description": "Mandatory training session",
      "startTime": "2025-11-15T10:00:00Z",
      "endTime": "2025-11-15T16:00:00Z",
      "status": 1,
      "isHighPriority": true,
      "createdAt": "2025-11-14T04:30:00Z"
    },
    "cancelledBookings": [
      {
        "bookingId": 100,
        "userId": 50,
        "labId": 5,
        "zoneId": 10,
        "startTime": "2025-11-15T11:00:00Z",
        "endTime": "2025-11-15T13:00:00Z",
        "status": 2,
        "notes": "Regular booking"
      },
      {
        "bookingId": 101,
        "userId": 51,
        "labId": 5,
        "zoneId": 10,
        "startTime": "2025-11-15T14:00:00Z",
        "endTime": "2025-11-15T15:00:00Z",
        "status": 2,
        "notes": "Another booking"
      }
    ],
    "cancelledEvents": [
      {
        "eventId": 200,
        "labId": 5,
        "zoneId": 10,
        "title": "Regular Meeting",
        "startTime": "2025-11-15T12:00:00Z",
        "endTime": "2025-11-15T14:00:00Z",
        "status": 2,
        "isHighPriority": false
      }
    ],
    "totalCancelled": 3
  },
  "message": "Lab event created successfully. 3 conflicting item(s) were automatically cancelled (2 booking(s), 1 event(s))",
  "success": true
}
```

**Response Success** (Normal Event, No Conflicts):

```json
{
  "data": {
    "event": {
      "eventId": 457,
      "title": "Study Group",
      "isHighPriority": false,
      ...
    },
    "cancelledBookings": [],
    "cancelledEvents": [],
    "totalCancelled": 0
  },
  "message": "Lab event created successfully",
  "success": true
}
```

**Response Error** (Conflict with High Priority):

```json
{
  "success": false,
  "message": "Cannot create event: conflicts with a high priority event",
  "errors": null
}
```

### 2. Update Lab Event

**Endpoint**: `PUT /api/labevents/{id}`

**Authorization**: Required

**Request Body**:

```json
{
  "title": "Updated Workshop",
  "isHighPriority": true,
  "startTime": "2025-11-15T09:00:00Z",
  "endTime": "2025-11-15T17:00:00Z"
}
```

**Notes**:
- Tất cả fields đều optional
- Nếu `isHighPriority` không gửi → giữ nguyên giá trị cũ
- Nếu change từ `false` → `true` → auto-cancel conflicts

**Response**:

Giống như Create Event, bao gồm `cancelledBookings`, `cancelledEvents`, và `totalCancelled`.

```json
{
  "data": {
    "event": { ... },
    "cancelledBookings": [ ... ],
    "cancelledEvents": [ ... ],
    "totalCancelled": 2
  },
  "message": "Lab event updated successfully. 2 conflicting item(s) were automatically cancelled (1 booking(s), 1 event(s))",
  "success": true
}
```

## Frontend Implementation

### React/TypeScript Example

#### 1. Type Definitions

```typescript
// types/labEvent.ts

export interface LabEventDTO {
  eventId: number;
  labId: number;
  zoneId: number;
  activityTypeId: number;
  organizerId: number;
  title: string;
  description?: string;
  startTime: string;
  endTime: string;
  status: number;
  isHighPriority: boolean;
  createdAt: string;
}

export interface BookingDTO {
  bookingId: number;
  userId: number;
  labId: number;
  zoneId: number;
  startTime: string;
  endTime: string;
  status: number;
  notes?: string;
}

export interface LabEventCreationResult {
  event: LabEventDTO;
  cancelledBookings: BookingDTO[];
  cancelledEvents: LabEventDTO[];
  totalCancelled: number;
}

export interface CreateLabEventDTO {
  labId: number;
  zoneId: number;
  activityTypeId: number;
  organizerId: number;
  title: string;
  description?: string;
  startTime: string;
  endTime: string;
  status: number;
  isHighPriority?: boolean; // Optional, default false
}
```

#### 2. API Service

```typescript
// services/labEventService.ts

import axios from 'axios';
import { LabEventCreationResult, CreateLabEventDTO } from '../types/labEvent';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

export const labEventService = {
  /**
   * Create a new lab event
   * @param data - Event creation data
   * @param isHighPriority - Whether this is a high priority event
   * @returns Created event with cancelled items info
   */
  async createLabEvent(
    data: CreateLabEventDTO, 
    isHighPriority: boolean = false
  ): Promise<LabEventCreationResult> {
    const token = localStorage.getItem('authToken');
    
    const response = await axios.post<{
      data: LabEventCreationResult;
      message: string;
      success: boolean;
    }>(
      `${API_BASE_URL}/labevents`,
      { ...data, isHighPriority },
      {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      }
    );
    
    return response.data.data;
  },

  /**
   * Update an existing lab event
   */
  async updateLabEvent(
    id: number, 
    updates: Partial<CreateLabEventDTO>
  ): Promise<LabEventCreationResult> {
    const token = localStorage.getItem('authToken');
    
    const response = await axios.put<{
      data: LabEventCreationResult;
      message: string;
    }>(
      `${API_BASE_URL}/labevents/${id}`,
      updates,
      {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      }
    );
    
    return response.data.data;
  }
};
```

#### 3. React Component - Create Event Form

```tsx
// components/CreateLabEventForm.tsx

import React, { useState } from 'react';
import { labEventService } from '../services/labEventService';
import { LabEventCreationResult } from '../types/labEvent';

const CreateLabEventForm: React.FC = () => {
  const [formData, setFormData] = useState({
    labId: 0,
    zoneId: 0,
    activityTypeId: 1,
    organizerId: 0,
    title: '',
    description: '',
    startTime: '',
    endTime: '',
    status: 1
  });
  
  const [isHighPriority, setIsHighPriority] = useState(false);
  const [result, setResult] = useState<LabEventCreationResult | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setResult(null);

    try {
      const result = await labEventService.createLabEvent(formData, isHighPriority);
      setResult(result);
      
      // Show success notification
      if (result.totalCancelled > 0) {
        alert(
          `Event created! ${result.totalCancelled} conflicts automatically cancelled:\n` +
          `- ${result.cancelledBookings.length} bookings\n` +
          `- ${result.cancelledEvents.length} events`
        );
      } else {
        alert('Event created successfully!');
      }
      
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 'Failed to create event';
      setError(errorMessage);
      alert(errorMessage);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Create Lab Event</h2>
      
      {/* Form fields */}
      <input
        type="text"
        placeholder="Event Title"
        value={formData.title}
        onChange={(e) => setFormData({ ...formData, title: e.target.value })}
        required
      />
      
      <textarea
        placeholder="Description"
        value={formData.description}
        onChange={(e) => setFormData({ ...formData, description: e.target.value })}
      />
      
      <input
        type="datetime-local"
        value={formData.startTime}
        onChange={(e) => setFormData({ ...formData, startTime: e.target.value })}
        required
      />
      
      <input
        type="datetime-local"
        value={formData.endTime}
        onChange={(e) => setFormData({ ...formData, endTime: e.target.value })}
        required
      />
      
      {/* High Priority Checkbox */}
      <label>
        <input
          type="checkbox"
          checked={isHighPriority}
          onChange={(e) => setIsHighPriority(e.target.checked)}
        />
        <strong>High Priority Event</strong>
        <span style={{ fontSize: '0.9em', color: '#666', marginLeft: '8px' }}>
          (will automatically cancel conflicting bookings and events)
        </span>
      </label>
      
      {isHighPriority && (
        <div style={{ 
          backgroundColor: '#fff3cd', 
          padding: '10px', 
          borderRadius: '4px',
          marginTop: '10px'
        }}>
          ⚠️ Warning: This will cancel all conflicting bookings and low-priority events!
        </div>
      )}
      
      <button type="submit">Create Event</button>
      
      {/* Display cancelled items */}
      {result && result.totalCancelled > 0 && (
        <div style={{ marginTop: '20px', padding: '15px', backgroundColor: '#f8f9fa' }}>
          <h3>Cancelled Items ({result.totalCancelled} total)</h3>
          
          {result.cancelledBookings.length > 0 && (
            <div>
              <h4>Cancelled Bookings ({result.cancelledBookings.length})</h4>
              <ul>
                {result.cancelledBookings.map(booking => (
                  <li key={booking.bookingId}>
                    Booking #{booking.bookingId} - User {booking.userId}
                    <br />
                    {new Date(booking.startTime).toLocaleString()} - 
                    {new Date(booking.endTime).toLocaleString()}
                  </li>
                ))}
              </ul>
            </div>
          )}
          
          {result.cancelledEvents.length > 0 && (
            <div>
              <h4>Cancelled Events ({result.cancelledEvents.length})</h4>
              <ul>
                {result.cancelledEvents.map(event => (
                  <li key={event.eventId}>
                    {event.title} (Event #{event.eventId})
                    <br />
                    {new Date(event.startTime).toLocaleString()} - 
                    {new Date(event.endTime).toLocaleString()}
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}
      
      {error && (
        <div style={{ color: 'red', marginTop: '10px' }}>
          {error}
        </div>
      )}
    </form>
  );
};

export default CreateLabEventForm;
```

#### 4. Notification Component for Cancelled Items

```tsx
// components/CancelledItemsNotification.tsx

import React from 'react';
import { LabEventCreationResult } from '../types/labEvent';

interface Props {
  result: LabEventCreationResult;
  onClose: () => void;
}

const CancelledItemsNotification: React.FC<Props> = ({ result, onClose }) => {
  if (result.totalCancelled === 0) return null;

  return (
    <div className="notification-overlay">
      <div className="notification-modal">
        <h3>⚠️ Items Automatically Cancelled</h3>
        
        <p>
          Your high priority event cancelled <strong>{result.totalCancelled}</strong> conflicting item(s):
        </p>
        
        <div className="cancelled-summary">
          <div>📅 {result.cancelledBookings.length} Booking(s)</div>
          <div>🎯 {result.cancelledEvents.length} Event(s)</div>
        </div>
        
        {result.cancelledBookings.length > 0 && (
          <div className="cancelled-section">
            <h4>Cancelled Bookings</h4>
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>User</th>
                  <th>Time</th>
                </tr>
              </thead>
              <tbody>
                {result.cancelledBookings.map(booking => (
                  <tr key={booking.bookingId}>
                    <td>#{booking.bookingId}</td>
                    <td>User {booking.userId}</td>
                    <td>
                      {new Date(booking.startTime).toLocaleString()} - 
                      {new Date(booking.endTime).toLocaleString()}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        
        {result.cancelledEvents.length > 0 && (
          <div className="cancelled-section">
            <h4>Cancelled Events</h4>
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Title</th>
                  <th>Time</th>
                </tr>
              </thead>
              <tbody>
                {result.cancelledEvents.map(event => (
                  <tr key={event.eventId}>
                    <td>#{event.eventId}</td>
                    <td>{event.title}</td>
                    <td>
                      {new Date(event.startTime).toLocaleString()} - 
                      {new Date(event.endTime).toLocaleString()}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        
        <button onClick={onClose}>Close</button>
      </div>
    </div>
  );
};

export default CancelledItemsNotification;
```

### Vue 3 Example

```vue
<!-- CreateLabEventForm.vue -->

<template>
  <form @submit.prevent="handleSubmit">
    <h2>Create Lab Event</h2>
    
    <input 
      v-model="formData.title" 
      placeholder="Event Title" 
      required 
    />
    
    <textarea 
      v-model="formData.description" 
      placeholder="Description"
    />
    
    <input 
      v-model="formData.startTime" 
      type="datetime-local" 
      required 
    />
    
    <input 
      v-model="formData.endTime" 
      type="datetime-local" 
      required 
    />
    
    <label>
      <input 
        v-model="isHighPriority" 
        type="checkbox" 
      />
      <strong>High Priority Event</strong>
      <span class="hint">
        (will automatically cancel conflicting bookings and events)
      </span>
    </label>
    
    <div v-if="isHighPriority" class="warning">
      ⚠️ Warning: This will cancel all conflicting bookings and low-priority events!
    </div>
    
    <button type="submit">Create Event</button>
    
    <!-- Display cancelled items -->
    <div v-if="result && result.totalCancelled > 0" class="cancelled-info">
      <h3>Cancelled Items ({{ result.totalCancelled }} total)</h3>
      
      <div v-if="result.cancelledBookings.length > 0">
        <h4>Cancelled Bookings ({{ result.cancelledBookings.length }})</h4>
        <ul>
          <li v-for="booking in result.cancelledBookings" :key="booking.bookingId">
            Booking #{{ booking.bookingId }} - User {{ booking.userId }}
            <br />
            {{ formatDateTime(booking.startTime) }} - {{ formatDateTime(booking.endTime) }}
          </li>
        </ul>
      </div>
      
      <div v-if="result.cancelledEvents.length > 0">
        <h4>Cancelled Events ({{ result.cancelledEvents.length }})</h4>
        <ul>
          <li v-for="event in result.cancelledEvents" :key="event.eventId">
            {{ event.title }} (Event #{{ event.eventId }})
            <br />
            {{ formatDateTime(event.startTime) }} - {{ formatDateTime(event.endTime) }}
          </li>
        </ul>
      </div>
    </div>
  </form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios';
import type { LabEventCreationResult } from '../types/labEvent';

const formData = ref({
  labId: 0,
  zoneId: 0,
  activityTypeId: 1,
  organizerId: 0,
  title: '',
  description: '',
  startTime: '',
  endTime: '',
  status: 1
});

const isHighPriority = ref(false);
const result = ref<LabEventCreationResult | null>(null);

const handleSubmit = async () => {
  try {
    const token = localStorage.getItem('authToken');
    
    const response = await axios.post<{
      data: LabEventCreationResult;
      message: string;
    }>(
      '/api/labevents',
      { ...formData.value, isHighPriority: isHighPriority.value },
      {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      }
    );
    
    result.value = response.data.data;
    
    if (result.value.totalCancelled > 0) {
      alert(
        `Event created! ${result.value.totalCancelled} conflicts automatically cancelled:\n` +
        `- ${result.value.cancelledBookings.length} bookings\n` +
        `- ${result.value.cancelledEvents.length} events`
      );
    } else {
      alert('Event created successfully!');
    }
    
  } catch (err: any) {
    const errorMessage = err.response?.data?.message || 'Failed to create event';
    alert(errorMessage);
  }
};

const formatDateTime = (dateString: string) => {
  return new Date(dateString).toLocaleString();
};
</script>

<style scoped>
.warning {
  background-color: #fff3cd;
  padding: 10px;
  border-radius: 4px;
  margin-top: 10px;
}

.cancelled-info {
  margin-top: 20px;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 4px;
}

.hint {
  font-size: 0.9em;
  color: #666;
  margin-left: 8px;
}
</style>
```

## Business Logic Summary

### High Priority Event Creation

1. **Request**: `isHighPriority: true`
2. **Backend Process**:
   - Tìm tất cả bookings trùng thời gian → Cancel (Status = 2)
   - Tìm tất cả events có `isHighPriority: false` trùng thời gian → Cancel
   - Tạo event mới
3. **Response**: Trả về event + danh sách items bị cancelled

### Normal Event Creation

1. **Request**: `isHighPriority: false` hoặc không gửi
2. **Backend Process**:
   - Check xem có high priority event trùng giờ không
   - Nếu có → Throw error
   - Nếu không → Tạo event bình thường
3. **Response**: Trả về event (không có cancelled items)

### Update Event to High Priority

1. **Request**: `isHighPriority: true` (event hiện tại là `false`)
2. **Backend Process**:
   - Auto-cancel tất cả conflicts
   - Update event
3. **Response**: Event updated + cancelled items

## UI/UX Recommendations

### 1. Warning Before Creating High Priority Event

```tsx
{isHighPriority && (
  <div className="warning-box">
    <h4>⚠️ High Priority Event</h4>
    <p>
      This event will <strong>automatically cancel</strong> all conflicting:
    </p>
    <ul>
      <li>Member bookings</li>
      <li>Low-priority events</li>
    </ul>
    <p>Are you sure you want to proceed?</p>
  </div>
)}
```

### 2. Show Cancelled Items After Creation

Display a modal or notification with:
- Total number of cancelled items
- List of cancelled bookings (with user info)
- List of cancelled events (with titles)

### 3. Calendar Display

```tsx
// Display high priority events with special styling
<div className={`event ${event.isHighPriority ? 'high-priority' : ''}`}>
  {event.isHighPriority && <span className="priority-badge">⚡ HIGH PRIORITY</span>}
  <h3>{event.title}</h3>
</div>
```

CSS:
```css
.event.high-priority {
  border-left: 4px solid #dc3545;
  background-color: #ffe6e6;
}

.priority-badge {
  background-color: #dc3545;
  color: white;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 0.8em;
  font-weight: bold;
}
```

### 4. Conflict Warning for Normal Events

```tsx
try {
  await createLabEvent(data, false);
} catch (error) {
  if (error.message.includes('conflicts with a high priority event')) {
    showModal({
      title: 'Cannot Create Event',
      message: 'This time slot is reserved for a high priority event. Please choose a different time.',
      type: 'error'
    });
  }
}
```

## Error Handling

### Common Errors

1. **Conflict with High Priority Event**:
```json
{
  "success": false,
  "message": "Cannot create event: conflicts with a high priority event"
}
```
**Action**: Show user-friendly message, suggest alternative times

2. **Conflict when Updating to High Priority**:
```json
{
  "success": false,
  "message": "Cannot update event: conflicts with a high priority event"
}
```
**Action**: Inform user that another high priority event exists

3. **Validation Errors**:
```json
{
  "success": false,
  "message": "Invalid lab event data",
  "errors": ["StartTime must be before EndTime"]
}
```
**Action**: Display field-specific errors

## Testing Scenarios

### Test Case 1: Create High Priority Event with Conflicts

1. Create 2 normal bookings: 10:00-12:00, 14:00-15:00
2. Create 1 normal event: 11:00-13:00
3. Create high priority event: 10:00-16:00
4. **Expected**: Response includes 2 cancelled bookings + 1 cancelled event

### Test Case 2: Try to Create Normal Event with High Priority Conflict

1. Create high priority event: 10:00-12:00
2. Try to create normal event: 11:00-13:00
3. **Expected**: Error 400 with conflict message

### Test Case 3: Update Normal Event to High Priority

1. Create normal event: 10:00-12:00
2. Create booking: 11:00-13:00
3. Update event to high priority
4. **Expected**: Booking cancelled, response includes cancelled items

## Summary

✅ **API Returns**:
- Created/updated event
- List of cancelled bookings
- List of cancelled events
- Total count of cancelled items

✅ **Frontend Should**:
- Show warning before creating high priority events
- Display cancelled items after successful creation
- Handle conflict errors gracefully
- Use special UI for high priority events (badges, colors)

✅ **User Experience**:
- Clear indication of high priority events
- Warning before cancelling conflicts
- Detailed info about what was cancelled
- Prevent normal events from conflicting with high priority
