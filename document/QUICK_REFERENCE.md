# 🚀 Quick Reference Card

## 📁 PROJECT STRUCTURE (Updated)

```
LabManagementBackend/
├── API/         Controllers + Middleware
├── BLL/         Interfaces/ + Implementations/ + DTOs/
├── DAL/         Interfaces/ + Implementations/ + Models/
└── Common/      Exceptions/ + Models/ + Constants/
```

---

## 🔥 COMMON TASKS

### ✅ How to throw exceptions in Service:
```csharp
// Not Found
throw new NotFoundException("User", id);

// Bad Request
throw new BadRequestException("Email already exists");

// Unauthorized
throw new UnauthorizedException("Invalid credentials");
```

### ✅ How to return success in Controller:
```csharp
return Ok(ApiResponse<UserDTO>.SuccessResponse(user, "User found"));
```

### ✅ How to return error manually:
```csharp
return BadRequest(ApiResponse<object>.ErrorResponse(
    "Validation failed",
    new List<string> { "Email is required", "Password too short" }
));
```

---

## 🎯 COMMON PATTERNS

### Create new Repository:

**1. Interface (DAL/Interfaces/):**
```csharp
public interface ILabRepository : IGenericRepository<Lab>
{
    Task<Lab?> GetByNameAsync(string name);
    Task<IEnumerable<Lab>> GetByZoneAsync(int zoneId);
}
```

**2. Implementation (DAL/Implementations/):**
```csharp
public class LabRepository : GenericRepository<Lab>, ILabRepository
{
    public LabRepository(LabManagementDbContext context) : base(context) {}
    
    public async Task<Lab?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(l => l.Name == name);
    }
    
    public async Task<IEnumerable<Lab>> GetByZoneAsync(int zoneId)
    {
        return await _dbSet.Where(l => l.ZoneId == zoneId).ToListAsync();
    }
}
```

**3. Register in DI (Program.cs):**
```csharp
builder.Services.AddScoped<ILabRepository, LabRepository>();
```

---

### Create new Service:

**1. Interface (BLL/Interfaces/):**
```csharp
public interface ILabService
{
    Task<IEnumerable<LabDTO>> GetAllLabsAsync();
    Task<LabDTO?> GetLabByIdAsync(int id);
    Task<LabDTO> CreateLabAsync(CreateLabDTO dto);
}
```

**2. Implementation (BLL/Implementations/):**
```csharp
public class LabService : ILabService
{
    private readonly ILabRepository _labRepository;
    
    public LabService(ILabRepository labRepository)
    {
        _labRepository = labRepository;
    }
    
    public async Task<LabDTO?> GetLabByIdAsync(int id)
    {
        var lab = await _labRepository.GetByIdAsync(id);
        if (lab == null) return null;
        
        return new LabDTO { /* map properties */ };
    }
}
```

**3. Register in DI (Program.cs):**
```csharp
builder.Services.AddScoped<ILabService, LabService>();
```

---

### Create new Controller:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LabController : ControllerBase
{
    private readonly ILabService _labService;
    
    public LabController(ILabService labService)
    {
        _labService = labService;
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = "2,3,4")]
    public async Task<ActionResult<ApiResponse<LabDTO>>> GetLab(int id)
    {
        var lab = await _labService.GetLabByIdAsync(id);
        
        if (lab == null)
            throw new NotFoundException("Lab", id);
        
        return Ok(ApiResponse<LabDTO>.SuccessResponse(lab, "Lab retrieved"));
    }
}
```

---

## 🔐 AUTHORIZATION QUICK REFERENCE

```csharp
[AllowAnonymous]                                       // No authentication required
[Authorize]                                            // Any authenticated user
[Authorize(Roles = nameof(Constant.UserRole.Admin))]   // Admin only
[Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")] // SchoolManager or Admin
[Authorize(Roles = $"{nameof(Constant.UserRole.LabManager)},{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")] // LabManager, SchoolManager, Admin
```

**Role Values:**
- `0` = Admin
- `1` = SchoolManager
- `2` = LabManager
- `3` = SecurityLab
- `4` = Member

---

## 📝 COMMON REPOSITORY METHODS

```csharp
// From IGenericRepository<T>
await _repository.GetByIdAsync(id);
await _repository.GetAllAsync();
await _repository.FindAsync(x => x.Status == 1);
await _repository.FirstOrDefaultAsync(x => x.Email == email);
await _repository.AddAsync(entity);
await _repository.UpdateAsync(entity);
await _repository.DeleteAsync(entity);
await _repository.ExistsAsync(x => x.Email == email);
await _repository.CountAsync(x => x.IsActive);
```

---

## 🎨 API RESPONSE EXAMPLES

### Success:
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { /* your data */ },
  "errors": []
}
```

### Error:
```json
{
  "success": false,
  "message": "Error occurred",
  "data": null,
  "errors": ["Error detail 1", "Error detail 2"]
}
```

---

## 🧪 TESTING COMMANDS

```bash
# Build
dotnet build

# Run
dotnet run --project LabManagement.API

# Test endpoint
curl http://localhost:5000/api/User/1 \
  -H "Authorization: Bearer YOUR_TOKEN"

# Test exception
curl http://localhost:5000/api/Example/not-found-example/999
```

---

## 📦 ADD NEW PACKAGE

```bash
# To DAL
dotnet add LabManagement.DAL package PackageName

# To BLL
dotnet add LabManagement.BLL package PackageName

# To API
dotnet add LabManagement.API package PackageName
```

---

## 🔄 MIGRATION COMMANDS

```bash
# Add migration
dotnet ef migrations add MigrationName --project LabManagement.DAL --startup-project LabManagement.API

# Update database
dotnet ef database update --project LabManagement.DAL --startup-project LabManagement.API

# Remove last migration
dotnet ef migrations remove --project LabManagement.DAL --startup-project LabManagement.API
```

---

## 🐛 DEBUGGING TIPS

### Check DI registrations:
```csharp
// In Program.cs, before app.Run():
Console.WriteLine("Registered services:");
foreach (var service in builder.Services)
{
    Console.WriteLine($"- {service.ServiceType.Name} -> {service.ImplementationType?.Name}");
}
```

### Check middleware order:
```csharp
// Correct order:
app.UseMiddleware<ExceptionMiddleware>();  // ← First!
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

### Test exceptions:
```bash
# Use Example controller endpoints
GET /api/Example/not-found-example/999    # 404
GET /api/Example/bad-request-example      # 400
GET /api/Example/unauthorized-example     # 401
GET /api/Example/error-example            # 500
```

---

## ⚡ PERFORMANCE TIPS

```csharp
// ❌ Bad: N+1 queries
var users = await _repository.GetAllAsync();
foreach (var user in users)
{
    var labs = await _labRepository.GetByUserAsync(user.Id); // N queries!
}

// ✅ Good: Include related data
var users = await _context.Users
    .Include(u => u.Labs)
    .ToListAsync();

// ✅ Good: Use AsNoTracking for read-only
var users = await _context.Users
    .AsNoTracking()
    .ToListAsync();
```

---

## 📚 USEFUL LINKS

- **API Docs:** `API_RESPONSE_DOCUMENTATION.md`
- **Summary:** `RESTRUCTURING_SUMMARY.md`
- **Commit Guide:** `COMMIT_GUIDE.md`
- **Swagger:** `http://localhost:5000/swagger`

---

## 🎯 REMEMBER

✅ **Services throw exceptions** - Controllers don't need try-catch
✅ **Use ApiResponse<T>** - Consistent response format
✅ **Inject via interfaces** - IUserRepository, IUserService
✅ **Repository pattern** - All data access through repositories
✅ **DI everything** - No `new` in constructors
✅ **Authorize endpoints** - Use [Authorize(Roles = "...")] 

---

**🎊 Happy Coding! 🚀**
