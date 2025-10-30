# Search, Sort, and Pagination Guide

## Overview
This guide explains how to use the search, sort, and pagination features available in the List endpoints of the Lab Management API.

## Implementation Status
✅ **User API** - Fully implemented with search, sort, and pagination  
✅ **Lab API** - Search, sort, and pagination available via `GET /api/labs/paged`  
✅ **Equipment API** - Search, sort, and pagination available via `GET /api/equipments/paged`  
✅ **Booking API** - Search, sort, and pagination available via `GET /api/bookings/paged`  
✅ **Report API** - Search, sort, and pagination available via `GET /api/reports/paged`  
✅ **Lab Event API** - Search, sort, and pagination available via `GET /api/lab-events/paged`  
✅ **Security Log API** - Search, sort, and pagination available via `GET /api/security-logs/paged`  
✅ **Notification API** - Search, sort, and pagination available via `GET /api/notifications/paged`

## Query Parameters

All List endpoints support the following query parameters:

### Pagination Parameters
- `PageNumber` (int, default: 1): The page number to retrieve
- `PageSize` (int, default: 10, max: 100): Number of items per page

### Search Parameter
- `SearchTerm` (string, optional): Keyword to search across multiple fields

### Sort Parameters
- `SortBy` (string, optional): Field name to sort by
- `SortOrder` (string, default: "asc"): Sort direction - "asc" or "desc"

## Response Format

All paginated endpoints return a `PagedResult<T>` object with the following structure:

```json
{
  "success": true,
  "message": "Users retrieved successfully",
  "data": {
    "items": [...],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 45,
    "totalPages": 5,
    "hasPrevious": false,
    "hasNext": true
  }
}
```

### Response Properties
- `items`: Array of DTOs for the current page
- `pageNumber`: Current page number
- `pageSize`: Items per page
- `totalCount`: Total number of items matching the query
- `totalPages`: Total number of pages available
- `hasPrevious`: Boolean indicating if there's a previous page
- `hasNext`: Boolean indicating if there's a next page

## API Examples

### Users API

#### Legacy Endpoint (Backward Compatible)
```
GET /api/users
```
Returns all users without pagination (original behavior maintained).

#### New Paginated Endpoint
```
GET /api/users/paged
```

##### Searchable Fields
- Name (case-insensitive)
- Email (case-insensitive)

#### Sortable Fields
- `name` - User's full name
- `email` - User's email address
- `role` - User's role
- `createdat` - Account creation date

#### Example Requests

1. **Legacy endpoint - All users**
   ```
   GET /api/users
   ```
   *Returns all users without pagination (backward compatible)*

2. **Basic pagination**
   ```
   GET /api/users/paged?pageNumber=1&pageSize=20
   ```

3. **Search by keyword**
   ```
   GET /api/users/paged?searchTerm=john&pageNumber=1&pageSize=10
   ```
   *Searches in Name and Email fields*

4. **Sort by name (ascending)**
   ```
   GET /api/users/paged?sortBy=name&sortOrder=asc
   ```

5. **Sort by creation date (descending)**
   ```
   GET /api/users/paged?sortBy=createdat&sortOrder=desc
   ```

6. **Combine all features**
   ```
   GET /api/users/paged?searchTerm=manager&sortBy=name&sortOrder=asc&pageNumber=1&pageSize=25
   ```

### Labs API

#### Legacy Endpoint
```
GET /api/labs
```

#### New Paginated Endpoint
```
GET /api/labs/paged
```

##### Searchable Fields
- Name
- Location
- Description

##### Sortable Fields
- `name`
- `location`
- `managerid`

##### Example
```
GET /api/labs/paged?searchTerm=chemistry&sortBy=name&pageNumber=1&pageSize=10
```

### Equipment API

#### Legacy Endpoint
```
GET /api/equipments
```

#### New Paginated Endpoint
```
GET /api/equipments/paged
```

##### Searchable Fields
- Name
- Code
- Description

##### Sortable Fields
- `name`
- `code`
- `status`
- `labid`

##### Example
```
GET /api/equipments/paged?searchTerm=microscope&sortBy=status&sortOrder=desc
```

### Bookings API

#### Legacy Endpoint
```
GET /api/bookings
```

#### New Paginated Endpoint
```
GET /api/bookings/paged
```

##### Searchable Fields
- Notes
- Booking ID
- User ID
- Lab ID
- Zone ID
- Status

##### Sortable Fields
- `starttime`
- `endtime`
- `status`
- `createdat`
- `userid`
- `labid`
- `zoneid`

##### Example
```
GET /api/bookings/paged?searchTerm=maintenance&sortBy=starttime&sortOrder=asc
```

### Reports API

#### Legacy Endpoint
```
GET /api/reports
```

#### New Paginated Endpoint
```
GET /api/reports/paged
```

##### Searchable Fields
- Report type
- Content
- Photo URL
- Lab name
- Zone name

##### Sortable Fields
- `reporttype`
- `generatedat`
- `labid`
- `zoneid`
- `userid`

##### Example
```
GET /api/reports/paged?searchTerm=laboratory&sortBy=generatedat&sortOrder=desc
```

### Lab Events API

#### Legacy Endpoint
```
GET /api/lab-events
```

#### New Paginated Endpoint
```
GET /api/lab-events/paged
```

##### Searchable Fields
- Title
- Description
- Event ID
- Lab ID
- Zone ID
- Organizer ID

##### Sortable Fields
- `title`
- `starttime`
- `endtime`
- `status`
- `createdat`
- `labid`
- `zoneid`

##### Example
```
GET /api/lab-events/paged?searchTerm=orientation&sortBy=starttime
```

### Security Logs API

#### Legacy Endpoint
```
GET /api/security-logs
```

#### New Paginated Endpoint
```
GET /api/security-logs/paged
```

##### Searchable Fields
- Notes
- Photo URL
- Log ID
- Security ID
- Event ID
- Action type

##### Sortable Fields
- `loggedat`
- `actiontype`
- `securityid`
- `eventid`

##### Example
```
GET /api/security-logs/paged?sortBy=loggedat&sortOrder=desc&pageNumber=2
```

### Notifications API

#### Legacy Endpoint
```
GET /api/notifications
```

#### New Paginated Endpoint
```
GET /api/notifications/paged
```

##### Searchable Fields
- Message
- Notification ID
- Recipient ID
- Event ID

##### Sortable Fields
- `sentat`
- `isread`
- `recipientid`
- `eventid`

##### Example
```
GET /api/notifications/paged?searchTerm=alert&sortBy=sentat&sortOrder=desc
```

## Implementation Pattern

The search/sort/pagination pattern follows this architecture:

1. **Common Layer**
   - `QueryParameters` model - Defines search/sort/pagination parameters
   - `PagedResult<T>` model - Generic wrapper for paginated responses
   - `QueryableExtensions` - Extension methods for pagination

2. **Repository Layer (DAL)**
   - Exposes `IQueryable<T>` for flexible querying
   - Example: `IQueryable<User> GetUsersQueryable()`
   - Maintains existing methods for backward compatibility

3. **Service Layer (BLL)**
   - Adds new method with QueryParameters: `GetUsersAsync(QueryParameters)`
   - Keeps existing method: `GetAllUsersAsync()` for backward compatibility
   - Applies search filters to the queryable
   - Applies sorting based on field name
   - Calls `ToPagedResultAsync()` for pagination
   - Maps entities to DTOs

4. **Controller Layer (API)**
   - **Legacy endpoint**: `GET /api/users` - Returns all users (unchanged)
   - **New endpoint**: `GET /api/users/paged` - Accepts `QueryParameters` from query string
   - Returns `PagedResult<DTO>` wrapped in `ApiResponse`

## Future Enhancements

- [ ] Extend support to additional endpoints as new modules are added

## Best Practices

1. **Backward Compatibility**: Legacy endpoints (`GET /api/users`) remain unchanged for existing clients
2. **New Endpoints**: Use `/paged` suffix for new paginated endpoints (e.g., `GET /api/users/paged`)
3. **Migration Path**: Clients can migrate to paginated endpoints at their own pace
4. **Page Size Limits**: Maximum page size is capped at 100 to prevent performance issues
5. **Search Performance**: Use specific search terms rather than very broad queries
6. **Sort Fields**: Only use documented sortable fields to avoid errors
7. **Default Behavior**: If no parameters are provided on `/paged` endpoints, returns first 10 items sorted by default order

## Error Handling

- Invalid `SortBy` field: Returns 400 Bad Request
- Invalid `SortOrder` value: Uses default "asc"
- Page size > 100: Automatically capped at 100
- Page number < 1: Uses default 1

## Notes

- Search is **case-insensitive** for all text fields
- Empty search terms are ignored (returns all records)
- Pagination metadata helps with UI implementation (prev/next buttons, page counts)
