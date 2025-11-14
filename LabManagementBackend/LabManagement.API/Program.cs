using System.Text;
using LabManagement.API.Hubs;
using LabManagement.BLL.Implementations;
using LabManagement.BLL.Interfaces;
using LabManagement.BLL.Mappings;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Implementations;
using LabManagement.DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace LabManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Debug: Log connection string (first 50 chars for security)
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connString))
            {
                Console.WriteLine($"🔍 Connection String Preview: {connString.Substring(0, Math.Min(50, connString.Length))}...");
            }
            else
            {
                Console.WriteLine("⚠️  WARNING: Connection String is empty!");
            }
            
            // Add DbContext
            builder.Services.AddDbContext<LabManagementDbContext>(options =>
                options.UseSqlServer(connString));

            // Configure Upstash Redis Connection
            // Option 1: Direct configuration (hardcoded)
            var upstashRedisConnectionString = "crack-oryx-18581.upstash.io:6379,password=AUiVAAIncDI0MDkzMTA1YTcyOGU0OGQwOWVjZTE3YTk1MDZmMGZmNHAyMTg1ODE,ssl=True,abortConnect=False";
            
            // Option 2: Use environment variable (recommended for production)
            // var upstashRedisConnectionString = Environment.GetEnvironmentVariable("UPSTASH_REDIS_CONNECTION") 
            //     ?? "crack-oryx-18581.upstash.io:6379,password=AUiVAAIncDI0MDkzMTA1YTcyOGU0OGQwOWVjZTE3YTk1MDZmMGZmNHAyMTg1ODE,ssl=True,abortConnect=False";
            
            Console.WriteLine($"🔍 Redis Connection String Preview: {upstashRedisConnectionString.Substring(0, Math.Min(50, upstashRedisConnectionString.Length))}...");
            
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                try
                {
                    var connection = ConnectionMultiplexer.Connect(upstashRedisConnectionString);
                    Console.WriteLine("✅ Redis Connection Established Successfully");
                    return connection;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Redis Connection Failed: {ex.Message}");
                    throw new InvalidOperationException($"Failed to connect to Upstash Redis: {ex.Message}", ex);
                }
            });

            // Add unit of work / repositories
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            // Add services
            builder.Services.AddScoped<IRedisHelper, RedisHelper>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ILabService, LabService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ILabZoneService, LabZoneService>();
            builder.Services.AddScoped<IActivityTypeService, ActivityTypeService>();
            builder.Services.AddScoped<ILabEventService, LabEventService>();
            builder.Services.AddScoped<ISecurityLogService, SecurityLogService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IEquipmentService, EquipmentService>();
            builder.Services.AddAutoMapper(typeof(UserProfile), typeof(LabProfile), typeof(BookingProfile), typeof(LabZoneProfile), typeof(ActivityTypeProfile), typeof(LabEventProfile), typeof(SecurityLogProfile), typeof(NotificationProfile), typeof(ReportProfile), typeof(DepartmentProfile));
          
            var jwtKey = builder.Configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is missing in configuration");
            }

            // Add services to the container.
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
                };
            });
            builder.Services.AddControllers();
            
            // Add SignalR with detailed logging
            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true; // Enable detailed error messages for debugging
            });
            
            builder.Services.AddEndpointsApiExplorer();

            // Add CORS - Allow Frontend to call API and SignalR
            builder.Services.AddCors(options =>
            {
                // Development policy - Allow common dev origins with credentials for SignalR
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:3000",      // React dev
                            "http://localhost:5173",      // Vite dev
                            "http://localhost:4200",      // Angular dev
                            "http://localhost:8080"       // Vue dev
                          )
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();          // Required for SignalR
                });
                
                // Mobile app policy - Allow any origin for mobile apps
                options.AddPolicy("AllowMobile", policy =>
                {
                    policy.AllowAnyOrigin()              // Mobile apps can come from any origin
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
                
                // More restrictive policy for production (recommended)
                options.AddPolicy("ProductionPolicy", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:3000",      // React dev
                            "http://localhost:5173",      // Vite dev
                            "https://lab-management-fe.vercel.app"  // Production frontend (removed trailing slash)
                          )
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            //Bearer
            builder.Services.AddSwaggerGen(options =>
            {
                // Configure basic information
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API LABMANAGEMENT",
                    Version = "v1",
                    Description = "API for Lab Management"
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddAuthorization();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            
            // Add Global Exception Handling Middleware (MUST be first)
            app.UseMiddleware<LabManagement.API.Middleware.ExceptionMiddleware>();
            
            // Enable CORS (MUST be before Authentication/Authorization)
            // Use AllowMobile for mobile apps, AllowAll for web dev, ProductionPolicy for production
            var corsPolicy = app.Environment.IsDevelopment() ? "AllowMobile" : "AllowMobile";
            app.UseCors(corsPolicy);
            
            // if (app.Environment.IsDevelopment())
            // {
            //     app.UseSwagger();
            //     app.UseSwaggerUI();
            // }

            // Enable Swagger for all environments (including Production)
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            //app.UseAuthentication();
            //app.UseAuthorization();


            app.MapControllers();
            
            // Map SignalR Hubs
            app.MapHub<BookingHub>("/hubs/booking");
            app.MapHub<EquipmentHub>("/hubs/equipment");
            app.MapHub<SecurityLogHub>("/hubs/security");
            app.MapHub<LabEventHub>("/hubs/lab-events");
            app.MapHub<NotificationHub>("/hubs/notifications");

            app.Run();
        }
    }
}
