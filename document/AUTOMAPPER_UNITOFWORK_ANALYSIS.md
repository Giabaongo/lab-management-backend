# 🎨 AutoMapper & Unit of Work - Flow Analysis & Evaluation

## 📊 ARCHITECTURE OVERVIEW

Bạn đã implement 2 design patterns quan trọng:
1. **AutoMapper** - Object-to-Object mapping
2. **Unit of Work** - Transaction management pattern

---

## 🔄 COMPLETE DATA FLOW

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT REQUEST                           │
│                  (JSON: CreateUserDTO)                           │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                      1. API CONTROLLER                           │
│                   (UserController.cs)                            │
│  • Receives CreateUserDTO from request body                      │
│  • Validates with [FromBody] + ModelState                        │
│  • Calls: _userService.CreateUserAsync(dto)                      │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                  2. SERVICE LAYER (BLL)                          │
│                   (UserService.cs)                               │
│                                                                   │
│  STEP 1: AutoMapper - DTO → Entity                               │
│  ┌───────────────────────────────────────────────────────┐      │
│  │ var user = _mapper.Map<User>(createUserDto);          │      │
│  │                                                         │      │
│  │ AutoMapper Profile (UserProfile.cs):                   │      │
│  │ CreateMap<CreateUserDTO, User>()                       │      │
│  │   .ForMember(dest => dest.PasswordHash, opt.Ignore()) │      │
│  │   .ForMember(dest => dest.CreatedAt, opt.Ignore())    │      │
│  │                                                         │      │
│  │ Result: User entity (partial, no password/date yet)    │      │
│  └───────────────────────────────────────────────────────┘      │
│                                                                   │
│  STEP 2: Password Hashing                                        │
│  ┌───────────────────────────────────────────────────────┐      │
│  │ user.PasswordHash = _passwordHasher.HashPassword(...); │      │
│  │ user.CreatedAt = DateTime.UtcNow;                      │      │
│  └───────────────────────────────────────────────────────┘      │
│                                                                   │
│  STEP 3: Unit of Work - Add to repository                        │
│  ┌───────────────────────────────────────────────────────┐      │
│  │ await _unitOfWork.Users.AddAsync(user);               │      │
│  │ await _unitOfWork.SaveChangesAsync();                 │      │
│  └───────────────────────────────────────────────────────┘      │
│                                                                   │
│  STEP 4: AutoMapper - Entity → DTO                               │
│  ┌───────────────────────────────────────────────────────┐      │
│  │ return _mapper.Map<UserDTO>(user);                     │      │
│  │                                                         │      │
│  │ AutoMapper Profile:                                     │      │
│  │ CreateMap<User, UserDTO>()                             │      │
│  │                                                         │      │
│  │ Result: UserDTO (safe for API response)                │      │
│  └───────────────────────────────────────────────────────┘      │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                  3. UNIT OF WORK (DAL)                           │
│                   (UnitOfWork.cs)                                │
│                                                                   │
│  Properties:                                                      │
│  • IUserRepository Users (lazy-loaded)                           │
│  • SaveChangesAsync() → commits transaction                      │
│                                                                   │
│  Flow:                                                            │
│  ┌───────────────────────────────────────────────────────┐      │
│  │ 1. _unitOfWork.Users → Returns IUserRepository        │      │
│  │    (creates UserRepository if not exists)              │      │
│  │                                                         │      │
│  │ 2. _unitOfWork.Users.AddAsync(user)                    │      │
│  │    → Adds to DbContext (not DB yet)                    │      │
│  │                                                         │      │
│  │ 3. _unitOfWork.SaveChangesAsync()                      │      │
│  │    → _context.SaveChangesAsync()                       │      │
│  │    → COMMITS to database                               │      │
│  └───────────────────────────────────────────────────────┘      │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                  4. REPOSITORY (DAL)                             │
│                (UserRepository.cs)                               │
│                                                                   │
│  Inherits: GenericRepository<User>                               │
│                                                                   │
│  AddAsync(user):                                                 │
│  ┌───────────────────────────────────────────────────────┐      │
│  │ await _dbSet.AddAsync(user);                           │      │
│  │ // Does NOT call SaveChanges here!                     │      │
│  │ // Unit of Work will handle transaction                │      │
│  └───────────────────────────────────────────────────────┘      │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                  5. DATABASE                                     │
│              (Azure SQL / SQL Server)                            │
│                                                                   │
│  INSERT INTO Users (Name, Email, PasswordHash, Role, CreatedAt) │
│  VALUES ('John', 'john@example.com', '$2a$11$...', 2, '2025...') │
│                                                                   │
│  Returns: UserId (auto-generated by database)                    │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                  6. RESPONSE                                     │
│                 (Back to Controller)                             │
│                                                                   │
│  UserDTO object:                                                 │
│  {                                                                │
│    "userId": 123,                                                │
│    "name": "John",                                               │
│    "email": "john@example.com",                                  │
│    "role": 2,                                                    │
│    "createdAt": "2025-10-15T10:30:00Z"                           │
│  }                                                                │
│                                                                   │
│  Wrapped in ApiResponse<UserDTO>:                                │
│  {                                                                │
│    "success": true,                                              │
│    "message": "User created successfully",                       │
│    "data": { /* UserDTO above */ },                              │
│    "errors": []                                                  │
│  }                                                                │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🎯 AUTOMAPPER FLOW DETAILS

### 1. **Registration (Program.cs)**
```csharp
builder.Services.AddAutoMapper(typeof(UserProfile));
```
- Scans assembly chứa `UserProfile`
- Registers all `Profile` classes
- Injects `IMapper` vào DI container

### 2. **Profile Configuration (UserProfile.cs)**
```csharp
public class UserProfile : Profile
{
    public UserProfile()
    {
        // Entity → DTO (for responses)
        CreateMap<User, UserDTO>();
        
        // DTO → Entity (for create)
        CreateMap<CreateUserDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())  // ← Manual handling
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());    // ← Manual handling
        
        // DTO → Entity (for update)
        CreateMap<UpdateUserDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            // ↑ Only map non-null properties
    }
}
```

### 3. **Mapping Operations**

#### **CREATE: DTO → Entity**
```csharp
// Input: CreateUserDTO
{
    "name": "John",
    "email": "john@example.com",
    "password": "SecurePass123",
    "role": 2
}

// AutoMapper mapping
var user = _mapper.Map<User>(createUserDto);

// Result: User entity
{
    UserId = 0,                    // Not set (DB will generate)
    Name = "John",                 // ✅ Mapped
    Email = "john@example.com",    // ✅ Mapped
    PasswordHash = null,           // ⚠️ Ignored (manual handling)
    Role = 2,                      // ✅ Mapped
    CreatedAt = null               // ⚠️ Ignored (manual handling)
}

// Manual post-processing
user.PasswordHash = _passwordHasher.HashPassword(createUserDto.Password);
user.CreatedAt = DateTime.UtcNow;
```

#### **READ: Entity → DTO**
```csharp
// From database: User entity
{
    UserId = 123,
    Name = "John",
    Email = "john@example.com",
    PasswordHash = "$2a$11$abcd...",  // Sensitive data
    Role = 2,
    CreatedAt = "2025-10-15T10:30:00Z"
}

// AutoMapper mapping
var dto = _mapper.Map<UserDTO>(user);

// Result: UserDTO (safe for API)
{
    "userId": 123,
    "name": "John",
    "email": "john@example.com",
    "role": 2,
    "createdAt": "2025-10-15T10:30:00Z"
}
// Note: PasswordHash is NOT included in DTO → Security ✅
```

#### **UPDATE: DTO → Entity (with conditional mapping)**
```csharp
// Existing user from DB
User existingUser = {
    UserId = 123,
    Name = "John",
    Email = "john@example.com",
    PasswordHash = "$2a$11$abcd...",
    Role = 2,
    CreatedAt = "2025-10-15T10:30:00Z"
}

// Update DTO (partial update)
UpdateUserDTO updateDto = {
    Name = "John Updated",
    Email = null,              // Don't update
    Password = null,           // Don't update
    Role = null                // Don't update
}

// AutoMapper mapping (with condition)
_mapper.Map(updateDto, existingUser);

// Result: Only non-null properties updated
{
    UserId = 123,                           // Unchanged
    Name = "John Updated",                  // ✅ Updated
    Email = "john@example.com",             // ⚠️ Unchanged (null in DTO)
    PasswordHash = "$2a$11$abcd...",        // ⚠️ Unchanged (ignored)
    Role = 2,                               // ⚠️ Unchanged (null in DTO)
    CreatedAt = "2025-10-15T10:30:00Z"     // Unchanged
}
```

---

## 🏗️ UNIT OF WORK FLOW DETAILS

### 1. **Purpose**
- **Centralized transaction management**
- **Single SaveChanges** for multiple operations
- **Ensures data consistency**

### 2. **Structure**
```csharp
public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository Users { get; }              // ← Lazy-loaded repository
    Task<int> SaveChangesAsync(...);            // ← Commits all changes
}
```

### 3. **Implementation**
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly LabManagementDbContext _context;
    private IUserRepository? _userRepository;   // ← Nullable, lazy-loaded
    
    public IUserRepository Users => 
        _userRepository ??= new UserRepository(_context);  // ← Create if null
    
    public Task<int> SaveChangesAsync(...) => 
        _context.SaveChangesAsync(...);  // ← Single transaction
}
```

### 4. **Transaction Flow**

#### **Scenario: Create User**
```csharp
// Service Layer
public async Task<UserDTO> CreateUserAsync(CreateUserDTO dto)
{
    // Step 1: Map DTO → Entity
    var user = _mapper.Map<User>(dto);
    user.PasswordHash = _passwordHasher.HashPassword(dto.Password);
    user.CreatedAt = DateTime.UtcNow;
    
    // Step 2: Add to repository (tracked by DbContext, not saved yet)
    await _unitOfWork.Users.AddAsync(user);
    
    // Step 3: Save changes (commits transaction)
    await _unitOfWork.SaveChangesAsync();  // ← All changes committed here
    
    // Step 4: Map Entity → DTO
    return _mapper.Map<UserDTO>(user);
}
```

#### **Scenario: Multiple Operations (Example)**
```csharp
// If you had multiple repositories
public async Task TransferUserBetweenLabs(int userId, int fromLabId, int toLabId)
{
    // Get user
    var user = await _unitOfWork.Users.GetByIdAsync(userId);
    
    // Remove from old lab
    var oldLabAssignment = await _unitOfWork.LabAssignments.FindAsync(
        la => la.UserId == userId && la.LabId == fromLabId
    );
    await _unitOfWork.LabAssignments.DeleteAsync(oldLabAssignment);
    
    // Add to new lab
    var newAssignment = new LabAssignment { UserId = userId, LabId = toLabId };
    await _unitOfWork.LabAssignments.AddAsync(newAssignment);
    
    // Log activity
    var log = new ActivityLog { UserId = userId, Action = "Lab Transfer" };
    await _unitOfWork.ActivityLogs.AddAsync(log);
    
    // Single transaction commits ALL changes
    await _unitOfWork.SaveChangesAsync();  
    // ↑ If any operation fails, ALL rollback (ACID transaction)
}
```

---

## ✅ ĐÁNH GIÁ IMPLEMENTATION

### **🎯 AutoMapper Implementation: 9/10**

#### **✅ Strengths:**
1. **Proper Profile Configuration**
   - ✅ Separate `UserProfile` class
   - ✅ Clear mapping rules
   - ✅ Proper `.ForMember()` ignores

2. **Security Best Practices**
   - ✅ `PasswordHash` ignored in mapping
   - ✅ Sensitive data not exposed in DTOs
   - ✅ Manual password hashing

3. **Conditional Mapping**
   - ✅ `.ForAllMembers()` with condition for updates
   - ✅ Only non-null properties mapped
   - ✅ Prevents overwriting with null

4. **DI Integration**
   - ✅ Registered in `Program.cs`
   - ✅ Injected via `IMapper`
   - ✅ Scoped lifetime

5. **Code Reduction**
   - ✅ Eliminates manual mapping boilerplate
   - ✅ Reduced lines: ~30% less code
   ```csharp
   // Before (manual mapping)
   return new UserDTO
   {
       UserId = user.UserId,
       Name = user.Name,
       Email = user.Email,
       Role = user.Role,
       CreatedAt = user.CreatedAt
   };
   
   // After (AutoMapper)
   return _mapper.Map<UserDTO>(user);
   ```

#### **⚠️ Minor Issues:**
1. **Missing Reverse Map**
   ```csharp
   // Current
   CreateMap<User, UserDTO>();
   
   // Could add
   CreateMap<User, UserDTO>().ReverseMap();
   // But not needed if you control mapping direction
   ```

2. **No Custom Value Resolvers** (not critical for current use)
   ```csharp
   // Could add for complex transformations
   .ForMember(dest => dest.RoleName, 
       opt => opt.MapFrom(src => UserRoleConstants.GetRoleName(src.Role)))
   ```

#### **📈 Recommendations:**
```csharp
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>()
            // Add custom mappings if needed
            .ForMember(dest => dest.RoleName, 
                opt => opt.MapFrom(src => UserRoleConstants.GetRoleName(src.Role)));
        
        CreateMap<CreateUserDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore()); // ← Add this
        
        CreateMap<UpdateUserDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())     // ← Add this
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())  // ← Add this
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

---

### **🏗️ Unit of Work Implementation: 8.5/10**

#### **✅ Strengths:**
1. **Proper Interface Design**
   - ✅ `IAsyncDisposable` implemented
   - ✅ Repository properties
   - ✅ Transaction method

2. **Lazy Loading**
   - ✅ Repositories created on demand
   - ✅ Null-coalescing operator: `??=`
   - ✅ Performance optimization

3. **Transaction Management**
   - ✅ Single `SaveChangesAsync()` point
   - ✅ ACID compliance
   - ✅ Rollback on failure

4. **Clean Service Layer**
   ```csharp
   // Clean and readable
   await _unitOfWork.Users.AddAsync(user);
   await _unitOfWork.SaveChangesAsync();
   ```

#### **⚠️ Issues Found:**

1. **❌ CRITICAL: Repository calls SaveChanges internally**
   ```csharp
   // Current GenericRepository implementation (PROBLEM)
   public virtual async Task AddAsync(T entity)
   {
       await _dbSet.AddAsync(entity);
       await _context.SaveChangesAsync();  // ← BAD! Should NOT save here
   }
   
   // This defeats the purpose of Unit of Work!
   ```

2. **⚠️ DisposeAsync not disposing context**
   ```csharp
   // Current
   public ValueTask DisposeAsync()
   {
       return ValueTask.CompletedTask;  // Does nothing
   }
   
   // Should be
   public async ValueTask DisposeAsync()
   {
       await _context.DisposeAsync();
   }
   ```

3. **⚠️ Missing BeginTransaction support**
   ```csharp
   // Could add for explicit transactions
   Task BeginTransactionAsync();
   Task CommitTransactionAsync();
   Task RollbackTransactionAsync();
   ```

#### **🔧 CRITICAL FIX NEEDED:**

**GenericRepository.cs - Remove SaveChanges:**
```csharp
// BEFORE (WRONG)
public virtual async Task AddAsync(T entity)
{
    await _dbSet.AddAsync(entity);
    await _context.SaveChangesAsync();  // ← REMOVE THIS!
}

public virtual async Task UpdateAsync(T entity)
{
    _dbSet.Update(entity);
    await _context.SaveChangesAsync();  // ← REMOVE THIS!
}

// AFTER (CORRECT)
public virtual async Task AddAsync(T entity)
{
    await _dbSet.AddAsync(entity);
    // No SaveChanges - Unit of Work handles it
}

public virtual async Task UpdateAsync(T entity)
{
    _dbSet.Update(entity);
    // No SaveChanges - Unit of Work handles it
}
```

**Why this matters:**
```csharp
// With current implementation (WRONG)
await _unitOfWork.Users.AddAsync(user1);      // Saves immediately
await _unitOfWork.Users.AddAsync(user2);      // Saves immediately
await _unitOfWork.SaveChangesAsync();         // Redundant

// If user2 fails, user1 is ALREADY in DB! No rollback! ❌

// With correct implementation (RIGHT)
await _unitOfWork.Users.AddAsync(user1);      // Tracked, not saved
await _unitOfWork.Users.AddAsync(user2);      // Tracked, not saved
await _unitOfWork.SaveChangesAsync();         // Commits ALL or rollbacks ALL ✅
```

---

## 📊 OVERALL ASSESSMENT

### **Final Score: 8.7/10**

| Component | Score | Comment |
|-----------|-------|---------|
| **AutoMapper** | 9.0/10 | Excellent implementation, minor improvements possible |
| **Unit of Work** | 8.5/10 | Good structure, needs GenericRepository fix |
| **Integration** | 9.0/10 | Well integrated with existing architecture |
| **Code Quality** | 9.0/10 | Clean, readable, follows best practices |

### **🎯 Action Items:**

#### **🔴 HIGH PRIORITY (Must Fix):**
1. **Remove `SaveChangesAsync()` from GenericRepository methods**
   - Currently defeats Unit of Work purpose
   - Prevents transaction rollback
   - Critical for data integrity

#### **🟡 MEDIUM PRIORITY (Should Do):**
2. **Fix UnitOfWork.DisposeAsync()**
   - Should dispose DbContext
   - Prevents resource leaks

3. **Add more AutoMapper configurations**
   - Custom value resolvers for role names
   - Validation for complex mappings

#### **🟢 LOW PRIORITY (Nice to Have):**
4. **Add explicit transaction support**
   - BeginTransaction/Commit/Rollback methods
   - Useful for complex multi-step operations

5. **Add AutoMapper validation**
   ```csharp
   // In Program.cs
   builder.Services.AddAutoMapper(config => 
   {
       config.AddProfile<UserProfile>();
       config.AssertConfigurationIsValid();  // ← Validates at startup
   });
   ```

---

## 🚀 BENEFITS ACHIEVED

1. **Code Reduction**
   - AutoMapper: ~30% less mapping code
   - Unit of Work: Centralized transactions

2. **Type Safety**
   - Compile-time mapping validation
   - Generic repository pattern

3. **Maintainability**
   - Single mapping configuration
   - Easy to add new entities

4. **Performance**
   - Lazy-loaded repositories
   - Single database roundtrip

5. **Security**
   - Sensitive data not exposed
   - Password hashing separated

---

**🎉 Overall: Excellent additions to the architecture! Just fix the GenericRepository issue and you'll have a production-ready implementation!**
